using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Playwright;

/// <summary>
/// Singleton browser lifecycle management
/// Handles browser initialization, page management, and cleanup
/// </summary>
public class PlaywrightManager
{
    private static PlaywrightManager? _instance;
    private static readonly object _lock = new object();
    
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    private const string COMPONENT = "PLAYWRIGHT";

    private PlaywrightManager()
    {
    }

    /// <summary>
    /// Get the singleton instance
    /// </summary>
    public static PlaywrightManager Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new PlaywrightManager();
            }
        }
    }

    /// <summary>
    /// Get the current page instance
    /// </summary>
    public IPage? Page => _page;

    /// <summary>
    /// Get the browser context
    /// </summary>
    public IBrowserContext? Context => _context;

    /// <summary>
    /// Initialize browser with configuration
    /// </summary>
    public async Task InitializeBrowser(
        string browserName = "chromium",
        bool headless = false,
        int slowMo = 0)
    {
        try
        {
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            // Select browser type
            IBrowserType browserType = browserName.ToLowerInvariant() switch
            {
                "firefox" => _playwright.Firefox,
                "webkit" => _playwright.Webkit,
                "chrome" => _playwright.Chromium,
                _ => _playwright.Chromium
            };

            // Launch options
            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = headless,
                SlowMo = slowMo,
                Channel = browserName.ToLowerInvariant() == "chrome" ? "chrome" : null
            };

            _browser = await browserType.LaunchAsync(launchOptions);

            // Context options
            var contextOptions = new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                RecordVideoDir = Path.Combine(Directory.GetCurrentDirectory(), "reports", "videos"),
                RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 }
            };

            _context = await _browser.NewContextAsync(contextOptions);

            // Set default timeout
            _context.SetDefaultTimeout(30000);
            _context.SetDefaultNavigationTimeout(30000);

            _page = await _context.NewPageAsync();

            Logger.Info($"Browser initialized: {browserName}, Headless: {headless}", COMPONENT);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error initializing browser: {ex.Message}", COMPONENT);
            throw;
        }
    }

    /// <summary>
    /// Close browser and cleanup resources
    /// </summary>
    public async Task CloseBrowser()
    {
        try
        {
            if (_page != null)
            {
                await _page.CloseAsync();
                _page = null;
            }

            if (_context != null)
            {
                await _context.CloseAsync();
                _context = null;
            }

            if (_browser != null)
            {
                await _browser.CloseAsync();
                _browser = null;
            }

            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }

            Logger.Success("Browser closed successfully", COMPONENT);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error closing browser: {ex.Message}", COMPONENT);
        }
    }

    /// <summary>
    /// Navigate to URL
    /// </summary>
    public async Task NavigateTo(string url)
    {
        if (_page == null)
            throw new InvalidOperationException("Browser not initialized. Call InitializeBrowser() first.");

        await _page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load });
        Logger.Info($"Navigated to: {url}", COMPONENT);
    }

    /// <summary>
    /// Take screenshot
    /// </summary>
    public async Task<byte[]?> TakeScreenshot(string? path = null)
    {
        if (_page == null)
            return null;

        var screenshotOptions = new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        };

        return await _page.ScreenshotAsync(screenshotOptions);
    }
}
