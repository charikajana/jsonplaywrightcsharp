using System.Text.RegularExpressions;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Extracts quoted parameters from Gherkin step text
/// Example: "User enters username 'admin' and password 'Test@123'"
/// </summary>
public static class ParameterExtractor
{
    /// <summary>
    /// Extract parameter at the specified index from Gherkin step
    /// </summary>
    /// <param name="gherkinStep">The complete Gherkin step text</param>
    /// <param name="index">Zero-based index of the parameter to extract</param>
    /// <returns>The extracted parameter value, or null if not found</returns>
    public static string? ExtractParameter(string gherkinStep, int index)
    {
        if (string.IsNullOrWhiteSpace(gherkinStep))
            return null;

        // Match both single and double quoted strings
        var pattern = @"""([^""]*)""|'([^']*)'";
        var matches = Regex.Matches(gherkinStep, pattern);

        if (index < 0 || index >= matches.Count)
            return null;

        // Return the captured group (either from double or single quotes)
        var match = matches[index];
        return match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
    }

    /// <summary>
    /// Extract all parameters from Gherkin step
    /// </summary>
    public static List<string> ExtractAllParameters(string gherkinStep)
    {
        var parameters = new List<string>();
        
        if (string.IsNullOrWhiteSpace(gherkinStep))
            return parameters;

        var pattern = @"""([^""]*)""|'([^']*)'";
        var matches = Regex.Matches(gherkinStep, pattern);

        foreach (Match match in matches)
        {
            var value = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            parameters.Add(value);
        }

        return parameters;
    }

    /// <summary>
    /// Check if Gherkin step contains runtime parameters (quoted values)
    /// </summary>
    public static bool HasParameters(string gherkinStep)
    {
        if (string.IsNullOrWhiteSpace(gherkinStep))
            return false;

        var pattern = @"""([^""]*)""|'([^']*)'";
        return Regex.IsMatch(gherkinStep, pattern);
    }

    /// <summary>
    /// Count the number of parameters in Gherkin step
    /// </summary>
    public static int CountParameters(string gherkinStep)
    {
        return ExtractAllParameters(gherkinStep).Count;
    }
}
