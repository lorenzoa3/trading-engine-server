using System;

namespace TradingEngineServer.Orders
{
    // Represents the core properties of an order in the trading system
    public interface IOrderCore
    {
        public long OrderId { get; } // Unique identifier for the order

        public string Username { get; } // Username of the user who placed the order

        public int SecurityId { get; } // Identifier for the security being traded
    }
}
