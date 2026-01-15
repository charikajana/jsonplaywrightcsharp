using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Playwright;
using PlaywrightJsonFramework.Core.Utils;
using Reqnroll;

namespace PlaywrightJsonFramework.Tests.StepDefinitions;

/// <summary>
/// Base class for all traditional step definitions.
/// Provides access to the Page and high-level WebActions.
/// </summary>
public abstract class BaseStepDefinition
{
    protected readonly ScenarioContext _context;

    protected BaseStepDefinition(ScenarioContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Current Playwright Page. Note that if you switch windows, you may need to update this or use the returned Page from window helpers.
    /// </summary>
    protected IPage Page => PlaywrightManager.Instance.Page 
        ?? throw new InvalidOperationException("Browser page is not initialized.");

    /// <summary>
    /// Current Browser Context (useful for window management)
    /// </summary>
    protected IBrowserContext Context => PlaywrightManager.Instance.Context
        ?? throw new InvalidOperationException("Browser context is not initialized.");

    #region -- Simplified Actions --

    protected async Task Navigate(string url) => await WebActions.Navigate(Page, url);

    protected async Task Click(string selector, string? desc = null) 
        => await WebActions.Click(Page.Locator(selector), desc);

    protected async Task DoubleClick(string selector, string? desc = null) 
        => await WebActions.DoubleClick(Page.Locator(selector), desc);

    protected async Task RightClick(string selector, string? desc = null) 
        => await WebActions.RightClick(Page.Locator(selector), desc);

    protected async Task Type(string selector, string value, string? desc = null) 
        => await WebActions.Type(Page.Locator(selector), value, desc);

    protected async Task Clear(string selector, string? desc = null) 
        => await WebActions.Clear(Page.Locator(selector), desc);

    protected async Task PressKey(string selector, string key, string? desc = null) 
        => await WebActions.PressKey(Page.Locator(selector), key, desc);

    protected async Task Check(string selector, string? desc = null) 
        => await WebActions.Check(Page.Locator(selector), desc);

    protected async Task Uncheck(string selector, string? desc = null) 
        => await WebActions.Uncheck(Page.Locator(selector), desc);

    protected async Task Hover(string selector, string? desc = null) 
        => await WebActions.Hover(Page.Locator(selector), desc);

    protected async Task Select(string selector, string option, string? desc = null) 
        => await WebActions.Select(Page.Locator(selector), option, desc);

    protected async Task DragAndDrop(string sourceSelector, string targetSelector, string? desc = null)
        => await WebActions.DragAndDrop(Page.Locator(sourceSelector), Page.Locator(targetSelector), desc);

    protected async Task ScrollTo(string selector, string? desc = null)
        => await WebActions.ScrollTo(Page.Locator(selector), desc);

    protected async Task UploadFile(string selector, string filePath, string? desc = null)
        => await WebActions.UploadFile(Page.Locator(selector), filePath, desc);

    protected async Task HandleDialog(string action = "accept", string? promptText = null)
        => await WebActions.HandleDialog(Page, action, promptText);

    protected async Task WaitForNetworkIdle()
        => await WebActions.WaitForNetworkIdle(Page);

    protected async Task JsEvaluate(string script, string? desc = null)
        => await WebActions.JsEvaluate(Page, script, desc);

    #endregion

    #region -- Verifications --

    protected async Task VerifyText(string selector, string expected, string? desc = null)
        => await WebActions.VerifyText(Page.Locator(selector), expected, desc);

    protected async Task VerifyVisible(string selector, string? desc = null)
        => await WebActions.VerifyElementVisible(Page.Locator(selector), desc);

    protected async Task VerifyAttribute(string selector, string attr, string expected, string? desc = null)
        => await WebActions.VerifyAttribute(Page.Locator(selector), attr, expected, desc);

    protected async Task VerifyCss(string selector, string prop, string expected, string? desc = null)
        => await WebActions.VerifyCss(Page.Locator(selector), prop, expected, desc);

    #endregion

    #region -- State Getters --

    protected async Task<string> GetText(string selector) => await WebActions.GetText(Page.Locator(selector));
    
    protected async Task<string?> GetAttribute(string selector, string attr) 
        => await WebActions.GetAttribute(Page.Locator(selector), attr);

    protected async Task<string> GetTitle() => await WebActions.GetTitle(Page);

    protected async Task<bool> IsVisible(string selector) => await WebActions.IsVisible(Page.Locator(selector));
    
    protected async Task<bool> IsEnabled(string selector) => await WebActions.IsEnabled(Page.Locator(selector));

    protected async Task<bool> IsChecked(string selector) => await WebActions.IsChecked(Page.Locator(selector));

    #endregion

    #region -- Utilities & Windows --

    protected async Task TakeScreenshot(string name) => await WebActions.TakeScreenshot(Page, name);

    /// <summary>
    /// Wait for a new window/tab to open and return it
    /// </summary>
    protected async Task<IPage> WaitForNewWindow(Func<Task> triggerAction) 
        => await WebActions.WaitForNewPage(Context, triggerAction);

    /// <summary>
    /// Wait for a popup triggered from the current page
    /// </summary>
    protected async Task<IPage> WaitForPopup(Func<Task> triggerAction)
        => await WebActions.WaitForPopup(Page, triggerAction);

    /// <summary>
    /// Switch to window by index
    /// </summary>
    protected async Task<IPage> SwitchToWindow(int index) => await WebActions.SwitchToPage(Context, index);

    /// <summary>
    /// Switch to window by partial title
    /// </summary>
    protected async Task<IPage> SwitchToWindow(string title) => await WebActions.SwitchToPage(Context, title);

    /// <summary>
    /// Close the current active page
    /// </summary>
    protected async Task CloseWindow() => await WebActions.ClosePage(Page);

    #endregion
}
