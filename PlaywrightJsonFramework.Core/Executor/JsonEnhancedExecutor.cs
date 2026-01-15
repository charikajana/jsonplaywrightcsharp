using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Repository;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Executor;

/// <summary>
/// Executes steps using JSON-provided locator data
/// Orchestrates action execution by delegating to appropriate handlers
/// </summary>
public class JsonEnhancedExecutor
{
    private readonly IPage _page;
    private const string COMPONENT = "JSON EXECUTOR";

    public JsonEnhancedExecutor(IPage page)
    {
        _page = page;
    }

    /// <summary>
    /// Execute a complete step from its Gherkin text
    /// </summary>
    public async Task ExecuteStep(string gherkinStep)
    {
        Logger.NewLine();
        Logger.Separator('=');
        Logger.Info($"Executing step: {gherkinStep}", COMPONENT);
        Logger.Separator('=');

        // Load step JSON
        var stepData = StepRepository.GetStep(gherkinStep);
        if (stepData == null)
        {
            throw new FileNotFoundException($"JSON not found for step: {gherkinStep}");
        }

        Logger.Info($"Loaded JSON with {stepData.Actions.Count} action(s)", COMPONENT);

        // Execute each action in sequence
        var parameterIndex = 0;
        foreach (var action in stepData.Actions)
        {
            parameterIndex = await ExecuteAction(action, gherkinStep, parameterIndex);
        }

        Logger.Success("Step completed successfully", COMPONENT);
        Logger.NewLine();
    }

    /// <summary>
    /// Execute a single action
    /// </summary>
    private async Task<int> ExecuteAction(ActionData action, string gherkinStep, int parameterIndex)
    {
        Logger.NewLine();
        Logger.Info($"Action #{action.ActionNumber}: {action.ActionType} - {action.Description}", COMPONENT);

        try
        {
            switch (action.ActionType)
            {
                case ActionTypes.NAVIGATE:
                    await NavigationHandler.ExecuteNavigate(_page, action, gherkinStep);
                    break;

                case ActionTypes.CLICK:
                    await InteractionHandler.ExecuteClick(_page, action, gherkinStep);
                    break;

                case ActionTypes.DOUBLE_CLICK:
                    await InteractionHandler.ExecuteDoubleClick(_page, action, gherkinStep);
                    break;

                case ActionTypes.RIGHT_CLICK:
                    await InteractionHandler.ExecuteRightClick(_page, action, gherkinStep);
                    break;

                case ActionTypes.TYPE:
                    await InteractionHandler.ExecuteType(_page, action, gherkinStep, parameterIndex);
                    return parameterIndex + 1;  // Increment for next TYPE action

                case ActionTypes.CLEAR:
                    await InteractionHandler.ExecuteClear(_page, action, gherkinStep);
                    break;

                case ActionTypes.SELECT:
                    await InteractionHandler.ExecuteSelect(_page, action, gherkinStep, parameterIndex);
                    return parameterIndex + 1;  // Increment for next SELECT action

                case ActionTypes.HOVER:
                    await InteractionHandler.ExecuteHover(_page, action, gherkinStep);
                    break;

                case ActionTypes.CHECK:
                    await InteractionHandler.ExecuteCheck(_page, action, gherkinStep);
                    break;

                case ActionTypes.UNCHECK:
                    await InteractionHandler.ExecuteUncheck(_page, action, gherkinStep);
                    break;

                case ActionTypes.PRESS_KEY:
                    await InteractionHandler.ExecutePressKey(_page, action, gherkinStep);
                    break;

                case ActionTypes.VERIFY_TEXT:
                    await VerificationHandler.ExecuteVerifyText(_page, action, gherkinStep, parameterIndex);
                    return parameterIndex + 1;  // Increment if param used

                case ActionTypes.VERIFY_ELEMENT:
                    await VerificationHandler.ExecuteVerifyElement(_page, action, gherkinStep);
                    break;

                case ActionTypes.VERIFY_NOT_VISIBLE:
                    await VerificationHandler.ExecuteVerifyElementNotVisible(_page, action, gherkinStep);
                    break;

                case ActionTypes.SCROLL:
                case ActionTypes.SCROLL_TO:
                    await InteractionHandler.ExecuteScroll(_page, action);
                    break;

                case ActionTypes.DRAG_DROP:
                case ActionTypes.DRAG_AND_DROP:
                    await InteractionHandler.ExecuteDragAndDrop(_page, action);
                    break;

                case ActionTypes.UPLOAD_FILE:
                    await InteractionHandler.ExecuteUploadFile(_page, action);
                    break;

                case ActionTypes.HANDLE_DIALOG:
                    await InteractionHandler.ExecuteHandleDialog(_page, action);
                    break;

                case ActionTypes.WAIT_STABLE:
                    await InteractionHandler.ExecuteWaitStable(_page);
                    break;

                case ActionTypes.VERIFY_ATTRIBUTE:
                    await VerificationHandler.ExecuteVerifyAttribute(_page, action);
                    break;

                case ActionTypes.VERIFY_CSS:
                    await VerificationHandler.ExecuteVerifyCss(_page, action);
                    break;

                case ActionTypes.JS_EVALUATE:
                    await InteractionHandler.ExecuteJsEvaluate(_page, action);
                    break;

                case ActionTypes.SCREENSHOT:
                    await ExecuteScreenshot(action);
                    break;

                default:
                    Logger.Warn($"Unsupported action type: {action.ActionType}", COMPONENT);
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Action failed: {ex.Message}", COMPONENT);
            throw;
        }

        return parameterIndex;  // Return current index if no increment happened
    }

    /// <summary>
    /// Execute screenshot action
    /// </summary>
    private async Task ExecuteScreenshot(ActionData action)
    {
        var desc = action.Description ?? "Manual_Screenshot";
        await WebActions.TakeScreenshot(_page, desc);
    }
}
