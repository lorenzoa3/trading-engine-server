using System;
using System.Collections.Generic;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Interface for an order book that allows retrieval of bid and ask orders
    public interface IRetrievalOrderbook : IOrderEntryOrderbook
    {
        // Retrieves all ask (sell) orders from the order book
        List<OrderbookEntry> GetAskOrders();

        // Retrieves all bid (buy) orders from the order book
        List<OrderbookEntry> GetBidOrders();
    }
}
