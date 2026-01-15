using PlaywrightJsonFramework.Core.Config;
using PlaywrightJsonFramework.Core.Utils;
using Reqnroll;

namespace PlaywrightJsonFramework.Tests.StepDefinitions;

[Binding]
public class Hooks
{
    private static int _passedCount = 0;
    private static int _failedCount = 0;

    [AfterScenario]
    public void TrackScenarioStatus(ScenarioContext context)
    {
        if (context.TestError == null)
            _passedCount++;
        else
            _failedCount++;
    }

    [AfterTestRun]
    public static async Task SendFinalReport()
    {
        if (!ExecutionConfig.EnableEmail) return;

        Logger.Info("Generating final email report with CI/CD metadata...", "HOOKS");

        var subject = $"[Test Report] Build #{ExecutionConfig.BuildNumber} - {ExecutionConfig.Environment.ToUpper()} - {(_failedCount > 0 ? "FAILED" : "PASSED")}";
        
        var body = EmailUtils.GenerateHtmlReport(
            _passedCount, 
            _failedCount, 
            ExecutionConfig.Environment, 
            ExecutionConfig.BrowserName,
            ExecutionConfig.BuildNumber,
            ExecutionConfig.ResultsUrl
        );

        await EmailUtils.SendTestSummary(subject, body);
    }
}
