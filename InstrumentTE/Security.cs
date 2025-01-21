namespace TradingEngineServer.Instrument
{
    // Represents a financial security, such as a stock or bond
    public class Security
    {
        // Unique identifier for the security
        public int SecurityId { get; }

        // Symbol or ticker of the security (e.g., "AAPL")
        public string Symbol { get; }

        // Full name of the security (e.g., "Apple Inc.")
        public string Name { get; }

        // Constructor to initialize the security
        public Security(int securityId, string symbol, string name)
        {
            SecurityId = securityId;
            Symbol = symbol;
            Name = name;
        }

        // Override ToString for easy debugging or logging
        public override string ToString()
        {
            return $"{Symbol} ({Name})";
        }
    }
}
