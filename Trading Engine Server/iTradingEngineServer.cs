using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Core
{
    internal interface iTradingEngineServer
    {
        // Runs the trading engine server asynchronously.
        Task Run(CancellationToken token);
    }
}
