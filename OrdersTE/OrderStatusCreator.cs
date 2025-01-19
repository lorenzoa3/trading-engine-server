using System;

namespace TradingEngineServer.Orders
{
    // Utility class for creating order status objects
    public sealed class OrderStatusCreator
    {
        // Generates a CancelOrderStatus object for a given CancelOrder
        public static CancelOrderStatus GenerateCancelOrderStatus(CancelOrder cancelOrder)
        {
            return new CancelOrderStatus();
        }

        // Generates a NewOrderStatus object for a given Order
        public static NewOrderStatus GenerateNewOrderStatus(Order newOrder)
        {
            return new NewOrderStatus();
        }

        // Generates a ModifyOrderStatus object for a given ModifyOrder
        public static ModifyOrderStatus GenerateModifyOrderStatus(ModifyOrder modifyOrder)
        {
            return new ModifyOrderStatus();
        }
    }
}
