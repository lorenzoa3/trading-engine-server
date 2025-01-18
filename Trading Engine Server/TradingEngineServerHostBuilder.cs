﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;
using TradingEngineServer.Logging.LoggingConfiguration;

namespace TradingEngineServer.Core
{
    public sealed class TradingEngineServerHostBuilder
    {
        public static IHost BuildTradingEnginServer()
            => Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                // Start configuration
                services.AddOptions();
                services.Configure<TradingEngineServerConfiguration>(context.Configuration.GetSection(nameof(TradingEngineServerConfiguration)));
                services.Configure<LoggerConfiguration>(context.Configuration.GetSection(nameof(LoggerConfiguration)));

                // Add singleton objects
                services.AddSingleton<iTradingEngineServer, TradingEngineServer>();
                services.AddSingleton<ITextLogger, TextLogger>();

                // Add hosted service
                services.AddHostedService<TradingEngineServer>();
            }).Build();
    }
}
