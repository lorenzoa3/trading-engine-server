using System;
using System.Collections.Generic;

namespace TradingEngineServer.Orders
{
    // Compares bid price levels in descending order (higher prices first)
    public class BidLimitComparer : IComparer<Limit>
    {
        // Singleton instance of the comparer
        public static IComparer<Limit> Comparer { get; } = new BidLimitComparer();

        public int Compare(Limit x, Limit y)
        {
            if (x.Price == y.Price)
                return 0;
            else if (x.Price > y.Price)
                return -1; // Higher prices come first
            else
                return 1; // Lower prices come later
        }
    }

    // Compares ask price levels in ascending order (lower prices first)
    public class AskLimitComparer : IComparer<Limit>
    {
        // Singleton instance of the comparer
        public static IComparer<Limit> Comparer { get; } = new AskLimitComparer();

        public int Compare(Limit x, Limit y)
        {
            if (x.Price == y.Price)
                return 0;
            else if (x.Price > y.Price)
                return 1; // Higher prices come later
            else
                return -1; // Lower prices come first
        }
    }
}
