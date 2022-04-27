using System;

namespace NASRx.Infra.Abstractions
{
    public interface ILogging
    {
        void LogError(string message);

        void LogError(Guid exceptionId);

        void LogError(Exception exception);

        void LogError(Exception exception, Guid exceptionId);

        void LogError(Exception exception, string exceptionDesc);

        void LogInformation(string message);

        void LogWarning(string message);
    }
}