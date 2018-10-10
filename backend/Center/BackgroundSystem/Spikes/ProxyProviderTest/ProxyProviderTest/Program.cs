using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProxyProviderTest
{
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core;
    using Gorba.Center.CommS.Core.ComponentModel.Messages;
    using Gorba.Center.CommS.Core.ComponentModel.Messages.Activities;
    using Gorba.Center.CommS.Wcf.ServiceModel;
    using Gorba.Center.Common.Core.Communication;
    using Gorba.Common.Utility.ConcurrentPriorityQueue;
    using Gorba.Common.Utility.Core;

    using NLog;

    class Program
    {
        private static CommsTester commsTester;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Random rnd = new Random();
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Logger.FatalException(
                    "Unhandled Exception; terminating=" + e.IsTerminating, e.ExceptionObject as Exception);
                LogManager.Flush();
            };

            Logger.Info("Starting application");
            ProxyProvider<ICommsService>.Current =
                new CustomConstructorProxyProvider<ICommsService>(() => new CommsServiceProxy("commsService"));
            ProxyProvider<ICommsMessagingService>.Current =
                new CustomConstructorProxyProvider<ICommsMessagingService>(
                    () => new CommsMessagingServiceProxy("commsMessagingService"));
            commsTester = new CommsTester();
            commsTester.Start();
            List<Thread> threads = new List<Thread>();
            for (int i = 10; i < 70; i++)
            {
                var address = string.Format("A:2.{0}.1", i);
                var task = new Thread(() => SendReferenceMessage(address));
                threads.Add(task);
                task.Start();
            }

            for (int i = 31; i < 100; i++)
            {
                var address = string.Format("A:2.{0}.1", i);
                var task = new Thread(() => SendInfoTextMessage(address, i));
                threads.Add(task);
                task.Start();
            }

                Console.WriteLine("Press q to quit");
            char key;
            do
            {
                key = Console.ReadKey().KeyChar;
                if (key == 'c')
                {
                    Logger.Info("Close proxy provider channel");
                    ProxyProvider<ICommsService>.Current.ResetInstance();
                    var comms = ProxyProvider<ICommsService>.Current.Provide();
                    
                    comms.GetUnitsConnectionStatus(null);
                    
                }
            }
            while (!string.Equals(key.ToString(), "q", StringComparison.InvariantCultureIgnoreCase));
            

            foreach (var thread in threads)
            {
                thread.Abort();
            }

            foreach (var thread in threads)
            {
                thread.Join(5000);
            }

            commsTester.Stop();

            Logger.Info("CommsTester console application stopped!");
        }

        private static void SendInfoTextMessage(string address, int id)
        {
            var activity = new InfoLineTextActivityMessage
                               {
                                   ActivityId = (uint)id + 10000,
                                   Address = address,
                                   Align = InfoLineTextActivityMessage.AlignType.Center,
                                   Blink = false,
                                   DisplayText = "Text",
                                   RowId = 1,
                                   Side = InfoLineTextActivityMessage.IqubeSide.Both,
                                   StartDate = TimeProvider.Current.Now,
                               };
            var revokeMessage = new RevokeMessage { ActivityId = (uint)id + 10000, Address = address };
            bool one = true;
            while (true)
            {
                try
                {
                    ProxyProvider<ICommsService>.Current.ResetInstance();
                    var comms = ProxyProvider<ICommsService>.Current.Provide();
                    Logger.Trace("Poll");
                    comms.GetUnitsConnectionStatus(null);
                    //if (one)
                    //    commsTester.SendMessage(activity);
                    //else
                    //{
                    //    commsTester.SendMessage(revokeMessage);
                    //}
                    //one = !one;
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Error", ex);
                }
                Thread.Sleep(rnd.Next(500, 1000));
            }
        }

        private static void SendReferenceMessage(string address)
        {
           
            var message = new ReferenceTextMessage(address)
                              {
                                  DisplayText = "Text",
                                  ReferenceTextId = 1,
                                  ReferenceType = ReferenceTextType.TextForDestination,
                                  Priority = QueuePriority.High
                              };
            
            while (true)
            {
                
                try
                {
                    ProxyProvider<ICommsService>.Current.ResetInstance();
                    //var comms = ProxyProvider<ICommsService>.Current.Provide();
                    //var commsproxy = comms as CommsServiceProxy;
                    //commsproxy.Close();
                    var comms = ProxyProvider<ICommsService>.Current.Provide();
                    Logger.Trace("Poll");
                    comms.GetUnitsConnectionStatus(null);
                    //commsTester.SendMessage(message);
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Error", ex);
                }
                 
                Thread.Sleep(rnd.Next(1000, 1500));
            }
        }
    }
}
