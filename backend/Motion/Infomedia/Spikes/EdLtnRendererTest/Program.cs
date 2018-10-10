namespace Gorba.Motion.Infomedia.EdLtnRendererTest
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    using NLog;

    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            var configurator = new FileConfigurator("medi.config");
            MessageDispatcher.Instance.Configure(configurator);

            var handler = new ScreenHandler();
            handler.Start();
            do
            {
                Console.WriteLine("Press q to exit");
            }
            while (Console.Read() != 'q');

            handler.Stop();
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.FatalException("Unhandled exception", (Exception)e.ExceptionObject);
            LogManager.Flush();
        }
    }
}
