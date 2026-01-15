using Microsoft.Playwright;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Comprehensive centralized utility for high-level Playwright actions.
/// Combines SmartWait, Logging, and Playwright API calls.
/// </summary>
public static class WebActions
{
    private const string COMPONENT = "WEB-ACTION";

    // Delegate to allow Tests project to inject Allure attachment logic without Core needing a dependency on Allure
    public static Func<byte[], string, Task>? OnScreenshotCaptured;

    #region -- Interactions --

    public static async Task Click(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "CLICK");
        Logger.Info($"Clicking: {desc}", COMPONENT);
        await locator.ClickAsync();
        Logger.Success($"Clicking {desc} successful", COMPONENT);
    }

    public static async Task DoubleClick(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "DOUBLE_CLICK");
        Logger.Info($"Double-clicking: {desc}", COMPONENT);
        await locator.DblClickAsync();
        Logger.Success($"Double-click {desc} successful", COMPONENT);
    }

    public static async Task RightClick(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "RIGHT_CLICK");
        Logger.Info($"Right-clicking: {desc}", COMPONENT);
        await locator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
        Logger.Success($"Right-click {desc} successful", COMPONENT);
    }

    public static async Task Type(ILocator locator, string value, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "TYPE");
        Logger.Info($"Typing into: {desc}", COMPONENT);
        await locator.FillAsync(value);
        Logger.Success($"Type '{value}' into {desc} successful", COMPONENT);
    }

    public static async Task Clear(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "CLEAR");
        Logger.Info($"Clearing: {desc}", COMPONENT);
        await locator.ClearAsync();
        Logger.Success($"Clear {desc} successful", COMPONENT);
    }

    public static async Task PressKey(ILocator locator, string key, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "PRESS_KEY");
        Logger.Info($"Pressing key '{key}' on: {desc}", COMPONENT);
        await locator.PressAsync(key);
        Logger.Success($"Press key '{key}' on {desc} successful", COMPONENT);
    }

    public static async Task Check(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "CHECK");
        Logger.Info($"Checking: {desc}", COMPONENT);
        await locator.CheckAsync();
        Logger.Success($"Check {desc} successful", COMPONENT);
    }

    public static async Task Uncheck(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "UNCHECK");
        Logger.Info($"Unchecking: {desc}", COMPONENT);
        await locator.UncheckAsync();
        Logger.Success($"Uncheck {desc} successful", COMPONENT);
    }

    public static async Task Hover(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "HOVER");
        Logger.Info($"Hovering over: {desc}", COMPONENT);
        await locator.HoverAsync();
        Logger.Success($"Hover over {desc} successful", COMPONENT);
    }

    public static async Task Select(ILocator locator, string option, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "SELECT");
        Logger.Info($"Selecting '{option}' in: {desc}", COMPONENT);
        await locator.SelectOptionAsync(new[] { option });
        Logger.Success($"Selection in {desc} successful", COMPONENT);
    }

    public static async Task DragAndDrop(ILocator source, ILocator target, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(source, "DRAG");
        await SmartWait.WaitForElementReady(target, "DROP");
        Logger.Info($"Dragging: {desc} to target", COMPONENT);
        await source.DragToAsync(target);
        Logger.Success($"Drag and drop {desc} successful", COMPONENT);
    }

    public static async Task ScrollTo(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        Logger.Info($"Scrolling to: {desc}", COMPONENT);
        await locator.ScrollIntoViewIfNeededAsync();
        Logger.Success($"Scroll to {desc} successful", COMPONENT);
    }

    #endregion

    #region -- Getters / Information --

    public static async Task<string> GetText(ILocator locator) => await locator.InnerTextAsync();
    
    public static async Task<string?> GetAttribute(ILocator locator, string attributeName) 
        => await locator.GetAttributeAsync(attributeName);

    public static async Task<string> GetTitle(IPage page)
    {
        var title = await page.TitleAsync();
        Logger.Info($"Page Title: {title}", COMPONENT);
        return title;
    }

    public static async Task<bool> IsVisible(ILocator locator) => await locator.IsVisibleAsync();

    public static async Task<bool> IsEnabled(ILocator locator) => await locator.IsEnabledAsync();

    public static async Task<bool> IsChecked(ILocator locator) => await locator.IsCheckedAsync();

    #endregion

    #region -- Verifications --

    public static async Task VerifyText(ILocator locator, string expectedText, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "VERIFY_TEXT");
        Logger.Info($"Verifying text in {desc}. Expected: '{expectedText}'", COMPONENT);
        
        var actualText = await locator.InnerTextAsync();
        if (actualText.Contains(expectedText))
        {
            Logger.Success($"Verification passed for {desc}", COMPONENT);
        }
        else
        {
            Logger.Error($"Verification FAILED for {desc}. Actual: '{actualText}'", COMPONENT);
            throw new Exception($"Text verification failed. Expected: {expectedText}, Actual: {actualText}");
        }
    }

    public static async Task VerifyElementVisible(ILocator locator, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "VERIFY_ELEMENT");
        Logger.Info($"Verifying visibility of: {desc}", COMPONENT);
        
        if (await locator.IsVisibleAsync())
        {
            Logger.Success($"{desc} is visible", COMPONENT);
        }
        else
        {
            Logger.Error($"{desc} is NOT visible", COMPONENT);
            throw new Exception($"Visibility verification failed for: {desc}");
        }
    }

    public static async Task VerifyAttribute(ILocator locator, string attributeName, string expectedValue, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "VERIFY_ATTRIBUTE");
        var actualValue = await locator.GetAttributeAsync(attributeName);
        Logger.Info($"Verifying {attributeName} of {desc}. Expected: '{expectedValue}', Actual: '{actualValue}'", COMPONENT);
        
        if (actualValue == expectedValue)
            Logger.Success($"Attribute verification passed", COMPONENT);
        else
            throw new Exception($"Attribute mismatch. Expected: {expectedValue}, Actual: {actualValue}");
    }

    public static async Task VerifyCss(ILocator locator, string propertyName, string expectedValue, string? description = null)
    {
        string desc = description ?? "element";
        await SmartWait.WaitForElementReady(locator, "VERIFY_CSS");
        var actualValue = await locator.EvaluateAsync<string>($"(el) => window.getComputedStyle(el).getPropertyValue('{propertyName}')");
        Logger.Info($"Verifying CSS {propertyName} of {desc}. Expected: '{expectedValue}', Actual: '{actualValue}'", COMPONENT);
        
        if (actualValue.Contains(expectedValue))
            Logger.Success($"CSS verification passed", COMPONENT);
        else
            throw new Exception($"CSS mismatch. Expected: {expectedValue}, Actual: {actualValue}");
    }

    #endregion

    #region -- Page / Navigation / Windows --

    public static async Task Navigate(IPage page, string url)
    {
        Logger.Info($"Navigating to: {url}", "NAVIGATION");
        await page.GotoAsync(url);
        await SmartWait.WaitForPageLoad(page);
        Logger.Success($"Navigation to {url} successful", "NAVIGATION");
    }

    public static async Task UploadFile(ILocator locator, string filePath, string? description = null)
    {
        string desc = description ?? "upload field";
        Logger.Info($"Uploading file to {desc}: {filePath}", COMPONENT);
        await locator.SetInputFilesAsync(filePath);
        Logger.Success("File upload successful", COMPONENT);
    }

    public static async Task HandleDialog(IPage page, string action = "accept", string? promptText = null)
    {
        Logger.Info($"Configuring Dialog handler: {action.ToUpper()}", COMPONENT);
        page.Dialog += async (_, dialog) =>
        {
            Logger.Info($"Dialog detected: [{dialog.Type}] {dialog.Message}", COMPONENT);
            if (action.ToLower() == "accept") 
                await dialog.AcceptAsync(promptText);
            else 
                await dialog.DismissAsync();
            Logger.Success($"Dialog {action}ed", COMPONENT);
        };
    }

    public static async Task WaitForNetworkIdle(IPage page)
    {
        Logger.Info("Waiting for network idle (stability)...", COMPONENT);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Logger.Success("Network is stable", COMPONENT);
    }

    public static async Task TakeScreenshot(IPage page, string name)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"{name}_{timestamp}.png";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "reports", "screenshots", filename);
        
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        byte[] screenshotBytes = await page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
        Logger.Info($"Screenshot saved to: {path}", COMPONENT);

        // Notify subscribers (Tests project) to attach to Allure
        if (OnScreenshotCaptured != null)
        {
            await OnScreenshotCaptured.Invoke(screenshotBytes, name);
        }
    }

    public static async Task<IPage> WaitForNewPage(IBrowserContext context, Func<Task> action)
    {
        Logger.Info("Waiting for new window/tab to open in context...", COMPONENT);
        var newPageTask = context.WaitForPageAsync();
        await action();
        var newPage = await newPageTask;
        await newPage.WaitForLoadStateAsync();
        Logger.Success($"New window detected: {await newPage.TitleAsync()}", COMPONENT);
        return newPage;
    }

    public static async Task<IPage> WaitForPopup(IPage page, Func<Task> action)
    {
        Logger.Info("Waiting for popup window to open...", COMPONENT);
        var popup = await page.RunAndWaitForPopupAsync(action);
        await popup.WaitForLoadStateAsync();
        Logger.Success($"Popup window detected: {await popup.TitleAsync()}", COMPONENT);
        return popup;
    }

    public static async Task<IPage> SwitchToPage(IBrowserContext context, int index)
    {
        var pages = context.Pages;
        if (index < 0 || index >= pages.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Page index out of range");
        }
        var targetPage = pages[index];
        await targetPage.BringToFrontAsync();
        Logger.Info($"Switched to page at index {index}: {await targetPage.TitleAsync()}", COMPONENT);
        return targetPage;
    }

    public static async Task<IPage> SwitchToPage(IBrowserContext context, string title)
    {
        foreach (var page in context.Pages)
        {
            var pageTitle = await page.TitleAsync();
            if (pageTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
            {
                await page.BringToFrontAsync();
                Logger.Info($"Switched to page with title containing '{title}': {pageTitle}", COMPONENT);
                return page;
            }
        }
        throw new Exception($"No page found with title containing: {title}");
    }

    public static async Task ClosePage(IPage page)
    {
        var title = await page.TitleAsync();
        Logger.Info($"Closing page: {title}", COMPONENT);
        await page.CloseAsync();
        Logger.Success("Page closed", COMPONENT);
    }

    public static async Task JsEvaluate(IPage page, string script, string? description = null)
    {
        string desc = description ?? "JavaScript injection";
        Logger.Info($"Executing JS: {desc}", COMPONENT);
        await page.EvaluateAsync(script);
        Logger.Success($"JS execution successful: {desc}", COMPONENT);
    }

    #endregion
}
