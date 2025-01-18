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
            _loggingConfig = loggingConfig.Value ?? throw new ArgumentNullException(nameof(loggingConfig));

            if (_loggingConfig.LoggerType != LoggerType.Text)
                throw new InvalidOperationException($"{nameof(TextLogger)} doesn't match LoggerType of {_loggingConfig.LoggerType}");

            var now = DateTime.Now;

            string logDirectory = Path.Combine(_loggingConfig.TextLoggerConfiguration.directory, $"{now:yyyy-MM-dd}");
            string uniqueLogName = $"{_loggingConfig.TextLoggerConfiguration.fileName}-{now:HH_mm_ss}";

            string baseLogName = Path.ChangeExtension(uniqueLogName, _loggingConfig.TextLoggerConfiguration.fileExtension);

            string filePath = Path.Combine(logDirectory, baseLogName);

            // In case the directory does not exist
            Directory.CreateDirectory(logDirectory);

            _ = Task.Run(() => LogAsync(filePath, _logQueue, _tokenSource.Token));
        }

        private static async Task LogAsync(string filePath, BufferBlock<LogInformation> logQueue, CancellationToken token)
        {
            // Need "using" to dispose of object at the end of scope
            using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs) { AutoFlush = true, };

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
            { }

        }
        // To format log entries
        private static string FormatLogItem(LogInformation logItem)
        {
            return $"[{logItem.now:yyyy-MM-dd HH-mm-ss.fffffff}] [{logItem.threadName, -30}:{logItem.threadID:000}] " +
                $"[{logItem.logLevel}] {logItem.message}";
        }

        protected override void Log(LogLevel logLevel, string module, string message)
        {
            _logQueue.Post(new LogInformation(logLevel, module, message, DateTime.Now, 
                Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
        }

        // Can think of this as a destructor (it's not a destructor though)
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
                // Get rid of resources managed
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            // Get rid of unmanaged resources

        }

        private readonly BufferBlock<LogInformation> _logQueue = new BufferBlock<LogInformation>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly object _lock = new object();
        private bool _disposed = false;
    }
}
