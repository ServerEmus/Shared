using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Globalization;

namespace ServerEmus.Shared;

/// <summary>
/// Logger to make logging easy.
/// </summary>
public static class MainLogger
{
    /// <summary>
    /// Gets or sets the name of the file for the File Logging.
    /// </summary>
    public static string FileName { get; set; } = "logs.txt";

    /// <summary>
    /// Gets or sets the template of both logging type.
    /// </summary>
    public static string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Gets the switch for all logging level.
    /// </summary>
    public static LoggingLevelSwitch LevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Gets the switch only for Console logging level.
    /// </summary>
    public static LoggingLevelSwitch ConsoleLevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Gets the switch only for File logging level.
    /// </summary>
    public static LoggingLevelSwitch FileLevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Creates and initialize logger.
    /// </summary>
    public static void CreateNew()
    {
        var ilogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.File(FileName, outputTemplate: OutputTemplate, levelSwitch: FileLevelSwitch, formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.Console(outputTemplate: OutputTemplate, levelSwitch: ConsoleLevelSwitch, formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
        Log.Logger = ilogger;
    }

    /// <summary>
    /// Close the Logger.
    /// </summary>
    public static void Close()
    {
        Log.CloseAndFlush();
        Log.Logger = Logger.None;
    }
}
