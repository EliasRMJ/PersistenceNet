using Microsoft.Extensions.Logging;

namespace PersistenceNet.CrossCutting.Logging
{
    public class FileLoggerProvider(string logDirectory) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(logDirectory);
        }

        public void Dispose() { }
    }
}