using System.Text.Json.Serialization;

namespace PlaywrightJsonFramework.Core.Data;

/// <summary>
/// Model for individual action within a step
/// Each step can contain multiple actions (e.g., TYPE username, TYPE password)
/// </summary>
public class ActionData
{
    [JsonPropertyName("actionNumber")]
    public int ActionNumber { get; set; }
    
    [JsonPropertyName("actionType")]
    public string ActionType { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("element")]
    public ElementLocators? Element { get; set; }
    
    [JsonPropertyName("value")]
    public string? Value { get; set; }  // For TYPE/SELECT/JS_EVALUATE actions
    
    [JsonPropertyName("targetElement")]
    public ElementLocators? TargetElement { get; set; }  // For DRAG_AND_DROP
}

/// <summary>
/// Supported action types in the framework
/// </summary>
public static class ActionTypes
{
    public const string NAVIGATE = "NAVIGATE";
    public const string CLICK = "CLICK";
    public const string DOUBLE_CLICK = "DOUBLE_CLICK";
    public const string RIGHT_CLICK = "RIGHT_CLICK";
    public const string TYPE = "TYPE";
    public const string CLEAR = "CLEAR";
    public const string SELECT = "SELECT";
    public const string HOVER = "HOVER";
    public const string CHECK = "CHECK";
    public const string UNCHECK = "UNCHECK";
    public const string PRESS_KEY = "PRESS_KEY";
    public const string SWITCH_WINDOW = "SWITCH_WINDOW";
    public const string DRAG_DROP = "DRAG_DROP"; // Legacy
    public const string DRAG_AND_DROP = "DRAG_AND_DROP";
    public const string SCROLL = "SCROLL"; // Legacy
    public const string SCROLL_TO = "SCROLL_TO";
    public const string WAIT_NAVIGATION = "WAIT_NAVIGATION";
    public const string VERIFY_TEXT = "VERIFY_TEXT";
    public const string VERIFY_ELEMENT = "VERIFY_ELEMENT";
    public const string VERIFY_NOT_VISIBLE = "VERIFY_NOT_VISIBLE";
    public const string VERIFY_ATTRIBUTE = "VERIFY_ATTRIBUTE";
    public const string VERIFY_CSS = "VERIFY_CSS";
    public const string SCREENSHOT = "SCREENSHOT";
    public const string JS_EVALUATE = "JS_EVALUATE";
    public const string UPLOAD_FILE = "UPLOAD_FILE";
    public const string HANDLE_DIALOG = "HANDLE_DIALOG";
    public const string WAIT_STABLE = "WAIT_STABLE";
}
