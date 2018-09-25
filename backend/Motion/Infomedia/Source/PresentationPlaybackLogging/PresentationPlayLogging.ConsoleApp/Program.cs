namespace Luminator.PresentaionPlayLogging.ConsoleApp
{
    using System;

    using Gorba.Common.SystemManagement.Host;
    using NLog;

    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                new ConsoleApplicationHost<PresentationLoggingImportApplication>(args).Run("PresentationLoggingImport");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to Start Import service, Exception: {0}", ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
