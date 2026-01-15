using System.Runtime.InteropServices;
using Allure.Net.Commons;
using PlaywrightJsonFramework.Core.Config;

namespace PlaywrightJsonFramework.Tests.Utils;

/// <summary>
/// Helper to enrich Allure reports with dynamic metadata
/// </summary>
public static class AllureMetadataHelper
{
    private static readonly string AllureResultsPath = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY") 
        ?? Path.Combine(Directory.GetCurrentDirectory(), "allure-results");

    /// <summary>
    /// Generate environment.properties file for Allure
    /// </summary>
    public static void GenerateEnvironmentProperties()
    {
        try
        {
            Console.WriteLine($"[DEBUG] Allure results directory: {AllureResultsPath}");
            Directory.CreateDirectory(AllureResultsPath);
            var envFilePath = Path.Combine(AllureResultsPath, "environment.properties");

            var properties = new List<string>
            {
                $"OS={RuntimeInformation.OSDescription}",
                $"MachineName={Environment.MachineName}",
                $"Framework=.NET Core 10.0",
                $"Environment={ExecutionConfig.Environment.ToUpper()}",
                $"Browser={ExecutionConfig.BrowserName.ToUpper()}",
                $"Headless={ExecutionConfig.IsHeadless}",
                $"Timeout={ExecutionConfig.DefaultTimeout}ms"
            };

            File.WriteAllLines(envFilePath, properties);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not generate Allure environment properties: {ex.Message}");
        }
    }

    /// <summary>
    /// Generate executor.json file for Allure (CI/CD vs Local tracking)
    /// </summary>
    public static void GenerateExecutorMetadata()
    {
        try
        {
            Directory.CreateDirectory(AllureResultsPath);
            var executorFilePath = Path.Combine(AllureResultsPath, "executor.json");

            var isCI = ExecutionConfig.BuildNumber != "LOCAL_RUN";
            var executor = new
            {
                name = isCI ? "CI/CD Pipeline" : "Local Machine",
                type = isCI ? "ci" : "local",
                buildName = ExecutionConfig.BuildNumber,
                buildUrl = ExecutionConfig.ResultsUrl,
                reportUrl = ExecutionConfig.ResultsUrl
            };

            var json = System.Text.Json.JsonSerializer.Serialize(executor, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(executorFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not generate Allure executor metadata: {ex.Message}");
        }
    }

    /// <summary>
    /// Attach screenshot to Allure
    /// </summary>
    public static void AttachScreenshot(byte[]? screenshot, string name = "Failure Screenshot")
    {
        if (screenshot != null)
        {
            AllureApi.AddAttachment(name, "image/png", screenshot);
        }
    }

    /// <summary>
    /// Attach video to Allure
    /// </summary>
    public static async Task AttachVideo(Microsoft.Playwright.IPage page, string name = "Execution Video")
    {
        if (page.Video != null)
        {
            var videoPath = await page.Video.PathAsync();
            if (File.Exists(videoPath))
            {
                // Wait a moment for Playwright to finish writing the file
                await Task.Delay(500);
                
                AllureApi.AddAttachment(name, "video/webm", videoPath);
            }
        }
    }

    /// <summary>
    /// Extract tags like @issue:123, @tm:456 and add as links to Allure
    /// </summary>
    public static void AddScenarioLinks(IEnumerable<string> tags)
    {
        var processedTags = new List<string>();

        foreach (var tag in tags)
        {
            if (tag.Split(':') is string[] parts && parts.Length > 1)
            {
                var prefix = parts[0].ToLower();
                var id = parts[1];

                if (prefix == "issue")
                {
                    var fullUrl = $"https://jira.com/browse/{id}";
                    AllureApi.AddIssue(name: id, url: fullUrl);
                    processedTags.Add(tag);
                }
                else if (prefix == "tm" || prefix == "tms")
                {
                    var fullUrl = $"https://testrail.com/testcase/{id}";
                    AllureApi.AddLink(name: id, url: fullUrl, type: "tms");
                    processedTags.Add(tag);
                }
                else if (prefix == "rally")
                {
                    var fullUrl = $"https://rally1.rallydev.com/#/detail/defect/{id}";
                    AllureApi.AddLink(name: id, url: fullUrl, type: "rally");
                    processedTags.Add(tag);
                }
            }
        }

        // Filter out these tags from the Allure tags list so they only appear as Links
        if (processedTags.Any())
        {
            AllureLifecycle.Instance.UpdateTestCase(tc => 
            {
                tc.labels.RemoveAll(l => l.name == "tag" && processedTags.Contains(l.value));
            });
        }
    }
}
