#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Plisky.Diagnostics;

namespace Plisky.Diagnostics;

/// <summary>
/// Provides an ILogger interface around an existing instance of Bilge, this allows Bilge to be used as a logger that can be passed into areas of the code that depend on ILogger implementations.
/// </summary>
public class ILoggerWrapper : ILogger {
    private readonly Bilge b;

    /// <summary>
    /// Wraps the passed instance of Bilge in an ILogger so that it can be used for compatibility with systemst hat require ILogger.
    /// </summary>
    /// <param name="b">The instance to use as the ILogger impplementation</param>
    public ILoggerWrapper(Bilge b) {
        this.b = b;
    }

    /// <summary>
    /// Converts a new LogLevel to a source level and returns true if the converted level is active relative to the log level.
    /// </summary>
    /// <param name="activeTraceLevel">The source level to use as a base</param>
    /// <param name="logLevel">The LogLevel to compare against</param>
    /// <returns>True if logging should occur.</returns>
    /// <exception cref="NotImplementedException">Thrown when there is no match made on the logLevel.</exception>
    public static bool LogLevelSourceLevelMapper(SourceLevels activeTraceLevel, LogLevel logLevel) {
        switch (logLevel) {
            case LogLevel.Trace:
                return activeTraceLevel >= SourceLevels.Information;

            case LogLevel.Debug:
                return activeTraceLevel >= SourceLevels.Verbose;
            case LogLevel.Information:
                return activeTraceLevel >= SourceLevels.Information;

            case LogLevel.Warning:
                return activeTraceLevel >= SourceLevels.Warning;
            case LogLevel.Error:
                return activeTraceLevel >= SourceLevels.Error;
            case LogLevel.Critical:
                return activeTraceLevel >= SourceLevels.Critical;

            case LogLevel.None:
                return false;

            default: throw new NotImplementedException($"Missing Level in Mapping to Source Levels {logLevel}");
        }
    }

    /// <summary>
    /// ILogger implementation of begin scope.
    /// </summary>
    /// <typeparam name="TState">State to log.</typeparam>
    /// <param name="state">The current state</param>
    /// <returns>A scope that is disposable</returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull {
        b.Info.Log($"BeginScope {state}");
        return new BilgeContextItem(state.ToString());
    }

    /// <summary>
    /// Returns true if the logging is enabled for a given level.
    /// </summary>
    /// <param name="logLevel">The level to check against</param>
    /// <returns>True if logging is enabled</returns>
    public bool IsEnabled(LogLevel logLevel) {

        return LogLevelSourceLevelMapper(b.ActiveTraceLevel, logLevel);

    }

    /// <summary>
    /// ILogger Implementation of log.
    /// </summary>
    /// <typeparam name="TState">State to log</typeparam>
    /// <param name="logLevel">The log level to write</param>
    /// <param name="eventId">An Event Id</param>
    /// <param name="state">State</param>
    /// <param name="exception">Error</param>
    /// <param name="formatter">A formatter</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
        switch (logLevel) {

            case LogLevel.Information:
                b.Info.Log(formatter(state, exception), $"{eventId.Name} {eventId.Id}");
                break;
            case LogLevel.Trace:
            case LogLevel.Debug:
                b.Verbose.Log(formatter(state, exception), $"{eventId.Name} {eventId.Id}");
                break;
            case LogLevel.Warning:
                b.Warning.Log(formatter(state, exception), $"{eventId.Name} {eventId.Id}");
                break;
            case LogLevel.Error:
                b.Error.Log(formatter(state, exception), $"{eventId.Name} {eventId.Id}");
                break;
            case LogLevel.Critical:
                b.Critical.Log(formatter(state, exception), $"{eventId.Name} {eventId.Id}");
                break;
            case LogLevel.None:
                break;
            default:
                break;
        }
    }
}
#endif