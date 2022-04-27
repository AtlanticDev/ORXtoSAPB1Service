using NASRx.Infra.Abstractions;
using NASRx.Utilities;
using System;
using System.Diagnostics;

namespace NASRx.Infra.Concretes
{
    public class Logging : ILogging
    {
        public void LogError(string message)
        {
            try
            {
                if (!EventLog.SourceExists(NASRxSettings.Instance.EventSource))
                    return;
                var eventLog = new EventLog { Source = NASRxSettings.Instance.EventSource };
                eventLog.WriteEntry($"{Environment.NewLine}{Environment.NewLine}{message}", EventLogEntryType.Error, 0);
            }
            catch { }
        }

        public void LogError(Guid exceptionId)
        {
            try { LogError($"System Error #{exceptionId}"); }
            catch { }
        }

        public void LogError(Exception exception)
            => LogError(exception, string.Empty);

        public void LogError(Exception exception, Guid exceptionId)
            => LogError(exception, $"System Error #{exceptionId}");

        public void LogError(Exception exception, string exceptionDesc)
        {
            try
            {
                var message = exception != null ? exception.ToString() : "Unknown error";
                message += $"{Environment.NewLine}{Environment.NewLine}{exceptionDesc}";
                LogError(message);
            }
            catch { }
        }

        public void LogInformation(string message)
        {
            try
            {
                if (!EventLog.SourceExists(NASRxSettings.Instance.EventSource))
                    return;
                var eventLog = new EventLog { Source = NASRxSettings.Instance.EventSource };
                eventLog.WriteEntry($"{Environment.NewLine}{Environment.NewLine}{message}", EventLogEntryType.Information, 2);
            }
            catch { }
        }

        public void LogWarning(string message)
        {
            try
            {
                if (!EventLog.SourceExists(NASRxSettings.Instance.EventSource))
                    return;
                var eventLog = new EventLog { Source = NASRxSettings.Instance.EventSource };
                eventLog.WriteEntry($"{Environment.NewLine}{Environment.NewLine}{message}", EventLogEntryType.Warning, 1);
            }
            catch { }
        }
    }
}