using System;

namespace NASRx.Infra.Abstractions
{
    public interface ILogging
    {
        void LogDebug(string message);

        void LogError(string message);

        void LogError(Exception exception);

        void LogInformation(string message);

        void LogWarning(string message);
    }
}