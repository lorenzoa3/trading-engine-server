using System.Collections.Generic;
using System;
using TradingEngineServer.Instrument;
using System.Linq;

public static class InstrumentRepository
{
    public static readonly List<Security> Instruments = new List<Security>
    {
        new Security(1, "AAPL", "Apple Inc."),
        new Security(2, "GOOG", "Alphabet Inc."),
        new Security(3, "TSLA", "Tesla Inc.")
    };

    public static Security GetSecurityById(int securityId)
    {
        return Instruments.FirstOrDefault(i => i.SecurityId == securityId)
               ?? throw new ArgumentException($"Security with ID {securityId} not found.");
    }
}
