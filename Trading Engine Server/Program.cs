using System;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingEngineServer.Core;

// Top-level statement
using var engine = TradingEngineServerHostBuilder.BuildTradingEnginServer();
TradingEngineServerServiceProvider.ServiceProvider = engine.Services;
{
    using var scope = TradingEngineServerServiceProvider.ServiceProvider.CreateScope();
    await engine.RunAsync().ConfigureAwait(false);
}