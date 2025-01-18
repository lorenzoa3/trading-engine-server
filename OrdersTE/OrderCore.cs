﻿using System;
using System.Collections.Generic;
using System.Text;
using OrdersTE;

namespace TradingEngineServer.Orders
{
    public class OrderCore : IOrderCore // Emutable (Can only get fields, not set them)
    {
        public OrderCore(long orderId, string username, int securityId)
        {
            OrderId = orderId;
            Username = username;
            SecurityId = securityId;
        }

        public long OrderId { get; private set; }
        public int SecurityId { get; private set; }
        public string Username { get; private set; }
    }
}
