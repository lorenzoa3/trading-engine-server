using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public class OrderCore : IOrderCore // Immutable (Can only get fields, not set them)
    {
        public OrderCore(long orderId, string username, int securityId)
        {
            OrderId = orderId;
            Username = username;
            SecurityId = securityId;
        }

        public long OrderId { get; private set; } // Unique identifier for the order
        public int SecurityId { get; private set; } // Identifier for the security being traded
        public string Username { get; private set; } // Username of the user who placed the order
    }
}
