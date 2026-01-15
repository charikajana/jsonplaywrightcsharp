using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Executor;

/// <summary>
/// Handles NAVIGATE actions using centralized WebActions
/// </summary>
public static class NavigationHandler
{
    public static async Task ExecuteNavigate(IPage page, ActionData action, string originalGherkinStep)
    {
        // Extract URL
        var url = action.Value ?? string.Empty;

        if (string.IsNullOrWhiteSpace(url) || url == "___RUNTIME_PARAMETER___")
        {
            url = ParameterExtractor.ExtractParameter(originalGherkinStep, 0) ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("URL not provided for NAVIGATE action");

        // Resolve URL
        url = UrlResolver.ResolveUrl(url, originalGherkinStep);

        // Execute via WebActions
        await WebActions.Navigate(page, url);
    }
}
