using Serilog;
using Serilog.Events;
using Serilog.Core;

namespace Shared;

/// <summary>
/// Logger to make logging easy.
/// </summary>
public static class MainLogger
{
    /// <summary>
    /// Switch for all logging level.
    /// </summary>
    public static LoggingLevelSwitch LevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Switch only for Console logging level.
    /// </summary>
    public static LoggingLevelSwitch ConsoleLevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Switch only for File logging level.
    /// </summary>
    public static LoggingLevelSwitch FileLevelSwitch { get; } = new(LogEventLevel.Information);

    /// <summary>
    /// Creates and initialize logger.
    /// </summary>
    public static void CreateNew()
    {
        var Ilogger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(LevelSwitch)
            .WriteTo.File("logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", levelSwitch: FileLevelSwitch)
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", levelSwitch: ConsoleLevelSwitch)
            .CreateLogger();
        Ilogger.Information("Application started!");
        Log.Logger = Ilogger;
    }

    /// <summary>
    /// Close the Logger.
    /// </summary>
    public static void Close()
    {
        Log.Information("Application closed!");
        Log.CloseAndFlush();
        Log.Logger = Logger.None;
    }
}
