using System;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Interface for an order book that allows order modifications
    public interface IOrderEntryOrderbook : IReadOnlyOrderbook
    {
        // Adds a new order to the order book
        void AddOrder(Order order);

        // Modifies an existing order in the order book
        void ChangeOrder(ModifyOrder modifyOrder);

        // Removes an order from the order book
        void RemoveOrder(CancelOrder cancelOrder);
    }
}
