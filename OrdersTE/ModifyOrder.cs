using System;

namespace TradingEngineServer.Orders
{
    // Represents an order modification, with functionality to convert to other order types
    public class ModifyOrder : IOrderCore
    {
        public ModifyOrder(IOrderCore orderCore, long modifyPrice, uint modifyQuantity, bool isBuySide)
        {
            // PROPERTIES //
            Price = modifyPrice;
            Quantity = modifyQuantity;
            IsBuySide = isBuySide;

            // FIELDS //
            _orderCore = orderCore ?? throw new ArgumentNullException(nameof(orderCore));
        }

        // PROPERTIES //
        public long Price { get; set; } // Modified price of the order
        public uint Quantity { get; set; } // Modified quantity of the order
        public bool IsBuySide { get; set; } // Indicates if the order remains on the buy side

        // Core order properties from IOrderCore
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;


        // METHODS //
        // Converts the ModifyOrder into a CancelOrder
        public CancelOrder ToCancelOrder()
        {
            return new CancelOrder(this);
        }

        // Converts the ModifyOrder into a new Order
        public Order ToNewOrder()
        {
            return new Order(this);
        }


        // FIELDS //
        private readonly IOrderCore _orderCore; // Encapsulates the original core order
    }
}
