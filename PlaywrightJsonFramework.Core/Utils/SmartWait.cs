using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Config;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Smart wait utility for ensuring element readiness before actions
/// Provides intelligent waiting strategies for different action types
/// </summary>
public static class SmartWait
{
    private const string COMPONENT = "SMART WAIT";

    /// <summary>
    /// Wait for element to be ready for interaction
    /// Combines visibility, enabled, and stability checks
    /// </summary>
    public static async Task<bool> WaitForElementReady(
        ILocator locator,
        string actionType,
        int timeoutMs = 0)
    {
        if (timeoutMs == 0)
            timeoutMs = ExecutionConfig.DefaultTimeout;

        Logger.Info($"Waiting for element to be ready for {actionType} action...", COMPONENT);

        try
        {
            switch (actionType.ToUpperInvariant())
            {
                case "CLICK":
                case "DOUBLE_CLICK":
                case "RIGHT_CLICK":
                    await WaitForClickable(locator, timeoutMs);
                    break;

                case "TYPE":
                case "CLEAR":
                    await WaitForEditable(locator, timeoutMs);
                    break;

                case "SELECT":
                    await WaitForSelectable(locator, timeoutMs);
                    break;

                case "VERIFY_TEXT":
                case "VERIFY_ELEMENT":
                    await WaitForVisible(locator, timeoutMs);
                    break;

                case "HOVER":
                    await WaitForHoverable(locator, timeoutMs);
                    break;

                case "CHECK":
                case "UNCHECK":
                    await WaitForCheckable(locator, timeoutMs);
                    break;

                default:
                    await WaitForVisible(locator, timeoutMs);
                    break;
            }

            Logger.Success("Element is ready", COMPONENT);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Element not ready: {ex.Message}", COMPONENT);
            return false;
        }
    }

    /// <summary>
    /// Wait for element to be clickable (visible, enabled, stable)
    /// </summary>
    private static async Task WaitForClickable(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });

        // Wait for element to be enabled
        await WaitForCondition(
            async () => await locator.IsEnabledAsync(),
            timeoutMs,
            "element to be enabled"
        );

        // Wait for element to be stable (no animations)
        await Task.Delay(100); // Small delay for animation completion
    }

    /// <summary>
    /// Wait for element to be editable (visible, enabled, not readonly)
    /// </summary>
    private static async Task WaitForEditable(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });

        await WaitForCondition(
            async () => await locator.IsEditableAsync(),
            timeoutMs,
            "element to be editable"
        );
    }

    /// <summary>
    /// Wait for select element to be ready
    /// </summary>
    private static async Task WaitForSelectable(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });

        await WaitForCondition(
            async () => await locator.IsEnabledAsync(),
            timeoutMs,
            "select element to be enabled"
        );
    }

    /// <summary>
    /// Wait for element to be visible
    /// </summary>
    private static async Task WaitForVisible(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });
    }

    /// <summary>
    /// Wait for element to be hoverable
    /// </summary>
    private static async Task WaitForHoverable(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });
    }

    /// <summary>
    /// Wait for checkbox/radio to be ready
    /// </summary>
    private static async Task WaitForCheckable(ILocator locator, int timeoutMs)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = timeoutMs
        });

        await WaitForCondition(
            async () => await locator.IsEnabledAsync(),
            timeoutMs,
            "checkbox/radio to be enabled"
        );
    }

    /// <summary>
    /// Wait for page to be fully loaded
    /// </summary>
    public static async Task WaitForPageLoad(IPage page, int timeoutMs = 0)
    {
        if (timeoutMs == 0)
            timeoutMs = ExecutionConfig.DefaultTimeout;

        Logger.Info("Waiting for page load...", COMPONENT);

        try
        {
            // Wait for load state
            await page.WaitForLoadStateAsync(LoadState.Load, new PageWaitForLoadStateOptions
            {
                Timeout = timeoutMs
            });

            // Wait for network idle (optional, but recommended)
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
            {
                Timeout = 5000 // Shorter timeout for network idle
            });

            Logger.Success("Page loaded", COMPONENT);
        }
        catch (TimeoutException)
        {
            Logger.Warn("Network idle timeout (page may still be usable)", COMPONENT);
            // Continue anyway - network idle is optional
        }
    }

    /// <summary>
    /// Wait for navigation to complete
    /// </summary>
    public static async Task WaitForNavigation(IPage page, int timeoutMs = 0)
    {
        if (timeoutMs == 0)
            timeoutMs = ExecutionConfig.DefaultTimeout;

        Logger.Info("Waiting for navigation...", COMPONENT);

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded, new PageWaitForLoadStateOptions
        {
            Timeout = timeoutMs
        });

        Logger.Success("Navigation complete", COMPONENT);
    }

    /// <summary>
    /// Wait for custom condition to be true
    /// </summary>
    private static async Task WaitForCondition(
        Func<Task<bool>> condition,
        int timeoutMs,
        string description)
    {
        var startTime = DateTime.Now;
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);

        while (DateTime.Now - startTime < timeout)
        {
            if (await condition())
                return;

            await Task.Delay(100); // Poll every 100ms
        }

        throw new TimeoutException($"Timeout waiting for {description}");
    }

    /// <summary>
    /// Wait for element count to stabilize
    /// Useful when waiting for dynamic lists to load
    /// </summary>
    public static async Task WaitForStableCount(
        ILocator locator,
        int stabilityDurationMs = 500,
        int timeoutMs = 0)
    {
        if (timeoutMs == 0)
            timeoutMs = ExecutionConfig.DefaultTimeout;

        Logger.Info("Waiting for stable element count...", COMPONENT);

        var startTime = DateTime.Now;
        var timeout = TimeSpan.FromMilliseconds(timeoutMs);
        int? lastCount = null;
        var stableStart = DateTime.Now;

        while (DateTime.Now - startTime < timeout)
        {
            var currentCount = await locator.CountAsync();

            if (lastCount == currentCount)
            {
                // Count is stable
                if (DateTime.Now - stableStart > TimeSpan.FromMilliseconds(stabilityDurationMs))
                {
                    Logger.Success($"Count stabilized at {currentCount}", COMPONENT);
                    return;
                }
            }
            else
            {
                // Count changed, reset stability timer
                lastCount = currentCount;
                stableStart = DateTime.Now;
            }

            await Task.Delay(100);
        }

        throw new TimeoutException("Timeout waiting for stable element count");
    }

    /// <summary>
    /// Wait for text to appear in element
    /// </summary>
    public static async Task WaitForText(
        ILocator locator,
        string expectedText,
        int timeoutMs = 0)
    {
        if (timeoutMs == 0)
            timeoutMs = ExecutionConfig.DefaultTimeout;

        Logger.Info($"Waiting for text: '{expectedText}'...", COMPONENT);

        await WaitForCondition(
            async () =>
            {
                var text = await locator.TextContentAsync() ?? "";
                return text.Contains(expectedText);
            },
            timeoutMs,
            $"text '{expectedText}' to appear"
        );

        Logger.Success("Text appeared", COMPONENT);
    }

    /// <summary>
    /// Smart delay with logging
    /// </summary>
    public static async Task Delay(int milliseconds, string reason = "")
    {
        if (!string.IsNullOrEmpty(reason))
            Logger.Info($"Delaying {milliseconds}ms: {reason}", COMPONENT);

        await Task.Delay(milliseconds);
    }
}
