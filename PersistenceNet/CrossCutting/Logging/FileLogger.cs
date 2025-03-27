using Microsoft.Extensions.Logging;

namespace PersistenceNet.CrossCutting.Logging
{
    public class FileLogger(string logDirectory) : ILogger
    {
        private static readonly Lock _lock = new();

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception
            , Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}{Environment.NewLine}";
            var currentDirectory = Directory.GetCurrentDirectory();
            var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            var logDirectory = Path.Combine(parentDirectory!, "Logs");
            var logFilePath = Path.Combine(logDirectory, $"log-{DateTime.Now:yyyy-MM-dd}.txt");

            Directory.CreateDirectory(logDirectory);

            lock (_lock)
            {
                File.AppendAllText(logFilePath, logMessage);
            }
        }
    }
}