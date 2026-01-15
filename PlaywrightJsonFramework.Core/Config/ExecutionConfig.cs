namespace PlaywrightJsonFramework.Core.Config;

/// <summary>
/// Runtime execution settings
/// </summary>
public static class ExecutionConfig
{
    private static string _browserName = "chromium";
    private static bool _headless = false;
    private static int _slowMotion = 0;
    private static int _defaultTimeout = 30000;
    private static string _environment = "dev";

    /// <summary>
    /// Browser name: chromium, firefox, webkit, chrome
    /// </summary>
    public static string BrowserName
    {
        get => System.Environment.GetEnvironmentVariable("BROWSER") ?? _browserName;
        set => _browserName = value;
    }

    /// <summary>
    /// Run browser in headless mode
    /// </summary>
    public static bool IsHeadless
    {
        get
        {
            var envValue = System.Environment.GetEnvironmentVariable("HEADLESS");
            if (bool.TryParse(envValue, out bool result))
                return result;
            return _headless;
        }
        set => _headless = value;
    }

    /// <summary>
    /// Slow motion delay in milliseconds
    /// </summary>
    public static int SlowMotion
    {
        get
        {
            var envValue = System.Environment.GetEnvironmentVariable("SLOW_MO");
            if (int.TryParse(envValue, out int result))
                return result;
            return _slowMotion;
        }
        set => _slowMotion = value;
    }

    /// <summary>
    /// Default timeout in milliseconds
    /// </summary>
    public static int DefaultTimeout
    {
        get
        {
            var envValue = System.Environment.GetEnvironmentVariable("DEFAULT_TIMEOUT");
            if (int.TryParse(envValue, out int result))
                return result;
            return _defaultTimeout;
        }
        set => _defaultTimeout = value;
    }

    /// <summary>
    /// Environment name: dev, staging, prod
    /// </summary>
    public static string Environment
    {
        get => System.Environment.GetEnvironmentVariable("TEST_ENV") ?? _environment;
        set => _environment = value;
    }

    #region -- SMTP Config (CI/CD Notifications) --

    public static string SmtpHost => System.Environment.GetEnvironmentVariable("SMTP_HOST") ?? string.Empty;
    public static int SmtpPort => int.TryParse(System.Environment.GetEnvironmentVariable("SMTP_PORT"), out int port) ? port : 587;
    public static string SmtpUser => System.Environment.GetEnvironmentVariable("SMTP_USER") ?? string.Empty;
    public static string SmtpPass => System.Environment.GetEnvironmentVariable("SMTP_PASS") ?? string.Empty;
    public static string RecipientEmail => System.Environment.GetEnvironmentVariable("RECIPIENT_EMAIL") ?? string.Empty;
    public static bool EnableEmail => bool.TryParse(System.Environment.GetEnvironmentVariable("ENABLE_EMAIL"), out bool enable) && enable;

    public static string BuildNumber => System.Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER") 
        ?? System.Environment.GetEnvironmentVariable("BUILD_NUMBER") 
        ?? "LOCAL_RUN";

    public static string ResultsUrl => System.Environment.GetEnvironmentVariable("RESULTS_URL") ?? string.Empty;

    #endregion
}
