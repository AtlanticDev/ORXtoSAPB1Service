using NASRx.Infra.Abstractions;
using NLog;
using System;

namespace NASRx.Infra.Concretes
{
    public class Logging : ILogging
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void DisplayMessage(string message)
        {
            if (Environment.UserInteractive)
                Console.WriteLine($"{DateTime.Now}: {message}");
        }

        public void LogError(string message)
        {
            DisplayMessage(message);
            _logger.Error(message);
        }

        public void LogError(Exception exception)
            => LogError(exception?.Message);

        public void LogDebug(string message)
        {
            DisplayMessage(message);
            _logger.Debug(message);
        }

        public void LogInformation(string message)
        {
            DisplayMessage(message);
            _logger.Info(message);
        }

        public void LogWarning(string message)
        {
            DisplayMessage(message);
            _logger.Warn(message);
        }
    }
}