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
            // Want to make sure these aren't null ?? == Null Coalescing Operator
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
            _tradingEngineServerConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
        }
        // For Completion
        public Task Run(CancellationToken token) => ExecuteAsync(token);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info(nameof(TradingEngineServer), "Starting Trading Engine");
            while (!stoppingToken.IsCancellationRequested)
            {

            }
            _logger.Info(nameof(TradingEngineServer), "Stopped Trading Engine");

            return Task.CompletedTask;
        }
    }
}
