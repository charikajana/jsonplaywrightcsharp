using System.Text.Json.Serialization;

namespace PlaywrightJsonFramework.Core.Data;

/// <summary>
/// The "Strong JSON" identity - 17 mandatory attributes + fingerprint DNA
/// This class represents the complete element locator information captured from the DOM
/// </summary>
public class ElementLocators
{
    // ============ Core Identifiers ============
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }  // Tag name (input, button, etc.)
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("selector")]
    public string? Selector { get; set; }  // Playwright selector
    
    [JsonPropertyName("cssSelector")]
    public string? CssSelector { get; set; }
    
    [JsonPropertyName("xpath")]
    public string? XPath { get; set; }  // Relative XPath
    
    // ============ Content Attributes ============
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }
    
    [JsonPropertyName("value")]
    public string? Value { get; set; }
    
    // ============ Accessibility Attributes ============
    
    [JsonPropertyName("dataTest")]
    public string? DataTest { get; set; }  // data-testid attribute
    
    [JsonPropertyName("ariaLabel")]
    public string? AriaLabel { get; set; }
    
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("alt")]
    public string? Alt { get; set; }
    
    // ============ Visual Attributes ============
    
    [JsonPropertyName("className")]
    public string? ClassName { get; set; }
    
    [JsonPropertyName("href")]
    public string? Href { get; set; }  // For links
    
    [JsonPropertyName("src")]
    public string? Src { get; set; }  // For images
    
    // ============ Healing Data ============
    
    [JsonPropertyName("fingerprint")]
    public FingerprintData? Fingerprint { get; set; }
    
    [JsonPropertyName("isHealed")]
    public bool IsHealed { get; set; }
}

/// <summary>
/// Contextual metadata for self-healing - the "Fingerprint DNA"
/// </summary>
public class FingerprintData
{
    [JsonPropertyName("attributes")]
    public FingerprintAttributes? Attributes { get; set; }
    
    [JsonPropertyName("context")]
    public FingerprintContext? Context { get; set; }
}

/// <summary>
/// Element attributes used for healing
/// </summary>
public class FingerprintAttributes
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("role")]
    public string? Role { get; set; }
    
    [JsonPropertyName("ariaLabel")]
    public string? AriaLabel { get; set; }
    
    [JsonPropertyName("classList")]
    public string? ClassList { get; set; }
}

/// <summary>
/// Contextual information surrounding the element
/// </summary>
public class FingerprintContext
{
    [JsonPropertyName("nearbyText")]
    public string? NearbyText { get; set; }
    
    [JsonPropertyName("parentTag")]
    public string? ParentTag { get; set; }
    
    [JsonPropertyName("heading")]
    public string? Heading { get; set; }
}
