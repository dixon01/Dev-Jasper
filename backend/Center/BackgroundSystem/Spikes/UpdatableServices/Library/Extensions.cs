namespace Library
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Library.Client;
    using Library.Model;
    using Library.ServiceModel;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    using Newtonsoft.Json;

    using NLog;
    using NLog.Config;

    public static class Extensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Tenant Apply(this TenantDelta delta, Tenant target)
        {
            throw new NotImplementedException();
        }

        public static Stream Serialize(this Delta delta)
        {
            var memoryStream = new MemoryStream();
            using (var content = new StreamWriter(memoryStream))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(content, delta);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public static T Deserialize<T>(this BrokeredMessage message)
        {
            try
            {
                var stream = message.GetBody<Stream>();
                using (var streamReader = new StreamReader(stream))
                {
                    using (var reader = new JsonTextReader(streamReader))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<T>(reader);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Error", exception);
                throw;
            }
        }

        public async static Task<TopicClient> EnsureTopic(
            this TrackingServiceConfiguration configuration)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
            return await namespaceManager.EnsureTopic(configuration.ConnectionString, configuration.TopicName);
        }

        public async static Task<TopicClient> EnsureTopic(this NamespaceManager namespaceManager, string connectionString, string name)
        {
            if (!(await namespaceManager.TopicExistsAsync(name)))
            {
                await namespaceManager.CreateTopicAsync(name);
            }

            return TopicClient.CreateFromConnectionString(connectionString, name);
        }

        public async static Task<SubscriptionClient> EnsureSubscription(
            this TrackingProxyConfiguration configuration,
            string sqlFilter = null)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
            var subscriptionName = string.Format("{0}_{1}", configuration.ApplicationName, configuration.SessionId);
            return
                await
                namespaceManager.EnsureSubscription(
                    configuration.ConnectionString,
                    configuration.TopicName,
                    subscriptionName,
                    sqlFilter);
        }

        public async static Task<SubscriptionClient> EnsureSubscription(
            this TrackingServiceConfiguration configuration,
            string sqlFilter = null)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
            return
                await
                namespaceManager.EnsureSubscription(
                    configuration.ConnectionString,
                    configuration.TopicName,
                    configuration.ApplicationName,
                    sqlFilter);
        }

        public async static Task<SubscriptionClient> EnsureSubscription(this NamespaceManager namespaceManager, string connectionString, string name, string subscriptionName, string sqlFilter = null)
        {
            if (
                !(await
                  namespaceManager.SubscriptionExistsAsync(
                      name,
                      subscriptionName)))
            {
                var filter = string.IsNullOrEmpty(sqlFilter)
                                 ? new TrueFilter()
                                 : new SqlFilter(sqlFilter);
                var subscription =
                    await
                    namespaceManager.CreateSubscriptionAsync(
                        name,
                        subscriptionName,
                        filter);
            }

            var client = SubscriptionClient.CreateFromConnectionString(
                connectionString,
                name,
                subscriptionName);
            return client;
        }
    }
}
