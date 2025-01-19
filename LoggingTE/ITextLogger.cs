using System;

namespace TradingEngineServer.Logging
{
    // Represents a logger that writes text-based logs and supports IDisposable for cleanup
    public interface ITextLogger : ILogger, IDisposable
    {
    }
}
