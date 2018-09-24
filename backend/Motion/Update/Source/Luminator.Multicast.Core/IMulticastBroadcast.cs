namespace Luminator.Multicast.Core
{
    using System;

    public interface IMulticastBroadcast : IDisposable
    {
        #region Public Methods and Operators

        void BroadcastMessage(string message);

        void CloseMultiCastSocket();

        void JoinMulticastGroup();

        #endregion
    }
}