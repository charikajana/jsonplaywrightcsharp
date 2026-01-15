using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Repository;

/// <summary>
/// Centralized CRUD operations for JSON step files
/// Handles file naming, loading, saving, and live attribute enrichment
/// </summary>
public class StepRepository
{
    private const string COMPONENT = "REPOSITORY";
    private static readonly string RepositoryPath = Path.Combine(
        Directory.GetCurrentDirectory(), 
        "LocatorRepository"
    );

    static StepRepository()
    {
        // Ensure repository directory exists
        if (!Directory.Exists(RepositoryPath))
        {
            Directory.CreateDirectory(RepositoryPath);
        }
    }

    /// <summary>
    /// Check if a JSON file exists for the given Gherkin step
    /// </summary>
    public static bool StepExists(string gherkinStep)
    {
        var fileName = GenerateStepFileName(gherkinStep);
        var filePath = Path.Combine(RepositoryPath, fileName);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Load StepData from JSON file
    /// </summary>
    public static StepData? GetStep(string gherkinStep)
    {
        var fileName = GenerateStepFileName(gherkinStep);
        var filePath = Path.Combine(RepositoryPath, fileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            return JsonSerializer.Deserialize<StepData>(jsonContent, options);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error loading step JSON: {ex.Message}", COMPONENT);
            return null;
        }
    }

    /// <summary>
    /// Save StepData to JSON file
    /// </summary>
    public static void SaveStep(StepData stepData)
    {
        var fileName = stepData.StepFileName.EndsWith(".json") 
            ? stepData.StepFileName 
            : $"{stepData.StepFileName}.json";
        
        var filePath = Path.Combine(RepositoryPath, fileName);

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            
            var jsonContent = JsonSerializer.Serialize(stepData, options);
            File.WriteAllText(filePath, jsonContent);
            
            Logger.Success($"Saved step JSON: {fileName}", COMPONENT);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error saving step JSON: {ex.Message}", COMPONENT);
        }
    }

    /// <summary>
    /// Get list of actions from JSON file
    /// </summary>
    public static List<ActionData>? GetActions(string gherkinStep)
    {
        var stepData = GetStep(gherkinStep);
        return stepData?.Actions;
    }

    /// <summary>
    /// Generate normalized filename from Gherkin step text
    /// Example: "User enters username 'admin'" → "user_enters_username_param.json"
    /// </summary>
    public static string GenerateStepFileName(string gherkinStep)
    {
        // Convert to lowercase
        var normalized = gherkinStep.ToLowerInvariant();

        // Replace quoted values with "_param_"
        normalized = Regex.Replace(normalized, @"""[^""]*""", "_param_");
        normalized = Regex.Replace(normalized, @"'[^']*'", "_param_");

        // Replace specific numbers with "_param_"
        normalized = Regex.Replace(normalized, @"\b\d+\b", "_param_");

        // Replace spaces and special characters with underscores
        normalized = Regex.Replace(normalized, @"[^\w]+", "_");

        // Remove multiple consecutive underscores
        normalized = Regex.Replace(normalized, @"_+", "_");

        // Remove leading/trailing underscores
        normalized = normalized.Trim('_');

        // Add .json extension
        return $"{normalized}.json";
    }

    /// <summary>
    /// Enrich JSON with 17 live attributes from the actual DOM element
    /// This is called after element is found to capture real-time data
    /// </summary>
    public static async Task PopulateLiveAttributes(ILocator locator, ElementLocators elementLocators)
    {
        try
        {
            // Use Playwright's evaluate to capture all 17 attributes
            var attributes = await locator.EvaluateAsync<Dictionary<string, string?>>(
                @"(el) => {
                    return {
                        type: el.tagName?.toLowerCase() || null,
                        id: el.id || null,
                        name: el.getAttribute('name') || null,
                        text: el.textContent?.trim() || null,
                        placeholder: el.placeholder || null,
                        value: el.value || null,
                        dataTest: el.getAttribute('data-testid') || el.getAttribute('data-test') || null,
                        ariaLabel: el.getAttribute('aria-label') || null,
                        role: el.getAttribute('role') || null,
                        title: el.title || null,
                        alt: el.alt || null,
                        className: el.className || null,
                        href: el.href || null,
                        src: el.src || null
                    };
                }"
            );

            // Update the elementLocators object
            elementLocators.Type = attributes.GetValueOrDefault("type");
            elementLocators.Id = attributes.GetValueOrDefault("id");
            elementLocators.Name = attributes.GetValueOrDefault("name");
            elementLocators.Text = attributes.GetValueOrDefault("text");
            elementLocators.Placeholder = attributes.GetValueOrDefault("placeholder");
            elementLocators.Value = attributes.GetValueOrDefault("value");
            elementLocators.DataTest = attributes.GetValueOrDefault("dataTest");
            elementLocators.AriaLabel = attributes.GetValueOrDefault("ariaLabel");
            elementLocators.Role = attributes.GetValueOrDefault("role");
            elementLocators.Title = attributes.GetValueOrDefault("title");
            elementLocators.Alt = attributes.GetValueOrDefault("alt");
            elementLocators.ClassName = attributes.GetValueOrDefault("className");
            elementLocators.Href = attributes.GetValueOrDefault("href");
            elementLocators.Src = attributes.GetValueOrDefault("src");

            Logger.Info("Live attributes populated successfully", COMPONENT);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Could not populate live attributes: {ex.Message}", COMPONENT);
        }
    }
}
