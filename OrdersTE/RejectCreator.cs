using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Rejects
{
    public sealed class RejectCreator
    {
        public static Rejection GenerateOrderCoreRejection(IOrderCore orderCore, RejectionReason rejectionReason)
        {
            return new Rejection(orderCore, rejectionReason);
        }
    }
}
