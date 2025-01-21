using System;
using System.Collections.Generic;
using System.Linq;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public class MatchingOrderBook : IMatchingOrderbook
    {
        private readonly Orderbook _orderbook;

        // Constructor initializes the internal orderbook
        public MatchingOrderBook(Security instrument)
        {
            _orderbook = new Orderbook(instrument);
        }

        // Delegate these methods to the internal orderbook
        public int Count => _orderbook.Count;

        public void AddOrder(Order order) => _orderbook.AddOrder(order);

        public void ChangeOrder(ModifyOrder modifyOrder) => _orderbook.ChangeOrder(modifyOrder);

        public void RemoveOrder(CancelOrder cancelOrder) => _orderbook.RemoveOrder(cancelOrder);

        public bool ContainsOrder(long orderId) => _orderbook.ContainsOrder(orderId);

        public List<OrderbookEntry> GetAskOrders() => _orderbook.GetAskOrders();

        public List<OrderbookEntry> GetBidOrders() => _orderbook.GetBidOrders();

        public OrderbookSpread GetSpread() => _orderbook.GetSpread();

        // Match logic: Implements price-time priority matching
        public MatchResult Match()
        {
            var matches = new List<(OrderbookEntry bid, OrderbookEntry ask, uint matchedQuantity)>();

            // Continue matching while there are valid bids and asks
            while (_orderbook.GetBidOrders().Any() && _orderbook.GetAskOrders().Any())
            {
                // Get the best bid and ask
                var bestBid = _orderbook.GetBidOrders().First(); // Best bid is the first in the list
                var bestAsk = _orderbook.GetAskOrders().First(); // Best ask is the first in the list

                // Skip empty limits
                if (bestBid.ParentLimit.IsEmpty || bestAsk.ParentLimit.IsEmpty)
                    break;

                // Stop if there's no price overlap
                if (bestBid.CurrentOrder.Price < bestAsk.CurrentOrder.Price)
                    break;

                // Determine the matched quantity
                uint matchedQuantity = Math.Min(bestBid.CurrentOrder.CurrentQuantity, bestAsk.CurrentOrder.CurrentQuantity);

                // Record the match
                matches.Add((bestBid, bestAsk, matchedQuantity));

                // Update bid and ask quantities
                bestBid.CurrentOrder.DecreaseQuantity(matchedQuantity);
                bestAsk.CurrentOrder.DecreaseQuantity(matchedQuantity);

                // Remove fully filled bid order
                if (bestBid.CurrentOrder.CurrentQuantity == 0)
                {
                    var cancelBid = new CancelOrder(bestBid.CurrentOrder);
                    _orderbook.RemoveOrder(cancelBid);
                }

                // Remove fully filled ask order
                if (bestAsk.CurrentOrder.CurrentQuantity == 0)
                {
                    var cancelAsk = new CancelOrder(bestAsk.CurrentOrder);
                    _orderbook.RemoveOrder(cancelAsk);
                }
            }

            // Return the match result
            return new MatchResult(matches);
        }

    }
}
