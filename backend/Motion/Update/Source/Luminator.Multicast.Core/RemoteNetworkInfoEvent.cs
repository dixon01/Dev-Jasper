namespace Luminator.Multicast.Core
{
    using System;

    public class RemoteNetworkInfoEvent : EventArgs
    {
        #region Constructors and Destructors

        public RemoteNetworkInfoEvent(RemoteNetworkInfo remoteNetworkInfo)
        {
            this.RemoteNetworkInfo = remoteNetworkInfo;
        }

        #endregion

        #region Public Properties

        public RemoteNetworkInfo RemoteNetworkInfo { get; set; }

        #endregion
    }
}