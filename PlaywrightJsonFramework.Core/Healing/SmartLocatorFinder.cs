using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Repository;
using PlaywrightJsonFramework.Core.Strategy;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Healing;

/// <summary>
/// Orchestrates standard locator resolution + self-healing
/// This is the main entry point for finding elements
/// </summary>
public static class SmartLocatorFinder
{
    private const string COMPONENT = "SMART FINDER";

    /// <summary>
    /// Find element using standard strategies, with self-healing fallback
    /// </summary>
    public static async Task<ILocator?> FindElement(IPage page, ElementLocators locators)
    {
        Logger.Info("Starting element search...", COMPONENT);

        // Step 1: Wait for element with best locator (5s timeout)
        var bestSelector = GetBestSelector(locators);
        if (!string.IsNullOrWhiteSpace(bestSelector))
        {
            var initialLocator = page.Locator(bestSelector);
            var isPresent = await LocatorStrategy.WaitForElement(initialLocator, 5000);

            if (isPresent)
            {
                Logger.Info($"Element appeared after wait: {bestSelector}", COMPONENT);
            }
        }

        // Step 2: Try Standard Fallback
        Logger.Info("Attempting standard locator strategies...", COMPONENT);
        var locator = await LocatorStrategy.FindElementWithFallback(page, locators);

        if (locator != null)
        {
            // Enrich with live attributes
            await StepRepository.PopulateLiveAttributes(locator, locators);
            Logger.Success("Element found using standard strategies", COMPONENT);
            return locator;
        }

        // Step 3: Self-Healing Engine (only if standard strategies failed)
        Logger.Info("Standard strategies failed, invoking self-healing...", COMPONENT);
        
        // Capture "Before" state
        var beforeState = CaptureElementState(locators);
        
        var healedResult = await SelfHealingEngine.AttemptHealing(page, locators);

        if (healedResult != null)
        {
            // Update locators with healed selector
            locators.Selector = healedResult.Selector;
            locators.IsHealed = true;

            // Enrich with live attributes
            await StepRepository.PopulateLiveAttributes(healedResult.Locator, locators);

            // Capture "After" state
            var afterState = CaptureElementState(locators);

            // Log comprehensive healing report
            LogHealingReport(beforeState, afterState);

            Logger.Success("Element successfully healed", COMPONENT);
            return healedResult.Locator;
        }

        Logger.Error("Element not found - all strategies failed", COMPONENT);
        return null;
    }

    /// <summary>
    /// Get the best selector from available locators
    /// Priority: ID > Selector > CSS > XPath
    /// </summary>
    private static string? GetBestSelector(ElementLocators locators)
    {
        if (!string.IsNullOrWhiteSpace(locators.Id))
            return $"#{locators.Id}";

        if (!string.IsNullOrWhiteSpace(locators.Selector))
            return locators.Selector;

        if (!string.IsNullOrWhiteSpace(locators.CssSelector))
            return locators.CssSelector;

        if (!string.IsNullOrWhiteSpace(locators.XPath))
            return locators.XPath;

        return null;
    }

    /// <summary>
    /// Capture element state for healing report
    /// </summary>
    private static Dictionary<string, string?> CaptureElementState(ElementLocators locators)
    {
        return new Dictionary<string, string?>
        {
            ["ID"] = locators.Id,
            ["Selector"] = locators.Selector,
            ["CssSelector"] = locators.CssSelector,
            ["XPath"] = locators.XPath,
            ["Text"] = locators.Text,
            ["ClassName"] = locators.ClassName
        };
    }

    /// <summary>
    /// Log comprehensive healing report showing before/after changes
    /// </summary>
    private static void LogHealingReport(
        Dictionary<string, string?> before,
        Dictionary<string, string?> after)
    {
        Logger.NewLine();
        Logger.Separator('=');
        Logger.Custom("           COMPREHENSIVE HEALING REPORT", ConsoleColor.Cyan);
        Logger.Separator('=');
        Logger.Info($"{"ATTRIBUTE",-20} | {"BEFORE (BROKEN)",-25} | {"AFTER (HEALED)",-25}");
        Logger.Separator('-');

        foreach (var key in before.Keys)
        {
            var beforeValue = TruncateValue(before[key] ?? "null", 23);
            var afterValue = TruncateValue(after[key] ?? "null", 23);

            var marker = beforeValue != afterValue ? "->" : "  ";
            Logger.Info($"{key,-20} {marker} {beforeValue,-25} | {afterValue,-25}");
        }

        Logger.Separator('=');
        Logger.NewLine();
    }

    /// <summary>
    /// Truncate long values for display
    /// </summary>
    private static string TruncateValue(string value, int maxLength)
    {
        if (value.Length <= maxLength)
            return value;

        return value.Substring(0, maxLength - 3) + "...";
    }
}
