// This is the listener example that shows how to use the MulticastOption class. 
// In particular, it shows how to use the MulticastOption(IPAddress, IPAddress) 
// constructor, which you need to use if you have a host with more than one 
// network card.
// The first parameter specifies the multicast group address, and the second 
// specifies the local address of the network card you want to use for the data
// exchange.
// You must run this program in conjunction with the sender program as 
// follows:
// Open a console window and run the listener from the command line. 
// In another console window run the sender. In both cases you must specify 
// the local IPAddress to use. To obtain this address run the ipconfig comand 
// from the command line. 
//  

namespace Luminator.MultiCast.Receive
{
    using System;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Timers;

    using Luminator.Multicast.Core;
    using Luminator.MultiCast.Receive.Properties;

    public class TestMulticastOption
    {
        #region Public Methods and Operators

        private static MulticastManager multicastManager;
        public static void Main(string[] args)
        {

            Console.WriteLine("Starting application Please wait....");
            multicastManager = new MulticastManager();
             multicastManager.StartMulticast();
            Console.ReadKey();
            multicastManager.Stop();
        }

        #endregion
    }
}