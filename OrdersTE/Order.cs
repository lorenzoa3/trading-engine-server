using System;

namespace TradingEngineServer.Orders
{
    // Represents an order with additional properties and behaviors beyond the core
    public class Order : IOrderCore
    {
        // Constructor to initialize an order with core details and trade-specific properties
        public Order(IOrderCore orderCore, long price, uint quantity, bool isBuySide)
        {
            // PROPERTIES //
            Price = price;
            IsBuySide = isBuySide;
            InitialQuantity = quantity;
            CurrentQuantity = quantity;

            // FIELDS //
            _orderCore = orderCore ?? throw new ArgumentNullException(nameof(orderCore));
        }

        // Constructor to initialize from a ModifyOrder object
        public Order(ModifyOrder modifyOrder) :
            this(modifyOrder, modifyOrder.Price, modifyOrder.Quantity, modifyOrder.IsBuySide)
        { }


        // PROPERTIES //
        public long Price { get; private set; }
        public uint InitialQuantity { get; private set; }
        public uint CurrentQuantity { get; private set; }
        public bool IsBuySide { get; private set; }

        // Accessors to the core properties from IOrderCore
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;


        // METHODS //
        // Increases the current quantity of the order
        public void IncreaseQuantity(uint quantityDelta)
        {
            CurrentQuantity += quantityDelta;
        }

        // Decreases the current quantity of the order
        public void DecreaseQuantity(uint quantityDelta)
        {
            if (quantityDelta > CurrentQuantity)
                throw new InvalidOperationException($"Quantity Delta > Current Quantity for OrderId={OrderId}");

            CurrentQuantity -= quantityDelta;
        }


        // FIELDS //
        private readonly IOrderCore _orderCore; // Encapsulates the core properties of the order
    }
}
