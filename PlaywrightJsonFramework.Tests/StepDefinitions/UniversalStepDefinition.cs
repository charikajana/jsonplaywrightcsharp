using System.Reflection;
using Allure.Net.Commons;
using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Config;
using PlaywrightJsonFramework.Core.Executor;
using PlaywrightJsonFramework.Core.Playwright;
using PlaywrightJsonFramework.Core.Repository;
using PlaywrightJsonFramework.Core.Utils;
using PlaywrightJsonFramework.Tests.Utils;
using Reqnroll;

namespace PlaywrightJsonFramework.Tests.StepDefinitions;

/// <summary>
/// Universal Step Definition - Single entry point for ALL Gherkin steps
/// Implements the hybrid JSON-first, code-second strategy
/// </summary>
[Binding]
public class UniversalStepDefinition
{
    private readonly ScenarioContext _scenarioContext;
    private IPage? _page;
    private JsonEnhancedExecutor? _executor;

    public UniversalStepDefinition(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        Logger.Initialize(enableFileLogging: true);
        
        // Subscribe to screenshot events to attach them to Allure
        WebActions.OnScreenshotCaptured = async (bytes, name) => 
        {
            await Task.Run(() => AllureMetadataHelper.AttachScreenshot(bytes, name));
        };

        AllureMetadataHelper.GenerateEnvironmentProperties();
        AllureMetadataHelper.GenerateExecutorMetadata();
        AllureMetadataHelper.AddScenarioLinks(_scenarioContext.ScenarioInfo.CombinedTags);
        Logger.ScenarioStart(_scenarioContext.ScenarioInfo.Title);

        try
        {
            await PlaywrightManager.Instance.InitializeBrowser(
                browserName: ExecutionConfig.BrowserName,
                headless: ExecutionConfig.IsHeadless,
                slowMo: ExecutionConfig.SlowMotion
            );

            _page = PlaywrightManager.Instance.Page;
            if (_page == null) throw new InvalidOperationException("Failed to initialize Playwright page");
            _executor = new JsonEnhancedExecutor(_page);

            AllureLifecycle.Instance.UpdateTestCase(tc =>
            {
                tc.labels.Add(new Label { name = "browser", value = ExecutionConfig.BrowserName });
                tc.labels.Add(new Label { name = "environment", value = ExecutionConfig.Environment });
            });

            Logger.Success("Browser initialized successfully", "FRAMEWORK");
        }
        catch (Exception ex)
        {
            Logger.Error($"Browser initialization failed: {ex.Message}", "FRAMEWORK");
            throw;
        }
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        try
        {
            if (_scenarioContext.TestError != null && _page != null)
            {
                var screenshot = await PlaywrightManager.Instance.TakeScreenshot();
                if (screenshot != null) AllureMetadataHelper.AttachScreenshot(screenshot);
            }

            string? videoPath = null;
            if (_page?.Video != null) videoPath = await _page.Video.PathAsync();

            await PlaywrightManager.Instance.CloseBrowser();

            if (!string.IsNullOrEmpty(videoPath) && File.Exists(videoPath))
            {
                AllureApi.AddAttachment("Execution Video", "video/webm", videoPath);
            }

            Logger.ScenarioEnd(_scenarioContext.TestError == null, _scenarioContext.ScenarioInfo.Title);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Warning during cleanup: {ex.Message}", "FRAMEWORK");
        }
    }

    [Given(@"^(.*)$")]
    public async Task GivenUniversalStep(string stepText) => await ExecuteUniversalStep(stepText, "Given");

    [When(@"^(.*)$")]
    public async Task WhenUniversalStep(string stepText) => await ExecuteUniversalStep(stepText, "When");

    [Then(@"^(.*)$")]
    public async Task ThenUniversalStep(string stepText) => await ExecuteUniversalStep(stepText, "Then");

    private async Task ExecuteUniversalStep(string stepText, string stepType)
    {
        if (_executor == null) throw new InvalidOperationException("Executor not initialized");

        Logger.Step(stepText, stepType.ToUpper());

        // 1. Try JSON
        if (StepRepository.StepExists(stepText))
        {
            Logger.Info("Executing via JSON Repository", "FRAMEWORK");
            AllureLifecycle.Instance.UpdateStep(s => s.name = $"[JSON] {stepText}");
            await _executor.ExecuteStep(stepText);
            Logger.Success("Step executed successfully via JSON", "FRAMEWORK");
        }
        // 2. Try Traditional Fallback
        else
        {
            Logger.Warn("No JSON found. Checking for Traditional Code Definition...", "FRAMEWORK");
            if (!await TryExecuteTraditionalStep(stepText))
            {
                Logger.Error($"Step definition not found: {stepText}", "FRAMEWORK");
                throw new PendingStepException($"No JSON or Traditional definition for: {stepText}");
            }
        }
    }

    private async Task<bool> TryExecuteTraditionalStep(string stepText)
    {
        var traditionalClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.Name.EndsWith("StepDefinitions") && t != typeof(UniversalStepDefinition));

        foreach (var type in traditionalClasses)
        {
            var method = type.GetMethods().FirstOrDefault(m => {
                var attr = m.GetCustomAttribute<TraditionalStepAttribute>();
                return attr != null && System.Text.RegularExpressions.Regex.IsMatch(stepText, attr.Regex);
            });

            if (method != null)
            {
                Logger.Info($"Executing TRADITIONAL fallback: {method.Name} in {type.Name}", "FRAMEWORK");
                AllureLifecycle.Instance.UpdateStep(s => s.name = $"[TRADITIONAL] {stepText}");

                // Instantiate (supports optional ScenarioContext inject)
                object instance;
                try { instance = Activator.CreateInstance(type, _scenarioContext)!; }
                catch { instance = Activator.CreateInstance(type)!; }

                var result = method.Invoke(instance, null);
                if (result is Task task) await task;
                Logger.Success("Traditional fallback executed successfully", "FRAMEWORK");
                return true;
            }
        }
        return false;
    }
}
