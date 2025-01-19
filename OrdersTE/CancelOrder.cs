using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    // Represents a request to cancel an existing order
    public class CancelOrder : IOrderCore
    {
        public CancelOrder(IOrderCore orderCore)
        {
            // FIELDS //
            _orderCore = orderCore;
        }

        // PROPERTIES //
        // Accessors to the core properties from IOrderCore
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;


        // FIELDS //
        private readonly IOrderCore _orderCore;
    }
}
