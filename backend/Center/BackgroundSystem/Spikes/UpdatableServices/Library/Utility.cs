namespace Library
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public static class Utility
    {
        public static TChannel CreateChannel<TChannel, TEntity>()
        {
            var remoteAddress = GetAddress<TEntity>();
            var channelFactory = new ChannelFactory<TChannel>(
                GetBinding(),
                new EndpointAddress(remoteAddress));
            return channelFactory.CreateChannel();
        }

        public static Binding GetBinding()
        {
            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
        }

        public static Uri GetAddress<T>()
        {
            return new Uri(string.Format("net.pipe://localhost/{0}TrackingService", typeof(T).Name));
        }

        public static void CloseChannel(object channel)
        {
            try
            {
                var communicationObject = (ICommunicationObject)channel;
                communicationObject.Close();
            }
            catch (Exception exception)
            {
            }
        }
    }
}
