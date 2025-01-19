using System;

namespace TradingEngineServer.Orders
{
    // Specifies reasons why an order might be rejected
    public enum RejectionReason
    {
        Unknown,            // The rejection reason is unspecified
        OrderNotFound,      // The specified order was not found
        InstrumentNotFound, // The specified instrument does not exist
        ModifyWrongSide,    // Modification attempted on the wrong side (buy/sell mismatch)
    }
}
