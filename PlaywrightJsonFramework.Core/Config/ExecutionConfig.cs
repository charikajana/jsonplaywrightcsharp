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
}
