using System;

namespace Library.Client
{
    public class UpdatableDataServiceManager
    {
        static UpdatableDataServiceManager()
        {
            TenantsClientConfiguration = new ClientServiceBusConfiguration
                                {
                                    SubscriptionName = "WpfApplication",
                                    TopicName = "Tenants"
                                };

            TenantsConfiguration = new ServiceBusConfiguration
                                {
                                    SubscriptionName = "UpdatableDataServiceManager",
                                    TopicName = "Tenants"
                                };

            UsersClientConfiguration = new ClientServiceBusConfiguration
                                {
                                    SubscriptionName = "WpfApplication",
                                    TopicName = "Users"
                                };

            UsersConfiguration = new ServiceBusConfiguration
                                {
                                    SubscriptionName = "UpdatableDataServiceManager",
                                    TopicName = "Users"
                                };
        }

        public static ClientServiceBusConfiguration TenantsClientConfiguration { get; private set; }

        public static ServiceBusConfiguration TenantsConfiguration { get; private set; }

        public static ClientServiceBusConfiguration UsersClientConfiguration { get; private set; }

        public static ServiceBusConfiguration UsersConfiguration { get; private set; }

        public IDisposable SubscribeToTenants(string applicationName)
        {
            var proxy = new UpdatableTenantDataServiceProxy(TenantsClientConfiguration, applicationName);
            AsyncPump.Run(proxy.Start);
            return proxy;
        }

        public IDisposable SubscribeToUsers(string applicationName)
        {
            var proxy = new UpdatableUserDataServiceProxy(UsersClientConfiguration, applicationName);
            AsyncPump.Run(proxy.Start);
            return proxy;
        }
    }
}