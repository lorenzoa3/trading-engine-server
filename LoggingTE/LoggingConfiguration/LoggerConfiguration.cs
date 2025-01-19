using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Logging.LoggingConfiguration
{
    public class LoggerConfiguration
    {
        // Specifies the type of logger (e.g., Text, Database)
        public LoggerType LoggerType { get; set; }
        // Configuration settings specific to text-based logging
        public TextLoggerConfiguration TextLoggerConfiguration { get; set; }

        // Placeholder for database logger configuration
        // public DatabaseLoggerConfiguration DatabaseLoggerConfiguration { get; set; }
    }

    public class DatabaseLoggerConfiguration
    {
        // Placeholder class for database logging settings
    }

    public class TextLoggerConfiguration
    {
        public string Directory { get; set; } // Directory where log files are stored
        public string FileName { get; set; } // Name of the log file
        public string FileExtension { get; set; } // File extension (e.g., ".log")
    }
}
