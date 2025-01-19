using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using TradingEngineServer.Logging.LoggingConfiguration;

namespace TradingEngineServer.Logging
{
    public class TextLogger : AbstractLogger, ITextLogger
    {
        private readonly LoggerConfiguration _loggingConfig;

        public TextLogger(IOptions<LoggerConfiguration> loggingConfig) : base()
        {
            // Retrieve and validate logger configuration
            _loggingConfig = loggingConfig.Value ?? throw new ArgumentNullException(nameof(loggingConfig));

            if (_loggingConfig.LoggerType != LoggerType.Text)
                throw new InvalidOperationException($"{nameof(TextLogger)} doesn't match LoggerType of {_loggingConfig.LoggerType}");

            var now = DateTime.Now;

            // Construct log file directory and name based on current timestamp
            string logDirectory = Path.Combine(_loggingConfig.TextLoggerConfiguration.Directory, $"{now:yyyy-MM-dd}");
            string uniqueLogName = $"{_loggingConfig.TextLoggerConfiguration.FileName}-{now:HH_mm_ss}";
            string baseLogName = Path.ChangeExtension(uniqueLogName, _loggingConfig.TextLoggerConfiguration.FileExtension);
            string filePath = Path.Combine(logDirectory, baseLogName);

            // Ensure the log directory exists
            Directory.CreateDirectory(logDirectory);

            // Start asynchronous logging task
            _ = Task.Run(() => LogAsync(filePath, _logQueue, _tokenSource.Token));
        }

        // Processes log entries asynchronously and writes them to a file
        private static async Task LogAsync(string filePath, BufferBlock<LogInformation> logQueue, CancellationToken token)
        {
            using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs) { AutoFlush = true };

            try
            {
                while (true)
                {
                    var logItem = await logQueue.ReceiveAsync(token).ConfigureAwait(false);
                    string formattedMessage = FormatLogItem(logItem);
                    await sw.WriteAsync(formattedMessage).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful exit when cancellation is requested
            }
        }

        // Formats a log entry into a string
        private static string FormatLogItem(LogInformation logItem)
        {
            return $"[{logItem.now:yyyy-MM-dd HH-mm-ss.fffffff}] [{logItem.threadName,-30}:{logItem.threadID:000}] " +
                   $"[{logItem.logLevel}] {logItem.message}";
        }

        // Enqueues a log entry for processing
        protected override void Log(LogLevel logLevel, string module, string message)
        {
            _logQueue.Post(new LogInformation(logLevel, module, message, DateTime.Now,
                Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
        }

        // Destructor to ensure cleanup
        ~TextLogger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_disposed)
                    return;

                _disposed = true;
            }

            if (disposing)
            {
                // Release managed resources
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            // Release unmanaged resources (if any)
        }

        // Queue for log entries
        private readonly BufferBlock<LogInformation> _logQueue = new BufferBlock<LogInformation>(); // Asynchronous and thread-safe queue
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly object _lock = new object();
        private bool _disposed = false;
    }
}
