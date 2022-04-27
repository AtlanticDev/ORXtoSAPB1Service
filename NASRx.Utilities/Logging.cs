using NLog;
using System;

namespace NASRx.Utilities
{
    public static class Logging
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void DisplayMessage(string message)
        {
            if (Environment.UserInteractive)
                Console.WriteLine($"{DateTime.Now}: {message}");
        }

        public static void LogDebug(string message)
        {
            DisplayMessage(message);
            _logger.Debug(message);
        }

        public static void LogError(Exception exception)
        {
            DisplayMessage(exception.Message);
            _logger.Error(exception);
        }

        public static void LogError(string errorMessage)
        {
            DisplayMessage(errorMessage);
            _logger.Error(errorMessage);
        }

        public static void LogInfo(string message)
        {
            DisplayMessage(message);
            _logger.Info(message);
        }

        public static void LogTrace(string message)
        {
            DisplayMessage(message);
            _logger.Trace(message);
        }
    }
}