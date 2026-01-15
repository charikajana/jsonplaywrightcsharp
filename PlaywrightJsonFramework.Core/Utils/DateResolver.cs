using System.Globalization;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Converts relative date keywords to actual dates
/// Supports: TODAY, TOMORROW, YESTERDAY, NEXT_WEEK, etc.
/// </summary>
public static class DateResolver
{
    private const string DefaultDateFormat = "dd-MMM-yyyy";

    /// <summary>
    /// Resolve date from keyword or formatted string
    /// </summary>
    public static string Resolve(string dateValue, string? format = null)
    {
        if (string.IsNullOrWhiteSpace(dateValue))
            return dateValue;

        var outputFormat = format ?? DefaultDateFormat;
        var upperValue = dateValue.Trim().ToUpperInvariant();

        return upperValue switch
        {
            "TODAY" => DateTime.Now.ToString(outputFormat, CultureInfo.InvariantCulture),
            "TOMORROW" => DateTime.Now.AddDays(1).ToString(outputFormat, CultureInfo.InvariantCulture),
            "YESTERDAY" => DateTime.Now.AddDays(-1).ToString(outputFormat, CultureInfo.InvariantCulture),
            "NEXT_WEEK" => DateTime.Now.AddDays(7).ToString(outputFormat, CultureInfo.InvariantCulture),
            "LAST_WEEK" => DateTime.Now.AddDays(-7).ToString(outputFormat, CultureInfo.InvariantCulture),
            "NEXT_MONTH" => DateTime.Now.AddMonths(1).ToString(outputFormat, CultureInfo.InvariantCulture),
            "LAST_MONTH" => DateTime.Now.AddMonths(-1).ToString(outputFormat, CultureInfo.InvariantCulture),
            "NEXT_YEAR" => DateTime.Now.AddYears(1).ToString(outputFormat, CultureInfo.InvariantCulture),
            "LAST_YEAR" => DateTime.Now.AddYears(-1).ToString(outputFormat, CultureInfo.InvariantCulture),
            _ => ResolveFromPattern(upperValue, dateValue, outputFormat)
        };
    }

    /// <summary>
    /// Resolve date from pattern like "TODAY+5" or "TODAY-3"
    /// </summary>
    private static string ResolveFromPattern(string upperValue, string originalValue, string outputFormat)
    {
        // Check for patterns like "TODAY+5" or "TOMORROW-2"
        if (upperValue.StartsWith("TODAY+"))
        {
            if (int.TryParse(upperValue.Replace("TODAY+", ""), out int days))
                return DateTime.Now.AddDays(days).ToString(outputFormat, CultureInfo.InvariantCulture);
        }
        else if (upperValue.StartsWith("TODAY-"))
        {
            if (int.TryParse(upperValue.Replace("TODAY-", ""), out int days))
                return DateTime.Now.AddDays(-days).ToString(outputFormat, CultureInfo.InvariantCulture);
        }

        // If not a keyword, try parsing as is
        if (DateTime.TryParse(originalValue, out DateTime parsedDate))
        {
            return parsedDate.ToString(outputFormat, CultureInfo.InvariantCulture);
        }

        // Return original value if no match
        return originalValue;
    }

    /// <summary>
    /// Check if value contains a date keyword
    /// </summary>
    public static bool IsDateKeyword(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var upperValue = value.Trim().ToUpperInvariant();
        string[] keywords = { "TODAY", "TOMORROW", "YESTERDAY", "NEXT_WEEK", "LAST_WEEK", 
                             "NEXT_MONTH", "LAST_MONTH", "NEXT_YEAR", "LAST_YEAR" };

        return keywords.Any(k => upperValue.StartsWith(k));
    }
}
