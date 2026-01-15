namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Resolves environment-specific URLs from Gherkin steps
/// Reads base URL from environment configuration
/// </summary>
public static class UrlResolver
{
    private static string? _baseUrl;
    private static readonly object _lock = new object();

    /// <summary>
    /// Set the base URL for the current environment
    /// </summary>
    public static void SetBaseUrl(string baseUrl)
    {
        lock (_lock)
        {
            _baseUrl = baseUrl;
        }
    }

    /// <summary>
    /// Get the configured base URL
    /// </summary>
    public static string GetBaseUrl()
    {
        lock (_lock)
        {
            return _baseUrl ?? "https://localhost";
        }
    }

    /// <summary>
    /// Resolve URL from Gherkin step or action value
    /// </summary>
    /// <param name="urlFragment">URL fragment from JSON or Gherkin</param>
    /// <param name="gherkinStep">Original Gherkin step (optional)</param>
    /// <returns>Complete URL</returns>
    public static string ResolveUrl(string urlFragment, string? gherkinStep = null)
    {
        // If it's already a full URL, return as is
        if (IsFullUrl(urlFragment))
        {
            return urlFragment;
        }

        // If gherkinStep is provided, try to extract URL from it
        if (!string.IsNullOrWhiteSpace(gherkinStep))
        {
            var extractedUrl = ExtractUrlFromGherkin(gherkinStep);
            if (!string.IsNullOrWhiteSpace(extractedUrl) && IsFullUrl(extractedUrl))
            {
                return extractedUrl;
            }
        }

        // Otherwise, append to base URL
        var baseUrl = GetBaseUrl().TrimEnd('/');
        var fragment = urlFragment.TrimStart('/');
        
        return $"{baseUrl}/{fragment}";
    }

    /// <summary>
    /// Check if the URL is a full URL (starts with http:// or https://)
    /// </summary>
    private static bool IsFullUrl(string url)
    {
        return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extract URL from Gherkin step (looks for quoted values starting with http)
    /// </summary>
    private static string? ExtractUrlFromGherkin(string gherkinStep)
    {
        var parameters = ParameterExtractor.ExtractAllParameters(gherkinStep);
        return parameters.FirstOrDefault(p => IsFullUrl(p));
    }
}
