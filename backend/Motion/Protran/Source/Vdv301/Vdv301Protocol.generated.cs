

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Protran.VDV301;
using Gorba.Common.Protocols.Vdv301.Messages;
using Gorba.Common.Protocols.Vdv301.Services;
using Gorba.Common.Protocols.Ximple;

using Gorba.Common.Utility.Core;

using Gorba.Motion.Common.IbisIP.Client;
using Gorba.Motion.Common.IbisIP.Discovery;

using Gorba.Motion.Protran.Vdv301.Handlers;

using NLog;

namespace Gorba.Motion.Protran.Vdv301
{
    public partial class Vdv301Protocol : IHandlerContext
    {
        ////////////////////// CustomerInformationService //////////////////////

        private ICustomerInformationService proxyCustomerInformationService;
        private CustomerInformationServiceHandler handlerCustomerInformationService;
        private IServiceQuery<ICustomerInformationService> queryCustomerInformationService;
        
        public event EventHandler CustomerInformationServiceChanged;
        
        public ICustomerInformationService CustomerInformationService
        {
            get
            {
                return this.proxyCustomerInformationService;
            }

            private set
            {
                this.proxyCustomerInformationService = value;
                this.RaiseCustomerInformationServiceChanged(EventArgs.Empty);
            }
        }

        private void InitializeCustomerInformationService()
        {
            var cfg = this.Config.Services.CustomerInformationService;
            var staticAddress = !string.IsNullOrEmpty(cfg.Host) && cfg.Port != 0;

            if (staticAddress)
            {
                this.CustomerInformationService =
                    ServiceClientProxyFactory.Create<ICustomerInformationService>(
                        cfg.Host, cfg.Port, cfg.Path, this.callbackServer);
            }

            this.handlerCustomerInformationService = new CustomerInformationServiceHandler();
            this.handlerCustomerInformationService.DataUpdated += this.ServiceHandlerOnDataUpdated;
            this.handlerCustomerInformationService.XimpleCreated += this.ServiceHandlerOnXimpleCreated;
            this.handlerCustomerInformationService.Configure(this);
            this.handlerCustomerInformationService.Start();
            
            if (!staticAddress)
            {
                this.queryCustomerInformationService = this.serviceLocator.QueryServices<ICustomerInformationService>();
                this.queryCustomerInformationService.ServicesChanged += this.QueryCustomerInformationServiceOnServicesChanged;
                this.queryCustomerInformationService.Start();
            }
        }

        private void QueryCustomerInformationServiceOnServicesChanged(object sender, EventArgs e)
        {
            var query = this.queryCustomerInformationService;
            if (query == null)
            {
                return;
            }

            if (this.CustomerInformationService != null && Array.IndexOf(query.Services, this.CustomerInformationService) < 0)
            {
                this.CustomerInformationService = null;
            }

            if (this.CustomerInformationService == null && query.Services.Length > 0)
            {
                this.CustomerInformationService = query.Services[0];
            }
        }

        private void DeinitializeCustomerInformationService()
        {
            if (this.queryCustomerInformationService != null)
            {
                this.queryCustomerInformationService.ServicesChanged -= this.QueryCustomerInformationServiceOnServicesChanged;
                this.queryCustomerInformationService.Stop();
                this.queryCustomerInformationService = null;
            }
            
            this.handlerCustomerInformationService.DataUpdated -= this.ServiceHandlerOnDataUpdated;
            this.handlerCustomerInformationService.XimpleCreated -= this.ServiceHandlerOnXimpleCreated;
            this.handlerCustomerInformationService.Stop();
        }
        
        protected virtual void RaiseCustomerInformationServiceChanged(EventArgs e)
        {
            var handler = this.CustomerInformationServiceChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    
        private void ServiceHandlerOnDataUpdated(object sender, DataUpdateEventArgs<object> e)
        {
            this.RaiseDataUpdated(e);
        }

        private void ServiceHandlerOnXimpleCreated(object sender, XimpleEventArgs e)
        {
            this.RaiseXimpleCreated(e);
        }
    }

    public interface IHandlerContext : IHandlerConfigContext
    {
        event EventHandler CustomerInformationServiceChanged;

        ICustomerInformationService CustomerInformationService { get; }
    }
}

