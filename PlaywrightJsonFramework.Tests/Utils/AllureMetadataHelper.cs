using System.Runtime.InteropServices;
using Allure.Net.Commons;
using PlaywrightJsonFramework.Core.Config;

namespace PlaywrightJsonFramework.Tests.Utils;

/// <summary>
/// Helper to enrich Allure reports with dynamic metadata
/// </summary>
public static class AllureMetadataHelper
{
    private static readonly string AllureResultsPath = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");

    /// <summary>
    /// Generate environment.properties file for Allure
    /// </summary>
    public static void GenerateEnvironmentProperties()
    {
        try
        {
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
}
