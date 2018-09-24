namespace ShutdownCatcherTest
{
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using NLog;

    using Timer = System.Timers.Timer;

    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static ManualResetEvent exitWait;

        public static void Main(string[] args)
        {
            Logger.Info("Starting ShudownCatcherTest");
            exitWait = new ManualResetEvent(false);

            ShutDownCatcher.Instance.SessionEnding += InstanceOnSessionEnding;
            ShutDownCatcher.Instance.PowerChanged += InstanceOnPowerChanged;
            ShutDownCatcher.Instance.SessionEnded += InstanceOnSessionEnded;

            exitWait.WaitOne();
        }

        private static void InstanceOnSessionEnding(object sender, SessionEndingEventArgs sessionEndingEventArgs)
        {
            Logger.Info("SessionEnding {0},{1}", sessionEndingEventArgs.Reason, sessionEndingEventArgs.Cancel);

            sessionEndingEventArgs.Cancel = true;

            Logger.Info("SessionEnding - Cancelled");
        }

        private static void InstanceOnSessionEnded(object sender, SessionEndedEventArgs sessionEndedEventArgs)
        {
            var timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = 10000;
            timer.Elapsed += (s, e) => exitWait.Set();
            timer.Enabled = true;

            Logger.Info("SessionEnded: {0}", sessionEndedEventArgs.Reason);
            for (int i = 0; i < 1000; i++)
            {
                Logger.Info("Sleeping...");
                Thread.Sleep(100);
            }
        }

        private static void InstanceOnPowerChanged(object sender, PowerModeChangedEventArgs powerModeChangedEventArgs)
        {
            Logger.Info("PowerChanged: {0}", powerModeChangedEventArgs.Mode);
        }
    }
}
