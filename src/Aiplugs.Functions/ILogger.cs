using System;

namespace Aiplugs.Functions
{
    public interface ILogger : IDisposable
    {
        void Log(object message);
        void LogInfo(string message);
        void LogError(string message);
        void LogFail(Exception ex, string message);

        void Flush();
    }
}