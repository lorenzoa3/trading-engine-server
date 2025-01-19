using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingEngineServer.Core;

// Top-level Statement

// Build the host using the host builder
using var engine = TradingEngineServerHostBuilder.BuildTradingEnginServer();

// Set the globally accessible service provider
TradingEngineServerServiceProvider.ServiceProvider = engine.Services;

// Create a service scope and run the engine
{
    using var scope = TradingEngineServerServiceProvider.ServiceProvider.CreateScope();
    await engine.RunAsync().ConfigureAwait(false); // Run the application asynchronously
}
