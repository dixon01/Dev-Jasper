namespace Library.Server
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using Library.Client;
    using Library.Model;
    using Library.ServiceModel;
    using Library.Tracking;

    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure;

    using Nito.AsyncEx;

    using NLog;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class TenantTrackingService : ITenantTrackingService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, Tenant> tenants = new Dictionary<int, Tenant>
                                                               {
                                                                   {
                                                                       1, new Tenant
                                                                              {
                                                                                  Changeset = new Changeset(1),
                                                                                  Description = "Tenant 1 description",
                                                                                  Id = 1,
                                                                                  Name = "Tenant 1"
                                                                              }
                                                                   },
                                                                   {
                                                                       2, new Tenant
                                                                              {
                                                                                  Changeset = new Changeset(1),
                                                                                  Description = "Tenant 2 description",
                                                                                  Id = 2,
                                                                                  Name = "Tenant 2"
                                                                              }
                                                                   }
                                                               };

        private readonly AsyncLock asyncLock = new AsyncLock();

        private SubscriptionClient subscriptionClient;

        private TopicClient topicClient;

        private bool isStarted;

        public async Task<TenantReadableModel> Get(int id)
        {
            await Task.FromResult(0);
            return this.Convert(this.tenants[id]);
        }

        public async Task<IEnumerable<TenantReadableModel>> List()
        {
            await Task.FromResult(0);
            return this.tenants.Values.Select(this.Convert);
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

                var connectionString = CloudConfigurationManager.GetSetting("ServiceBus");
                var configuration = new TrackingServiceConfiguration
                                        {
                                            ApplicationName = "Host",
                                            ConnectionString = connectionString,
                                            Timeout = TimeSpan.FromSeconds(10),
                                            TopicName = "Tenants",
                                        };
                Logger.Debug("Subscribing to ServiceBus...");
                this.topicClient = await configuration.EnsureTopic();
                this.subscriptionClient = await configuration.EnsureSubscription("[sys].[ReplyToSessionId] != NULL");
                this.subscriptionClient.OnMessageAsync(this.OnMessage);

                this.isStarted = true;
                Logger.Info("Started");
            }
        }

        private async Task OnMessage(BrokeredMessage message)
        {
            var delta = message.GetBody<TenantDelta>();
            Logger.Trace("Received delta for tenant {0}", delta.Id);
            if (delta.Name != null)
            {
                Logger.Trace("Changed name for tenant {0} to value '{1}'", delta.Id, delta.Name.Value);
                this.tenants[delta.Id].Name = delta.Name.Value;
            }

            if (delta.Description != null)
            {
                Logger.Trace("Changed description for tenant {0} to value '{1}'", delta.Id, delta.Description.Value);
                this.tenants[delta.Id].Description = delta.Description.Value;
            }

            this.tenants[delta.Id].LastModifiedOn = DateTime.Now;
            delta.IncrementChangeset();
            var reply = new BrokeredMessage(delta);
            //reply.SessionId = message.ReplyToSessionId;

            await message.CompleteAsync();
            await Task.Delay(TimeSpan.FromSeconds(1));

            Logger.Trace("Sending delta to all registered applications");
            await this.topicClient.SendAsync(reply);
        }

        private TenantReadableModel Convert(Tenant tenant)
        {
            return new TenantReadableModel(tenant);
        }
    }
}