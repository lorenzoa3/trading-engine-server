using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    // Represents the result of the matching process
    public class MatchResult
    {
        // List of matches: Each entry contains a bid, ask, and matched quantity
        public List<(OrderbookEntry bid, OrderbookEntry ask, uint matchedQuantity)> Matches { get; }

        // Constructor initializes the matches
        public MatchResult(List<(OrderbookEntry bid, OrderbookEntry ask, uint matchedQuantity)> matches)
        {
            Matches = matches ?? throw new ArgumentNullException(nameof(matches));
        }

        // Converts the match result to a string for display/logging
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var match in Matches)
            {
                sb.AppendLine(
                    $"Matched: Bid {match.bid.CurrentOrder.OrderId} " +
                    $"and Ask {match.ask.CurrentOrder.OrderId}, " +
                    $"Quantity: {match.matchedQuantity}, " +
                    $"Price: {match.bid.CurrentOrder.Price}");
            }
            return sb.ToString();
        }
    }
}
