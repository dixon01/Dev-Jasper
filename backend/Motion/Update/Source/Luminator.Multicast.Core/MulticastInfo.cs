namespace Luminator.Multicast.Core
{
    using System.Net;

    public class MulticastInfo
    {
        public MulticastInfo() { }
        public MulticastInfo(string multicastListenerSocketAdaptorName, IPAddress multicastListenerSocketAddress, string multicastMessageReceived, IPAddress multicastReceivedFromIpAddress, IPAddress muticastReceivedFromSubnetMask)
        {
            this.MulticastListenerSocketAdaptorName = multicastListenerSocketAdaptorName;
            this.MulticastListenerSocketAddress = multicastListenerSocketAddress;
            this.MulticastMessageReceived = multicastMessageReceived;
            this.MulticastReceivedFromIpAddress = multicastReceivedFromIpAddress;
            this.MuticastReceivedFromSubnetMask = muticastReceivedFromSubnetMask;
        }

        #region Public Properties

        public string MulticastListenerSocketAdaptorName { get; set; }

        public IPAddress MulticastListenerSocketAddress { get; set; }

        public string MulticastMessageReceived { get; set; }

        public IPAddress MulticastReceivedFromIpAddress { get; set; }

        public IPAddress MuticastReceivedFromSubnetMask { get; set; }

        #endregion

      
    }
}