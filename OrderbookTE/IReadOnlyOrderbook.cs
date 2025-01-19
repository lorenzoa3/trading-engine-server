using System;

namespace TradingEngineServer.Orderbook
{
    // Interface for a read-only view of the order book
    public interface IReadOnlyOrderbook
    {
        // Checks if an order with the specified ID exists in the order book
        bool ContainsOrder(long orderId);

        // Retrieves the current spread of the order book
        OrderbookSpread GetSpread();

        // Gets the total number of orders in the order book
        int Count { get; }
    }
}
