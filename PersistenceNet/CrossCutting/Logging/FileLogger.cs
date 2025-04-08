using Microsoft.Extensions.Logging;

namespace PersistenceNet.CrossCutting.Logging
{
#pragma warning disable CS9113 
    public class FileLogger(string logDirectory) : ILogger
#pragma warning restore CS9113
    {
        private static readonly Lock _lock = new();

#pragma warning disable CS8633 
        public IDisposable? BeginScope<TState>(TState state) => null;
#pragma warning restore CS8633 

        public bool IsEnabled(LogLevel logLevel) => true;

#pragma warning disable CS8767 
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception
#pragma warning restore CS8767 
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