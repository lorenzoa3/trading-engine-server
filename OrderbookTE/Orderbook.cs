using System;
using System.Collections.Generic;
using System.Linq;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Represents an order book for a specific security, handling order management and retrieval
    public class Orderbook : IRetrievalOrderbook
    {
        // PRIVATE FIELDS //
        private readonly Security _instrument; // Security associated with this order book
        private readonly Dictionary<long, OrderbookEntry> _orders = new Dictionary<long, OrderbookEntry>(); // Tracks all orders by ID
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer); // Sorted ask price levels
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer); // Sorted bid price levels

        // Constructor initializes the order book with a specific security
        public Orderbook(Security instrument)
        {
            _instrument = instrument;
        }

        // Number of orders currently in the order book
        public int Count => _orders.Count;

        // Adds a new order to the order book
        public void AddOrder(Order order)
        {
            var baseLimit = new Limit(order.Price);
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        // Helper method for adding an order to the order book
        private static void AddOrder(Order order, Limit baseLimit, SortedSet<Limit> limitLevels,
            Dictionary<long, OrderbookEntry> internalOrderbook)
        {
            if (limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                OrderbookEntry orderbookEntry = new OrderbookEntry(order, limit);
                // Add order to an existing price level
                if (limit.Head == null)
                {
                    limit.Head = orderbookEntry;
                    limit.Tail = orderbookEntry;
                }
                else
                {
                    limit.Tail.Next = orderbookEntry;
                    orderbookEntry.Previous = limit.Tail;
                    limit.Tail = orderbookEntry;
                }
                internalOrderbook.Add(order.OrderId, orderbookEntry);
            }
            else
            {
                OrderbookEntry orderbookEntry = new OrderbookEntry(order, baseLimit);
                // Create a new price level for the order
                limitLevels.Add(baseLimit);
                baseLimit.Head = orderbookEntry;
                baseLimit.Tail = orderbookEntry;
                internalOrderbook.Add(order.OrderId, orderbookEntry);
            }
        }

        // Modifies an existing order in the order book
        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            if (_orders.TryGetValue(modifyOrder.OrderId, out OrderbookEntry obe))
            {
                RemoveOrder(modifyOrder.ToCancelOrder());
                AddOrder(modifyOrder.ToNewOrder(), obe.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        // Checks if the order book contains an order with the specified ID
        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);
        }

        // Retrieves all ask orders in the order book
        public List<OrderbookEntry> GetAskOrders()
        {
            List<OrderbookEntry> orderbookEntries = new List<OrderbookEntry>();

            foreach (var askLimit in _askLimits)
            {
                if (askLimit.IsEmpty)
                    continue;

                OrderbookEntry askLimitPointer = askLimit.Head;
                while (askLimitPointer != null)
                {
                    orderbookEntries.Add(askLimitPointer);
                    askLimitPointer = askLimitPointer.Next;
                }
            }
            return orderbookEntries;
        }

        // Retrieves all bid orders in the order book
        public List<OrderbookEntry> GetBidOrders()
        {
            List<OrderbookEntry> orderbookEntries = new List<OrderbookEntry>();

            foreach (var bidLimit in _bidLimits)
            {
                if (bidLimit.IsEmpty)
                    continue;

                OrderbookEntry bidLimitPointer = bidLimit.Head;
                while (bidLimitPointer != null)
                {
                    orderbookEntries.Add(bidLimitPointer);
                    bidLimitPointer = bidLimitPointer.Next;
                }
            }
            return orderbookEntries;
        }

        // Retrieves the current spread of the order book
        public OrderbookSpread GetSpread()
        {
            long? bestAsk = null, bestBid = null;

            if (_askLimits.Any() && !_askLimits.Min.IsEmpty)
                bestAsk = _askLimits.Min.Price;

            if (_bidLimits.Any() && !_bidLimits.Max.IsEmpty)
                bestBid = _bidLimits.Max.Price;

            return new OrderbookSpread(bestBid, bestAsk);
        }

        // Removes an order from the order book
        public void RemoveOrder(CancelOrder cancelOrder)
        {
            if (_orders.TryGetValue(cancelOrder.OrderId, out OrderbookEntry obe))
            {
                RemoveOrder(cancelOrder.OrderId, obe, _orders);
            }
        }

        // Helper method for removing an order from the order book
        private static void RemoveOrder(long orderId, OrderbookEntry obe,
            Dictionary<long, OrderbookEntry> internalOrderbook)
        {
            // Adjust pointers in the linked list
            if (obe.Previous != null && obe.Next != null)
            {
                obe.Next.Previous = obe.Previous;
                obe.Previous.Next = obe.Next;
            }
            else if (obe.Previous != null)
            {
                obe.Previous.Next = null;
            }
            else if (obe.Next != null)
            {
                obe.Next.Previous = null;
            }

            // Update the limit level's head/tail pointers
            if (obe.ParentLimit.Head == obe && obe.ParentLimit.Tail == obe)
            {
                obe.ParentLimit.Head = null;
                obe.ParentLimit.Tail = null;
            }
            else if (obe.ParentLimit.Head == obe)
            {
                obe.ParentLimit.Head = obe.Next;
            }
            else if (obe.ParentLimit.Tail == obe)
            {
                obe.ParentLimit.Tail = obe.Previous;
            }

            // Remove the order from the internal dictionary
            internalOrderbook.Remove(orderId);
        }
    }
}
