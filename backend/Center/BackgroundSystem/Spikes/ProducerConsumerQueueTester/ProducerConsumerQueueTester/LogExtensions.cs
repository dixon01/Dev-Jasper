namespace Gorba.Center.BackgroundSystem.Spikes.ProducerConsumerQueueTester
{
    using System.Globalization;

    using NLog;

    public static class LogExtensions
    {
        public static void Trace(this Logger logger, ProducerConsumerQueueType type, string start, string format, params object[] args)
        {
            var info = new LogEventInfo(LogLevel.Trace, logger.Name, CultureInfo.CurrentUICulture, format, args);
            info.Properties.Add("Type", type);
            info.Properties.Add("Start", start);
            logger.Log(info);
        }

        public static void Debug(this Logger logger, ProducerConsumerQueueType type, string start, string format, params object[] args)
        {
            var info = new LogEventInfo(LogLevel.Debug, logger.Name, CultureInfo.CurrentUICulture, format, args);
            info.Properties.Add("Type", type);
            info.Properties.Add("Start", start);
            logger.Log(info);
        }

        public static void Info(this Logger logger, ProducerConsumerQueueType type, string start, string format, params object[] args)
        {
            var info = new LogEventInfo(LogLevel.Info, logger.Name, CultureInfo.CurrentUICulture, format, args);
            info.Properties.Add("Type", type);
            info.Properties.Add("Start", start);
            logger.Log(info);
        }
    }
}