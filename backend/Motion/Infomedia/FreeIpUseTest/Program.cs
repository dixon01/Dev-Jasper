namespace Luminator.FindFreeIpTestTool
{
    using System;
    using System.Net;
    using System.Threading;

    using Luminator.Multicast.Core;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter a IP address argument.");
                return;
            }
            var ipAddress = IPAddress.None;
            try
            {
                ipAddress = IPAddress.Parse(args[0]);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message + e.InnerException?.Message);
                Console.ReadKey();
                return;
            }
            var result = NetworkUtils.GetNextFreeIpAddress(ipAddress);
            Console.WriteLine($"Free available ip to use : {result}");
            string localAdaptorDescription = NetworkUtils.GetAdapterDescFromIp4Address(NetworkUtils.GetLocalIpAddress());
            bool staticIpVerified = false;
            do
            {
                Console.WriteLine($"Local Ip is being set to : {result}");
                NetworkManagement.SetSystemIp(result.ToString(),"255.255.255.224", localAdaptorDescription);
                Thread.Sleep(5000);
                var localIpAddress = NetworkUtils.GetLocalIpAddress();
                Console.WriteLine($"Local Ip address query returned: {localIpAddress}");
                if (localIpAddress.Equals(result))
                {
                    Console.WriteLine($"Local Ip address Set Successfully to : {localIpAddress}");
                    staticIpVerified = true;
                }
                else
                {
                    var nextIpaddressToTry = NetworkUtils.IncrementIpAddress(ipAddress);
                    result = NetworkUtils.GetNextFreeIpAddress(nextIpaddressToTry);
                    Console.WriteLine($"Local Ip set FAIL going to try new ip: {result}");
                }
            }
            while (staticIpVerified == false);

            Console.ReadKey();

        }
    }
}
