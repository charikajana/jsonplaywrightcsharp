using System.Diagnostics;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Logger utility for framework-wide logging
/// Provides structured logging with different levels
/// </summary>
public static class Logger
{
    private static readonly object _lock = new object();
    private static string? _logFilePath;
    private static bool _consoleEnabled = true;
    private static bool _fileEnabled = false;
    private static LogLevel _minLogLevel = LogLevel.INFO;

    public enum LogLevel
    {
        DEBUG = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3,
        SUCCESS = 4
    }

    /// <summary>
    /// Initialize logger with file logging
    /// </summary>
    public static void Initialize(string? logDirectory = null, bool enableFileLogging = false)
    {
        _fileEnabled = enableFileLogging;

        if (_fileEnabled)
        {
            var logDir = logDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine(logDir, $"test_execution_{timestamp}.log");

            Info($"Logger initialized. Log file: {_logFilePath}");
        }
    }

    /// <summary>
    /// Set minimum log level
    /// </summary>
    public static void SetLogLevel(LogLevel level)
    {
        _minLogLevel = level;
    }

    /// <summary>
    /// Enable/disable console output
    /// </summary>
    public static void SetConsoleOutput(bool enabled)
    {
        _consoleEnabled = enabled;
    }

    /// <summary>
    /// Log DEBUG message
    /// </summary>
    public static void Debug(string message, string? component = null)
    {
        Log(LogLevel.DEBUG, message, component);
    }

    /// <summary>
    /// Log INFO message
    /// </summary>
    public static void Info(string message, string? component = null)
    {
        Log(LogLevel.INFO, message, component);
    }

    /// <summary>
    /// Log WARNING message
    /// </summary>
    public static void Warn(string message, string? component = null)
    {
        Log(LogLevel.WARN, message, component);
    }

    /// <summary>
    /// Log ERROR message
    /// </summary>
    public static void Error(string message, string? component = null, Exception? exception = null)
    {
        var fullMessage = exception != null 
            ? $"{message}\nException: {exception.Message}\nStackTrace: {exception.StackTrace}"
            : message;
        Log(LogLevel.ERROR, fullMessage, component);
    }

    /// <summary>
    /// Log SUCCESS message
    /// </summary>
    public static void Success(string message, string? component = null)
    {
        Log(LogLevel.SUCCESS, message, component);
    }

    /// <summary>
    /// Log step start
    /// </summary>
    public static void Step(string stepText, string stepType = "STEP")
    {
        var separator = new string('=', 80);
        Info($"\n{separator}");
        Info($"[{stepType}] {stepText}");
        Info(separator);
    }

    /// <summary>
    /// Log action
    /// </summary>
    public static void Action(int actionNumber, string actionType, string description)
    {
        Info($"\nAction #{actionNumber}: {actionType} - {description}", "JSON EXECUTOR");
    }

    /// <summary>
    /// Log scenario start
    /// </summary>
    public static void ScenarioStart(string scenarioName)
    {
        var separator = new string('=', 80);
        Console.WriteLine($"\n{separator}");
        Console.WriteLine($"  STARTING SCENARIO: {scenarioName}");
        Console.WriteLine($"{separator}\n");
    }

    /// <summary>
    /// Log scenario end
    /// </summary>
    public static void ScenarioEnd(bool passed, string scenarioName)
    {
        var separator = new string('=', 80);
        var status = passed ? "PASSED" : "FAILED";
        Console.WriteLine($"\n{separator}");
        Console.WriteLine($"  SCENARIO COMPLETED: {status}");
        Console.WriteLine($"{separator}\n");
    }

    /// <summary>
    /// Core logging method
    /// </summary>
    private static void Log(LogLevel level, string message, string? component = null)
    {
        if (level < _minLogLevel)
            return;

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelStr = GetLevelString(level);
        var componentStr = component != null ? $"[{component}] " : "";
        var formattedMessage = $"{timestamp} {levelStr} {componentStr}{message}";

        lock (_lock)
        {
            // Console output with colors
            if (_consoleEnabled)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = GetLevelColor(level);
                Console.WriteLine(componentStr + message);
                Console.ForegroundColor = originalColor;
            }

            // File output
            if (_fileEnabled && _logFilePath != null)
            {
                try
                {
                    File.AppendAllText(_logFilePath, formattedMessage + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// Get level string representation
    /// </summary>
    private static string GetLevelString(LogLevel level)
    {
        return level switch
        {
            LogLevel.DEBUG => "[DEBUG]",
            LogLevel.INFO => "[INFO] ",
            LogLevel.WARN => "[WARN] ",
            LogLevel.ERROR => "[ERROR]",
            LogLevel.SUCCESS => "[SUCCESS]",
            _ => "[INFO] "
        };
    }

    /// <summary>
    /// Get console color for log level
    /// </summary>
    private static ConsoleColor GetLevelColor(LogLevel level)
    {
        return level switch
        {
            LogLevel.DEBUG => ConsoleColor.Gray,
            LogLevel.INFO => ConsoleColor.White,
            LogLevel.WARN => ConsoleColor.Yellow,
            LogLevel.ERROR => ConsoleColor.Red,
            LogLevel.SUCCESS => ConsoleColor.Green,
            _ => ConsoleColor.White
        };
    }

    /// <summary>
    /// Log with custom formatting
    /// </summary>
    public static void Custom(string message, ConsoleColor color = ConsoleColor.White)
    {
        if (_consoleEnabled)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }

    /// <summary>
    /// Log section separator
    /// </summary>
    public static void Separator(char character = '-', int length = 80)
    {
        Info(new string(character, length));
    }

    /// <summary>
    /// Log empty line
    /// </summary>
    public static void NewLine()
    {
        if (_consoleEnabled)
            Console.WriteLine();
    }

    /// <summary>
    /// Flush and close log file
    /// </summary>
    public static void Close()
    {
        if (_fileEnabled && _logFilePath != null)
        {
            Info("Logger closed");
        }
    }
}
