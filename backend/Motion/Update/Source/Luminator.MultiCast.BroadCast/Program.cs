// This sender example must be used in conjunction with the listener program.
// You must run this program as follows:
// Open a console window and run the listener from the command line. 
// In another console window run the sender. In both cases you must specify 
// the local IPAddress to use. To obtain this address,  run the ipconfig command 
// from the command line. 
//  

namespace Luminator.MultiCast.BroadCast
{
    using System;
    using System.Net;
    using System.Timers;

    using Luminator.Multicast.BroadCast.Properties;
    using Luminator.Multicast.Core;

    internal class TestMulticastOption
    {
        #region Static Fields

        private static readonly int mcastPort = 31000;

        // Initialize the multicast address group and multicast port.
        // Both address and port are selected from the allowed sets as
        // defined in the related RFC documents. These are the same 
        // as the values used by the sender.
        private static IPAddress mcastAddress = IPAddress.Any;

        private static MulticastBroadcast multicast;

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            const string IpForMultiCast = "239.255.100.2";
            mcastAddress = IPAddress.Parse(IpForMultiCast);

            Console.WriteLine("Multicast Group => " + IpForMultiCast + ":" + mcastPort);
            var text = new RandomText();

            multicast = new MulticastBroadcast();
            multicast.BroadcastMessage(mcastAddress, mcastPort, "Hello First");
            // Join the listener multicast group.
            multicast.JoinMulticastGroup();

            var timer = new Timer(5000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
            Console.ReadKey();
            multicast.CloseMultiCastSocket();
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Settings.Default.GenerateRandomMessages)
            {
                var text = new RandomText();
                text.AddContentParagraphs(2, 2, 2, 5, 11);
                multicast.BroadcastMessage(mcastAddress, mcastPort, text.Content + " => " + NetworkUtils.GetLocalIpAddress());
            }
            else
            {
                multicast.BroadcastMessage(mcastAddress, mcastPort, "Hello From MultiCast ");
            }
        }

        #endregion
    }
}