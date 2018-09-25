namespace WpfApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Library.Client;
    using Library.Tracking;
    using Library.ViewModel;

    using Microsoft.WindowsAzure;

    using Nito.AsyncEx;

    public class TenantsController
    {
        private static readonly Lazy<TenantsController> LazyInstance =
            new Lazy<TenantsController>(() => new TenantsController());

        private readonly AsyncLazy<TenantTrackingProxy> lazyProxy;

        private readonly Lazy<string> lazySessionId = new Lazy<string>(CreateSessionId);

        private static string CreateSessionId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private async Task<TenantTrackingProxy> CreateProxy()
        {
            var configuration = new TrackingProxyConfiguration
                                    {
                                        ApplicationName = "WpfApplication",
                                        ConnectionString =
                                            CloudConfigurationManager.GetSetting("ServiceBus"),
                                        SessionId = this.SessionId,
                                        Timeout = TimeSpan.FromSeconds(15),
                                        TopicName = "Tenants"
                                    };
            var proxy = new TenantTrackingProxy(configuration);
            await proxy.Start();
            return proxy;
        }

        private TenantsController()
        {
            this.lazyProxy = new AsyncLazy<TenantTrackingProxy>(() => this.CreateProxy());
        }

        public string SessionId
        {
            get
            {
                return this.lazySessionId.Value;
            }
        }

        public static TenantsController Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        public TenantTrackingProxy Proxy
        {
            get
            {
                return this.lazyProxy.GetAwaiter().GetResult();
            }
        }

        public async Task Load()
        {
            var tenantTrackingProxy = await this.lazyProxy;
            var tenants = (await this.Proxy.List()).Select(this.Convert);
            Shell.Instance.Tenants.Clear();
            tenants.ToList().ForEach(Shell.Instance.Tenants.Add);
        }

        public void Edit()
        {
            var writableModel = Shell.Instance.SelectedTenant.Model.ToWritable();
            
            Shell.Instance.EditingTenant = this.Convert(writableModel);
        }

        public void Update()
        {
            Shell.Instance.EditingTenant.Model.Update();
            Shell.Instance.EditingTenant = null;
        }

        private ReadOnlyTenantDataViewModel Convert(TenantReadableModel model)
        {
            return new ReadOnlyTenantDataViewModel(model);
        }

        private TenantDataViewModel Convert(TenantWritableModel model)
        {
            return new TenantDataViewModel(model);
        }
    }
}