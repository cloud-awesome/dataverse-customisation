using CloudAwesome.Dataverse.Core.Loggers;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Xrm.Core.Loggers;
using Microsoft.Extensions.Logging;

namespace CloudAwesome.Dataverse.Core;

public class TracingHelper
{
	private readonly ILogger _logger;
        private readonly Dictionary<LoggingConfigurationType, ILogger> _loggers = new Dictionary<LoggingConfigurationType, ILogger>();

        public string LoggerImplementationType => _logger.GetType().Name;

        /// <summary>
        /// Construct trace logging with one of the pre-rolled loggers, included in the manifest's configuration
        /// </summary>
        /// <param name="loggingConfiguration">Logging configuration with logger type and any required parameters for the type</param>
        public TracingHelper(LoggingConfiguration loggingConfiguration)
        {
            _loggers.Add(LoggingConfigurationType.Console, new ConsoleLogger(loggingConfiguration.LogLevelToTrace));
            _loggers.Add(LoggingConfigurationType.ApplicationInsights, new AppInsightsLogger(loggingConfiguration.LogLevelToTrace, loggingConfiguration.ApplicationInsightsConnectionString));
            _loggers.Add(LoggingConfigurationType.TextFile, new TextFileLogger(loggingConfiguration.LogLevelToTrace, loggingConfiguration.TextFileOutputPath));

            _logger = _loggers[loggingConfiguration.LoggerConfigurationType];
        }

        /// <summary>
        /// Construct trace logging with a custom ILogger implementation, not one of those provided with the .Core library
        /// </summary>
        /// <param name="logger">An ILogger implementation. If null, all logs are ignored</param>
        public TracingHelper(ILogger logger)
        {
            if (logger != null)
            {
                _logger = logger;
            }
        }
        
        /// <summary>
        /// Used to silently throw away any logs
        /// If no traces/logs are required for whatever reason in the consuming application,
        /// construct an empty TracingHelper where one is required 
        /// </summary>
        public TracingHelper() { }

        /// <summary>
        /// Register a log entry
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        public void Log(LogLevel logLevel, string message)
        {
            _logger?.Log(logLevel, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Debug
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Information
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Critical
        /// </summary>
        /// <param name="message"></param>
        public void Critical(string message)
        {
            Log(LogLevel.Critical, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Error
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Trace
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        /// <summary>
        /// Register a log entry with LogLevel of Warning
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }
}