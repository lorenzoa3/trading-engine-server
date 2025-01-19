using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public class ModifyOrder : IOrderCore
    {
        public ModifyOrder(IOrderCore orderCore, long modifyPrice, uint modifyQuantity, bool isBuySide)
        {
            // PROPERTIES //
            Price = modifyPrice;
            Quantity = modifyQuantity;
            IsBuySide = isBuySide;

            // FIELDS //
            _orderCore = orderCore;
        }

        // PROPERTIES //
        public long Price { get; set; }
        public uint Quantity { get; set; }
        public bool IsBuySide { get; set; }
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;

        // METHODS //
        public CancelOrder ToCancelOrder()
        {
            return new CancelOrder(this);
        }

        public Order ToNewOrder()
        {
            return new Order(this);
        }

        // FIELDS //
        IOrderCore _orderCore;
    }
}
