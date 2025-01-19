using System;
using System.Collections.Generic;

namespace TradingEngineServer.Orders
{
    // Represents a price level in the order book
    public class Limit
    {
        public Limit(long price)
        {
            Price = price;
        }

        // PROPERTIES //
        public long Price { get; set; } // Price level for this limit
        public OrderbookEntry Head { get; set; } // Points to the first order at this price level
        public OrderbookEntry Tail { get; set; } // Points to the last order at this price level

        // Checks if the limit level is empty
        public bool IsEmpty
        {
            get
            {
                return Head == null && Tail == null;
            }
        }

        // Determines the side (Bid or Ask) of the orders at this limit
        public Side Side
        {
            get
            {
                if (IsEmpty)
                    return Side.Unknown;
                else
                {
                    return Head.CurrentOrder.IsBuySide ? Side.Bid : Side.Ask;
                }
            }
        }

        // METHODS //

        // Gets the count of orders at this price level
        public uint GetLevelOrderCount()
        {
            uint orderCount = 0;
            OrderbookEntry headPointer = Head;

            while (headPointer != null)
            {
                if (headPointer.CurrentOrder.CurrentQuantity != 0)
                    orderCount++;

                headPointer = headPointer.Next;
            }

            return orderCount;
        }

        // Gets the total quantity of orders at this price level
        public uint GetLevelOrderQuantity()
        {
            uint orderQuantity = 0;
            OrderbookEntry headPointer = Head;

            while (headPointer != null)
            {
                orderQuantity += headPointer.CurrentOrder.CurrentQuantity;

                headPointer = headPointer.Next;
            }

            return orderQuantity;
        }

        // Creates a list of order records for all orders at this price level
        public List<OrderRecord> GetLevelOrderRecords()
        {
            List<OrderRecord> orderRecords = new List<OrderRecord>();

            OrderbookEntry headPointer = Head;
            uint theoreticalQueuePosition = 0;

            while (headPointer != null)
            {
                var currentOrder = headPointer.CurrentOrder;

                if (currentOrder.CurrentQuantity != 0)
                    orderRecords.Add(new OrderRecord(
                        currentOrder.OrderId,
                        currentOrder.CurrentQuantity,
                        currentOrder.Price,
                        currentOrder.IsBuySide,
                        currentOrder.Username,
                        currentOrder.SecurityId,
                        theoreticalQueuePosition));

                theoreticalQueuePosition++;
                headPointer = headPointer.Next;
            }

            return orderRecords;
        }
    }
}
