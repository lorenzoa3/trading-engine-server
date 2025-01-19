using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Logging
{
    // Represents structured information for a single log entry
    public record LogInformation(LogLevel logLevel, string module, string message, DateTime now, int threadID, string threadName);
}

// Enables the use of 'record' types in .NET Standard or frameworks that don't natively support it
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { };
}