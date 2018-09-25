namespace Library.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Library.Model;
    using Library.Server;
    using Library.ServiceModel;
    using Library.Tracking;

    using Microsoft.ServiceBus.Messaging;

    using Nito.AsyncEx;

    public class TenantTrackingProxy : TrackingProxyBase, ITenantTrackingService
    {
        private readonly Dictionary<int, TenantReadableModel> tenants = new Dictionary<int, TenantReadableModel>();

        private bool isStarted;

        public TenantTrackingProxy(TrackingProxyConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<TenantReadableModel> Get(int id)
        {
            var channel = Utility.CreateChannel<ITenantTrackingService, Tenant>();
            var model = await channel.Get(id);
            Utility.CloseChannel(channel);
            return model;
        }

        public async Task<IEnumerable<TenantReadableModel>> List()
        {
            var channel = Utility.CreateChannel<ITenantTrackingService, Tenant>();
            var tenants = (await channel.List()).ToList();
            tenants.ForEach(this.Action);
            Utility.CloseChannel(channel);
            return tenants;
        }

        protected override void OnStarted()
        {
            this.subscriptionClient.OnMessageAsync(this.OnReply);
        }

        private void Action(TenantReadableModel tenantReadableModel)
        {
            this.tenants[tenantReadableModel.Id] = tenantReadableModel;
            tenantReadableModel.WritableModelCreated += this.TenantReadableModelOnWritableModelCreated;
        }

        private void TenantReadableModelOnWritableModelCreated(object sender, WritableModelCreatedEventArgs writableModelCreatedEventArgs)
        {
            writableModelCreatedEventArgs.TrackingModel.Updated += this.TrackingModelOnUpdated;
        }

        private async void TrackingModelOnUpdated(object sender, WritableModelUpdatedEventArgs writableModelUpdatedEventArgs)
        {
            await this.SendDelta(writableModelUpdatedEventArgs.Delta);
        }

        private async Task OnReply(BrokeredMessage message)
        {
            await this.SetConfirmation(true).ConfigureAwait(false);
            var delta = message.GetBody<TenantDelta>();
            this.tenants[delta.Id].Apply(delta);
            await message.CompleteAsync();
        }
    }

    public class TrackingProxyBase : IDisposable
    {
        private readonly AsyncLock asyncLock = new AsyncLock();

        private TopicClient topicClient;

        protected SubscriptionClient subscriptionClient;

        private bool isStarted;

        private TaskCompletionSource<bool> wait = new TaskCompletionSource<bool>();

        public TrackingProxyBase(TrackingProxyConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public TrackingProxyConfiguration Configuration { get; set; }

        public void Dispose()
        {
            if (!this.isStarted)
            {
                return;
            }

            lock (this.asyncLock)
            {
                if (!this.isStarted)
                {
                    return;
                }

                AsyncContext.Run(() => this.subscriptionClient.CloseAsync());
                AsyncContext.Run(() => this.topicClient.CloseAsync());
            }
        }

        protected async Task SendDelta(Delta delta)
        {
            var message = new BrokeredMessage(delta) { ReplyToSessionId = this.Configuration.SessionId };
            await this.topicClient.SendAsync(message).ConfigureAwait(false);
            var delay = Task.Delay(this.Configuration.Timeout);
            var waitedTask = this.wait.Task;
            waitedTask.ConfigureAwait(false);
            var task = await Task.WhenAny(delay, waitedTask);
            if (task != waitedTask)
            {
                throw new TimeoutException("Timeout while waiting for confirmation");
            }

            if (!waitedTask.Result)
            {
                throw new Exception("Change not confirmed");
            }
        }

        public async Task Start()
        {
            if (this.isStarted)
            {
                return;
            }

            using (await this.asyncLock.LockAsync())
            {
                if (this.isStarted)
                {
                    return;
                }

                this.topicClient = await this.Configuration.EnsureTopic();
                var sqlFilter =
                    string.Format(
                        "[sys].ReplyToSessionId = NULL");
                this.subscriptionClient =
                    await
                    this.Configuration.EnsureSubscription(sqlFilter);
                this.OnStarted();
                this.isStarted = true;
            }
        }

        protected virtual void OnStarted()
        {
        }

        protected virtual async Task SetConfirmation(bool result)
        {
            var w = this.wait;
            this.wait = new TaskCompletionSource<bool>();
            w.SetResult(result);
        }
    }

    public class TrackingProxyConfiguration : TrackingServiceConfiguration
    {
        public string SessionId { get; set; }
    }

    public class TrackingServiceConfiguration
    {
        public string ApplicationName { get; set; }

        public string ConnectionString { get; set; }

        public string TopicName { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}