using System;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    // Utility class for creating Rejection objects
    public sealed class RejectCreator
    {
        // Generates a Rejection object for the specified order and reason
        public static Rejection GenerateOrderCoreRejection(IOrderCore orderCore, RejectionReason rejectionReason)
        {
            return new Rejection(orderCore, rejectionReason);
        }
    }
}
