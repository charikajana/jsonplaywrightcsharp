using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Healing;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Executor;

/// <summary>
/// Handles interaction actions: CLICK, TYPE, SELECT, HOVER, CHECK, etc.
/// Delegating to WebActions for centralized interaction logic.
/// </summary>
public static class InteractionHandler
{
    private const string COMPONENT = "INTERACTION";

    public static async Task ExecuteClick(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "CLICK");
        await WebActions.Click(locator, action.Description);
    }

    public static async Task ExecuteType(IPage page, ActionData action, string originalGherkinStep, int parameterIndex = 0)
    {
        var locator = await GetLocator(page, action, "TYPE");
        var value = action.Value ?? string.Empty;

        if (value == "___RUNTIME_PARAMETER___" || string.IsNullOrWhiteSpace(value))
        {
            value = ParameterExtractor.ExtractParameter(originalGherkinStep, parameterIndex) ?? string.Empty;
        }

        await WebActions.Type(locator, value, action.Description);
    }

    public static async Task ExecuteSelect(IPage page, ActionData action, string originalGherkinStep, int parameterIndex = 0)
    {
        var locator = await GetLocator(page, action, "SELECT");
        var value = action.Value ?? string.Empty;
        
        if (value == "___RUNTIME_PARAMETER___" || string.IsNullOrWhiteSpace(value))
        {
            value = ParameterExtractor.ExtractParameter(originalGherkinStep, parameterIndex) ?? string.Empty;
        }

        if (DateResolver.IsDateKeyword(value))
        {
            var resolvedDate = DateResolver.Resolve(value);
            await WebActions.Type(locator, resolvedDate, $"{action.Description} (Date)");
        }
        else
        {
            await WebActions.Select(locator, value, action.Description);
        }
    }

    public static async Task ExecuteHover(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "HOVER");
        await WebActions.Hover(locator, action.Description);
    }

    public static async Task ExecuteCheck(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "CHECK");
        await WebActions.Check(locator, action.Description);
    }

    public static async Task ExecuteUncheck(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "UNCHECK");
        await WebActions.Uncheck(locator, action.Description);
    }

    public static async Task ExecuteDoubleClick(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "DOUBLE_CLICK");
        await WebActions.DoubleClick(locator, action.Description);
    }

    public static async Task ExecuteRightClick(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "RIGHT_CLICK");
        await WebActions.RightClick(locator, action.Description);
    }

    public static async Task ExecutePressKey(IPage page, ActionData action, string originalGherkinStep)
    {
        var key = action.Value ?? "Enter";
        
        // Use page directly or locator if element exists
        if (action.Element != null)
        {
            var locator = await GetLocator(page, action, "PRESS_KEY");
            await WebActions.PressKey(locator, key, action.Description);
        }
        else
        {
            Logger.Info($"Pressing key: {key} (Global)", COMPONENT);
            await page.Keyboard.PressAsync(key);
            Logger.Success("Key press successful", COMPONENT);
        }
    }

    public static async Task ExecuteClear(IPage page, ActionData action, string originalGherkinStep)
    {
        var locator = await GetLocator(page, action, "CLEAR");
        await WebActions.Clear(locator, action.Description);
    }

    public static async Task ExecuteScroll(IPage page, ActionData action)
    {
        var locator = await GetLocator(page, action, "SCROLL");
        await WebActions.ScrollTo(locator, action.Description);
    }

    public static async Task ExecuteDragAndDrop(IPage page, ActionData action)
    {
        var source = await GetLocator(page, action, "DRAG_SOURCE");
        
        if (action.TargetElement == null)
            throw new InvalidOperationException("Target element locators missing for DRAG_AND_DROP");

        var target = await SmartLocatorFinder.FindElement(page, action.TargetElement);
        if (target == null)
            throw new Exception("Drag target element not found");

        await WebActions.DragAndDrop(source, target, action.Description);
    }

    public static async Task ExecuteJsEvaluate(IPage page, ActionData action)
    {
        if (string.IsNullOrEmpty(action.Value))
            throw new InvalidOperationException("JavaScript script missing in 'value' field");

        await WebActions.JsEvaluate(page, action.Value, action.Description);
    }

    public static async Task ExecuteUploadFile(IPage page, ActionData action)
    {
        var locator = await GetLocator(page, action, "UPLOAD");
        if (string.IsNullOrEmpty(action.Value))
            throw new InvalidOperationException("File path missing in 'value' field");

        await WebActions.UploadFile(locator, action.Value, action.Description);
    }

    public static async Task ExecuteHandleDialog(IPage page, ActionData action)
    {
        var dialAct = action.Value ?? "accept";
        await WebActions.HandleDialog(page, dialAct);
    }

    public static async Task ExecuteWaitStable(IPage page)
    {
        await WebActions.WaitForNetworkIdle(page);
    }

    public static async Task<IPage> ExecuteSwitchWindow(IBrowserContext context, ActionData action)
    {
        IPage newPage;
        if (int.TryParse(action.Value, out int index))
        {
            newPage = await WebActions.SwitchToPage(context, index);
        }
        else
        {
            newPage = await WebActions.SwitchToPage(context, action.Value ?? "");
        }
        return newPage;
    }

    public static async Task<IPage> ExecuteClickAndSwitch(IPage page, ActionData action)
    {
        var locator = await GetLocator(page, action, "CLICK_AND_SWITCH");
        
        Logger.Info($"Clicking and waiting for popup: {action.Description}", COMPONENT);
        
        var popup = await page.RunAndWaitForPopupAsync(async () =>
        {
            await locator.ClickAsync();
        });

        await popup.WaitForLoadStateAsync();
        Logger.Success($"Switched to popup: {await popup.TitleAsync()}", COMPONENT);
        
        return popup;
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
