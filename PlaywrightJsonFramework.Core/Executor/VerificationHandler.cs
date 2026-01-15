using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Healing;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Executor;

/// <summary>
/// Handles verification actions: VERIFY_TEXT, VERIFY_ELEMENT
/// Delegating to WebActions for centralized verification logic.
/// </summary>
public static class VerificationHandler
{
    public static async Task ExecuteVerifyText(IPage page, ActionData action, string originalGherkinStep, int parameterIndex = 0)
    {
        var locator = await GetLocator(page, action, "VERIFY_TEXT");
        var expectedText = action.Value ?? string.Empty;

        if (expectedText == "___RUNTIME_PARAMETER___" || string.IsNullOrWhiteSpace(expectedText))
        {
            expectedText = ParameterExtractor.ExtractParameter(originalGherkinStep, parameterIndex) ?? string.Empty;
        }

        await WebActions.VerifyText(locator, expectedText, action.Description);
    }

    public static async Task ExecuteVerifyElement(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "VERIFY_ELEMENT");
        await WebActions.VerifyElementVisible(locator, action.Description);
    }

    public static async Task ExecuteVerifyAttribute(IPage page, ActionData action)
    {
        var locator = await GetLocator(page, action, "VERIFY_ATTRIBUTE");
        var parts = action.Value?.Split(':') ?? new[] { "src", "" };
        var attrName = parts[0];
        var expected = parts.Length > 1 ? parts[1] : "";
        
        await WebActions.VerifyAttribute(locator, attrName, expected, action.Description);
    }

    public static async Task ExecuteVerifyCss(IPage page, ActionData action)
    {
        var locator = await GetLocator(page, action, "VERIFY_CSS");
        var parts = action.Value?.Split(':') ?? new[] { "color", "" };
        var propName = parts[0];
        var expected = parts.Length > 1 ? parts[1] : "";

        await WebActions.VerifyCss(locator, propName, expected, action.Description);
    }

    public static async Task ExecuteVerifyElementNotVisible(IPage page, ActionData action, string originalGherkinStep)
    {
        if (action.Element == null)
            throw new InvalidOperationException("Element locators not provided for VERIFY_ELEMENT_NOT_VISIBLE action");

        var locator = await SmartLocatorFinder.FindElement(page, action.Element);
        Logger.Info($"Verifying element is not visible: {action.Description}", "VERIFICATION");

        if (locator != null && await locator.IsVisibleAsync())
        {
            throw new Exception($"Verification FAILED: {action.Description} is visible but should NOT be.");
        }

        Logger.Success("Element not visible verification passed", "VERIFICATION");
    }

    private static async Task<ILocator> GetLocator(IPage page, ActionData action, string context)
    {
        if (action.Element == null)
            throw new InvalidOperationException($"Element locators not provided for {context} action");

        var locator = await SmartLocatorFinder.FindElement(page, action.Element);
        if (locator == null)
            throw new Exception($"Element not found for {context}: {action.Description}");

        return locator;
    }
}
