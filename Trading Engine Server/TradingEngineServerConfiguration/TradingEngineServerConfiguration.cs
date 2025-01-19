using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Core.Configuration
{
    // Configuration class for the server
    internal class TradingEngineServerConfiguration
    {
        // Holds the server-specific settings, such as the port number
        public TradingEngineServerSettings TradingEngineServerSettings { get; set; }
    }

    // Represents the settings for the trading engine server
    class TradingEngineServerSettings
    {
        // Port number on which the server will listen
        public int Port { get; set; }
    }
}
