using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingEngineServer.Core
{
    // Provides a globally accessible service provider for dependency injection
    public static class TradingEngineServerServiceProvider
    {
        // Static property to store the service provider instance
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
