using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Logging.LoggingConfiguration
{
    public class LoggerConfiguration
    {
        public LoggerType LoggerType { get; set; }
        // public DatabaseLoggerConfiguration DatabaseLoggerConfiguration { get; set; }
        public TextLoggerConfiguration TextLoggerConfiguration{ get; set; }
    }
    public class DatabaseLoggerConfiguration
    {

    }

    public class TextLoggerConfiguration
    {
        public string directory { get; set; }

        public string fileName { get; set; }

        public string fileExtension { get; set; }
    }
}
