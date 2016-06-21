using NLog;
using System.Collections.Generic;

namespace Silkroad.Framework.Utility
{
    public class StaticLogger
    {
        public static Logger Instance { get; set; }

        private static Dictionary<string, Logger> _logger = new Dictionary<string, NLog.Logger>();

        public static IReadOnlyDictionary<string, Logger> Logger
        {
            get { return _logger; }
        }

        private static void ConfigRule(NLog.Config.LoggingRule rule, int minLevel)
        {
            //Disable all loglevel rules
            for (int i = 0; i < 6; i++)
            {
                rule.DisableLoggingForLevel(LogLevel.FromOrdinal(i));
            }

            //Enable present loglevel rules up to maxLevel
            if (minLevel > 0)
            {
                for (int i = 0; i <= minLevel; i++)
                {
                    var logLevel = LogLevel.FromOrdinal(6 - i);
                    if (logLevel != LogLevel.Off)
                        rule.EnableLoggingForLevel(logLevel);
                }
            }
        }

        public static void SetInstance()
        {
            Instance = _logger["Instance"];
        }

        public static void Create(string key)
        {
            _logger.Add(key, LogManager.GetLogger(key));
        }

        public static void SetLogLevel(int minLevel, Logger logger)
        {
            SetLogLevel(minLevel, logger.Name);
        }

        public static void SetLogLevel(int minLevel, string logger)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                if (rule.NameMatches(logger))
                {
                    ConfigRule(rule, 6 - minLevel);
                }
            }
            LogManager.ReconfigExistingLoggers();
        }
    }
}