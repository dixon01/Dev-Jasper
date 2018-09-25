namespace Library.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Library.Model;
    using Library.ServiceModel;

    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    using Nito.AsyncEx;

    using NLog;

    public class UpdatableTenantDataServiceProxy : IDisposable
    {
        public ClientServiceBusConfiguration ServiceBusConfiguration { get; set; }

        private const string TopicName = "Tenants";

        private const string ConnectionStringName = "ServiceBus";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string sessionId = Guid.NewGuid().ToString("N");

        private readonly AsyncLock locker = new AsyncLock();

        private readonly ConcurrentDictionary<object, TenantDataViewModel> tenants =
            new ConcurrentDictionary<object, TenantDataViewModel>();

        private bool isStarted;

        private SubscriptionClient subscriptionClient;

        private TopicClient topicClient;

        private TaskCompletionSource<bool> wait = new TaskCompletionSource<bool>();

        public UpdatableTenantDataServiceProxy(
            ClientServiceBusConfiguration serviceBusConfiguration,
            string applicationName)
        {
            this.ServiceBusConfiguration = serviceBusConfiguration;
            this.ApplicationName = string.Format("{0}_{1}", "WpfApplication", this.sessionId);
        }

        public string ApplicationName { get; private set; }

        public TenantDataViewModel Get(int id)
        {
            var tenantDataViewModel = new TenantDataViewModel(this.sessionId, id);
            return tenantDataViewModel;
        }

        public async Task<IEnumerable<TenantDataViewModel>> List()
        {
            var list = (await this.GetTenants()).ToList();
            list.ForEach(tenant => this.tenants.AddOrUpdate(tenant.Id, id => tenant, this.UpdateValueFactory));
            return list;
        }

        private TenantDataViewModel UpdateValueFactory(object id, TenantDataViewModel tenantDataViewModel)
        {
            return tenantDataViewModel;
        }

        private async Task<IEnumerable<TenantDataViewModel>> GetTenants()
        {
            var channelFactory = new ChannelFactory<ITenantDataService>(
                new NetTcpBinding(SecurityMode.None),
                "net.tcp://localhost:9999/TenantDataService");
            var channel = channelFactory.CreateChannel();
            var tenantsList = (await channel.List()).Select(this.Convert).ToList();
            (channel as ICommunicationObject).Close();
            return tenantsList;
        }

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

                await this.EnsureTopic();
                await this.EnsureSubscription();
                this.isStarted = true;
            }
        }

        public void Dispose()
        {
        }

        private TenantDataViewModel Convert(Tenant tenant)
        {
            return new TenantDataViewModel(this.sessionId, tenant.Id) {Name = tenant.Name};
        }

        public async Task SendDelta(TenantDelta tenantDelta)
        {
            var stream = tenantDelta.Serialize();
            var message = new BrokeredMessage(stream, true) { ReplyToSessionId = this.sessionId };
            await this.topicClient.SendAsync(message).ConfigureAwait(false);
            await this.wait.Task.ConfigureAwait(false);
        }

        private async Task OnMessage(BrokeredMessage message)
        {
            if (message.SessionId == this.sessionId)
            {
                this.HandleTransactionalReply(message);
                await message.CompleteAsync();
                return;
            }

            var delta = message.Deserialize<TenantDelta>();
            TenantDataViewModel tenant;
            await message.CompleteAsync();
            if (!this.tenants.TryGetValue(delta.Id, out tenant))
            {
                return;
            }

            tenant.Apply(delta);
        }

        private void HandleTransactionalReply(BrokeredMessage message)
        {
            try
            {
                var result = (bool)message.Properties["Succeeded"];
                var w = this.wait;
                this.wait = new TaskCompletionSource<bool>();
                var x = w.TrySetResult(result);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task EnsureTopic()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!(await namespaceManager.TopicExistsAsync(this.ServiceBusConfiguration.TopicName)))
            {
                await namespaceManager.CreateTopicAsync(this.ServiceBusConfiguration.TopicName);
            }

            this.topicClient = TopicClient.CreateFromConnectionString(
                connectionString,
                this.ServiceBusConfiguration.TopicName);
            Logger.Info("Created topic '{0}'", this.ServiceBusConfiguration.TopicName);
        }

        private async Task EnsureSubscription()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!(await namespaceManager.SubscriptionExistsAsync(TopicName, this.ApplicationName)))
            {
                var sqlExpression = "[sys].SessionId = '" + this.sessionId
                                    + "' OR ([sys].ReplyToSessionId != NULL AND [sys].ReplyToSessionId != '"
                                    + this.sessionId + "')";
                var subscription =
                    await
                    namespaceManager.CreateSubscriptionAsync(
                        TopicName,
                        this.ApplicationName,
                        new SqlFilter(sqlExpression));
                subscription.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                await namespaceManager.UpdateSubscriptionAsync(subscription);
            }

            this.subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                connectionString,
                this.ServiceBusConfiguration.TopicName,
                this.ApplicationName);
            Logger.Info(
                "Created subscription '{0}' on topic '{1}'",
                this.ServiceBusConfiguration.TopicName,
                this.ApplicationName);
            this.subscriptionClient.OnMessageAsync(this.OnMessage);
        }
    }
}