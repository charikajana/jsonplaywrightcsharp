using System.Text.Json.Serialization;

namespace PlaywrightJsonFramework.Core.Data;

/// <summary>
/// Model for JSON step structure - represents a complete Gherkin step with all actions
/// </summary>
public class StepData
{
    [JsonPropertyName("stepFileName")]
    public string StepFileName { get; set; } = string.Empty;
    
    [JsonPropertyName("gherkinStep")]
    public string GherkinStep { get; set; } = string.Empty;
    
    [JsonPropertyName("normalizedStep")]
    public string NormalizedStep { get; set; } = string.Empty;
    
    [JsonPropertyName("stepType")]
    public string? StepType { get; set; }  // Given/When/Then
    
    [JsonPropertyName("stepNumber")]
    public int StepNumber { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; }  // passed/failed
    
    [JsonPropertyName("actions")]
    public List<ActionData> Actions { get; set; } = new();
    
    [JsonPropertyName("metadata")]
    public StepMetadata? Metadata { get; set; }
}

/// <summary>
/// Metadata about the step
/// </summary>
public class StepMetadata
{
    [JsonPropertyName("createdDate")]
    public string? CreatedDate { get; set; }
    
    [JsonPropertyName("totalActions")]
    public int TotalActions { get; set; }
}
