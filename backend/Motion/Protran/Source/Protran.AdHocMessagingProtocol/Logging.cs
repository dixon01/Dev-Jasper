namespace Luminator.Motion.Protran.AdHocMessagingProtocol
{
    using System.Diagnostics;

    using NLog;

    internal static class Logging<T>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(typeof(T));

        public static void Info(string format, params object[] args)
        {
            Logger.Info(format, args);
            Debug.WriteLine(format, args);
        }

        public static void Warn(string format, params object[] args)
        {
            Logger.Warn(format, args);
            Debug.WriteLine(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Logger.Error(format, args);
           Debug.WriteLine(format, args);
        }
    }
}
