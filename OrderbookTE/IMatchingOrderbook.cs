using System;

namespace TradingEngineServer.Orderbook
{
    // Interface for an order book capable of matching orders
    public interface IMatchingOrderbook : IRetrievalOrderbook
    {
        // Matches orders in the order book and returns the result
        MatchResult Match();
    }
}
