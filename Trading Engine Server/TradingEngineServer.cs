using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;

namespace TradingEngineServer.Core
{
    sealed class TradingEngineServer : BackgroundService, iTradingEngineServer
    {
        private readonly TradingEngineServerConfiguration _tradingEngineServerConfig;
        private readonly ITextLogger _logger;

        public TradingEngineServer(ITextLogger textLogger, IOptions<TradingEngineServerConfiguration> config)
        {
            // Ensure logger and configuration are not null
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
            _tradingEngineServerConfig = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        // Allows manual running of the server
        public Task Run(CancellationToken token) => ExecuteAsync(token);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Log server startup
            _logger.Info(nameof(TradingEngineServer), "Starting Trading Engine");

            // Main processing loop
            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO: Add core trading engine logic here
            }

            // Log server shutdown
            _logger.Info(nameof(TradingEngineServer), "Stopped Trading Engine");

            return Task.CompletedTask;
        }
    }
}
