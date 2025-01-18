using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Logging
{
    public record LogInformation(LogLevel logLevel, string module, string message, DateTime now, int threadID, string threadName);
}


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { };
}