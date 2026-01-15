using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Strategy;

/// <summary>
/// Prioritized locator fallback mechanism
/// Tries multiple locator strategies in order of stability
/// </summary>
public static class LocatorStrategy
{
    private const string COMPONENT = "LOCATOR STRATEGY";

    /// <summary>
    /// Find element using fallback priority
    /// Priority: ID > Name > CSS > Selector > XPath > Text > Placeholder > DataTestId
    /// </summary>
    public static async Task<ILocator?> FindElementWithFallback(IPage page, ElementLocators locators)
    {
        var attempts = BuildLocatorAttempts(locators);

        foreach (var attempt in attempts)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(attempt.Selector))
                    continue;

                var locator = page.Locator(attempt.Selector);
                var count = await locator.CountAsync();

                if (count > 0)
                {
                    Logger.Success($"Found element using: {attempt.Strategy}", COMPONENT);
                    return locator.First;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{attempt.Strategy} failed: {ex.Message}", COMPONENT);
            }
        }

        Logger.Error("All locator strategies failed", COMPONENT);
        return null;
    }

    /// <summary>
    /// Build list of locator attempts in priority order
    /// </summary>
    private static List<LocatorAttempt> BuildLocatorAttempts(ElementLocators locators)
    {
        var attempts = new List<LocatorAttempt>();

        // 1. ID (highest priority - most stable)
        if (!string.IsNullOrWhiteSpace(locators.Id))
        {
            attempts.Add(new LocatorAttempt("ID", $"#{locators.Id}"));
        }

        // 2. Name
        if (!string.IsNullOrWhiteSpace(locators.Name))
        {
            attempts.Add(new LocatorAttempt("Name", $"[name='{locators.Name}']"));
        }

        // 3. CSS Selector
        if (!string.IsNullOrWhiteSpace(locators.CssSelector))
        {
            attempts.Add(new LocatorAttempt("CssSelector", locators.CssSelector));
        }

        // 4. Selector (Playwright's best selector - role-based, etc.)
        if (!string.IsNullOrWhiteSpace(locators.Selector))
        {
            attempts.Add(new LocatorAttempt("Selector", locators.Selector));
        }

        // 5. XPath
        if (!string.IsNullOrWhiteSpace(locators.XPath))
        {
            attempts.Add(new LocatorAttempt("XPath", locators.XPath));
        }

        // 6. Text content
        if (!string.IsNullOrWhiteSpace(locators.Text))
        {
            var escapedText = locators.Text.Replace("'", "\\'");
            attempts.Add(new LocatorAttempt("Text", $"text='{escapedText}'"));
        }

        // 7. Placeholder
        if (!string.IsNullOrWhiteSpace(locators.Placeholder))
        {
            var escapedPlaceholder = locators.Placeholder.Replace("'", "\\'");
            attempts.Add(new LocatorAttempt("Placeholder", $"[placeholder='{escapedPlaceholder}']"));
        }

        // 8. Data-testid (lowest priority but common in modern apps)
        if (!string.IsNullOrWhiteSpace(locators.DataTest))
        {
            attempts.Add(new LocatorAttempt("DataTestId", $"[data-testid='{locators.DataTest}']"));
            attempts.Add(new LocatorAttempt("DataTest", $"[data-test='{locators.DataTest}']"));
        }

        return attempts;
    }

    /// <summary>
    /// Wait for element to be attached (5 second timeout)
    /// </summary>
    public static async Task<bool> WaitForElement(ILocator locator, int timeoutMs = 5000)
    {
        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = timeoutMs
            });
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Represents a single locator resolution attempt
/// </summary>
internal class LocatorAttempt
{
    public string Strategy { get; }
    public string Selector { get; }

    public LocatorAttempt(string strategy, string selector)
    {
        Strategy = strategy;
        Selector = selector;
    }
}
