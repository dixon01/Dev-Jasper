namespace Luminator.Multicast.Core
{
    using System;
    using System.Net;

    public interface IMulticastReceive : IDisposable
    {
        #region Public Methods and Operators

        void MulticastOptionProperties();

        void ReceiveBroadcastMessages();

        void StartMulticast(IPAddress mcastAddressIn, int mcastPortIn);

        #endregion
    }
}