using System;

namespace TradingEngineServer.Orderbook
{
    // Represents the spread of an order book, including the best bid, best ask, and the spread
    public class OrderbookSpread
    {
        // Constructor to initialize bid and ask values
        // Nullable to distinguish whether bid/ask values are present
        public OrderbookSpread(long? bid, long? ask)
        {
            Bid = bid;
            Ask = ask;
        }

        // Best bid price in the order book
        public long? Bid { get; private set; }

        // Best ask price in the order book
        public long? Ask { get; private set; }

        // Difference between the best ask and bid prices (spread)
        public long? Spread
        {
            get
            {
                if (Bid.HasValue && Ask.HasValue)
                    return Ask.Value - Bid.Value; // Spread is the difference between ask and bid
                else
                    return null; // No spread if bid or ask is missing
            }
        }
    }
}
