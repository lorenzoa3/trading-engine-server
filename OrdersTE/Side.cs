using System;

namespace TradingEngineServer.Orders
{
    // Represents the side of an order in the market
    public enum Side
    {
        Unknown, // Side is not specified or empty order
        Bid,     // Buy side of the order
        Ask,     // Sell side of the order
    }
}
