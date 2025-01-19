using System;

namespace TradingEngineServer.Orders
{
    /// <summary>
    /// Read-only representation of an order.
    /// </summary>
    public record OrderRecord(
        long OrderId,                  // Unique identifier for the order
        uint Quantity,                 // Quantity of the order
        long Price,                    // Price of the order
        bool IsBuySide,                // Indicates whether the order is on the buy side
        string Username,               // Username associated with the order
        int SecurityId,                // Identifier for the security being traded
        uint TheoreticalQueuePosition  // Theoretical position in the queue
    );
}

// This is here to enable record types (due to a VS bug?)
// Required to support records in certain environments
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { };
}
