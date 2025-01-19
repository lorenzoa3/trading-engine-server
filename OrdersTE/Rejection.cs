using System;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    // Represents a rejection for an order, including the reason for rejection
    public class Rejection : IOrderCore
    {
        public Rejection(IOrderCore rejectedOrder, RejectionReason rejectionReason)
        {
            // PROPERTIES // 
            RejectionReason = rejectionReason;

            // FIELDS //
            _orderCore = rejectedOrder ?? throw new ArgumentNullException(nameof(rejectedOrder));
        }


        // PROPERTIES // 

        // The reason why the order was rejected
        public RejectionReason RejectionReason { get; private set; }

        // Core order properties from IOrderCore
        public long OrderId => _orderCore.OrderId;
        public string Username => _orderCore.Username;
        public int SecurityId => _orderCore.SecurityId;


        // FIELDS //
        private readonly IOrderCore _orderCore; // Encapsulates the core properties of the rejected order
    }
}
