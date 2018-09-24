namespace Library.Services
{
    using System;
    using System.Threading.Tasks;

    using Library.ServiceModel;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    using Nito.AsyncEx;

    using NLog;

    public class UpdatableDataManager
    {
        private const string ConnectionStringName = "ServiceBus";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AsyncLock locker = new AsyncLock();

        private bool isStarted;

        private SubscriptionClient tenantsSubscriptionClient;

        private SubscriptionClient usersSubscriptionClient;

        private TopicClient tenantsUpdatableDataManagerTopicClient;

        private TopicClient usersUpdatableDataManagerTopicClient;

        public UpdatableDataManager(
            ServiceBusConfiguration tenantsServiceBusConfiguration,
            ITenantDataService tenantDataService,
            IUserDataService userDataService,
            ServiceBusConfiguration usersServiceBusConfiguration)
        {
            this.UsersServiceBusConfiguration = usersServiceBusConfiguration;
            this.TenantsServiceBusConfiguration = tenantsServiceBusConfiguration;
            this.TenantDataService = tenantDataService;
            this.UserDataService = userDataService;
        }

        public ServiceBusConfiguration TenantsServiceBusConfiguration { get; private set; }

        public ServiceBusConfiguration UsersServiceBusConfiguration { get; private set; }

        public ITenantDataService TenantDataService { get; private set; }

        public IUserDataService UserDataService { get; private set; }

        public async Task Start()
        {
            if (this.isStarted)
            {
                return;
            }

            using (await this.locker.LockAsync())
            {
                if (this.isStarted)
                {
                    return;
                }

                await this.EnsureTopics();
                await this.EnsureSubscriptions();
                this.isStarted = true;
            }
        }

        public async Task Stop()
        {
            if (!this.isStarted)
            {
                return;
            }

            using (await this.locker.LockAsync())
            {
                if (!this.isStarted)
                {
                    return;
                }

                await this.tenantsSubscriptionClient.CloseAsync();
                await this.tenantsUpdatableDataManagerTopicClient.CloseAsync();

                await this.usersSubscriptionClient.CloseAsync();
                await this.usersUpdatableDataManagerTopicClient.CloseAsync();
                this.isStarted = false;
            }
        }

        private async Task OnTenantsMessage(BrokeredMessage message)
        {
            var delta = message.Deserialize<TenantDelta>();
            if (delta.Name != null)
            {
                Logger.Debug("Name changed to '{0}'", delta.Name.Value);
            }

            try
            {
                var tenant = await this.TenantDataService.Get(delta.Id);
                var updatedTenant = delta.Apply(tenant);
                await this.TenantDataService.Update(updatedTenant);
            }
            catch (Exception exception)
            {
                Logger.Error("Exception while updating tenant", exception.Message);
                return;
            }

            await message.CompleteAsync();
            var reply = new BrokeredMessage { SessionId = message.ReplyToSessionId };
            reply.Properties["Succeeded"] = true;
            await this.tenantsUpdatableDataManagerTopicClient.SendAsync(reply);
            Logger.Info("Sent reply");
        }

        private async Task OnUsersMessage(BrokeredMessage message)
        {
            var delta = message.Deserialize<TenantDelta>();
            if (delta.Name != null)
            {
                Logger.Debug("Name changed to '{0}'", delta.Name.Value);
            }

            try
            {
                var tenant = await this.TenantDataService.Get(delta.Id);
                var updatedTenant = delta.Apply(tenant);
                await this.TenantDataService.Update(updatedTenant);
            }
            catch (Exception exception)
            {
                Logger.Error("Exception while updating tenant", exception.Message);
                return;
            }

            await message.CompleteAsync();
            var reply = new BrokeredMessage { SessionId = message.ReplyToSessionId };
            reply.Properties["Succeeded"] = true;
            await this.tenantsUpdatableDataManagerTopicClient.SendAsync(reply);
            Logger.Info("Sent reply");
        }

        private async Task EnsureSubscriptions()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            this.tenantsSubscriptionClient =
                await
                this.EnsureSubscription(
                    namespaceManager,
                    connectionString,
                    this.TenantsServiceBusConfiguration,
                    this.OnTenantsMessage);
            this.usersSubscriptionClient =
                await
                this.EnsureSubscription(
                    namespaceManager,
                    connectionString,
                    this.UsersServiceBusConfiguration,
                    this.OnUsersMessage);
        }

        private async Task<SubscriptionClient> EnsureSubscription(
            NamespaceManager namespaceManager,
            string connectionString,
            ServiceBusConfiguration serviceBusConfiguration,
            Func<BrokeredMessage, Task> onMessage)
        {
            if (
                !(await
                  namespaceManager.SubscriptionExistsAsync(
                      serviceBusConfiguration.TopicName,
                      serviceBusConfiguration.SubscriptionName)))
            {
                var filter = new SqlFilter("[sys].ReplyToSessionId <> NULL");
                var subscription =
                    await
                    namespaceManager.CreateSubscriptionAsync(
                        serviceBusConfiguration.TopicName,
                        serviceBusConfiguration.SubscriptionName,
                        filter);
            }

            var client = SubscriptionClient.CreateFromConnectionString(
                connectionString,
                serviceBusConfiguration.TopicName,
                serviceBusConfiguration.SubscriptionName);
            client.OnMessageAsync(onMessage);
            return client;
        }

        private async Task EnsureTopics()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            var serviceBusConfiguration = this.TenantsServiceBusConfiguration;
            this.tenantsUpdatableDataManagerTopicClient =
                await this.EnsureTopic(namespaceManager, serviceBusConfiguration, connectionString);
            this.usersUpdatableDataManagerTopicClient =
                await this.EnsureTopic(namespaceManager, serviceBusConfiguration, connectionString);
        }

        private async Task<TopicClient> EnsureTopic(
            NamespaceManager namespaceManager,
            ServiceBusConfiguration serviceBusConfiguration,
            string connectionString)
        {
            if (!(await namespaceManager.TopicExistsAsync(serviceBusConfiguration.TopicName)))
            {
                await namespaceManager.CreateTopicAsync(serviceBusConfiguration.TopicName);
            }

            return TopicClient.CreateFromConnectionString(
                connectionString,
                serviceBusConfiguration.TopicName);
        }
    }
}
