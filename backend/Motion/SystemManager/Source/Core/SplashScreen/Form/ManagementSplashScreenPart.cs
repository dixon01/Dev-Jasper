// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementSplashScreenPart.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementSplashScreenPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.SplashScreen.Form
{
    using System;
    using System.Threading;

    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core.Management.Remote;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Splash screen part that shows the current value of a management property.
    /// </summary>
    public class ManagementSplashScreenPart : TextSplashScreenPartBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ManagementSplashScreenPart>();

        private readonly ManagementSplashScreenItem config;

        private readonly ITimer updateTimer;

        private IManagementObjectProvider provider;

        private string propertyName;

        private ManagementProperty property;

        private bool loadProviderFailed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementSplashScreenPart"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public ManagementSplashScreenPart(ManagementSplashScreenItem config)
        {
            this.config = config;

            if (string.IsNullOrEmpty(config.Path))
            {
                Logger.Warn("Path is not set");
                return;
            }

            if (config.UpdateInterval.HasValue && config.UpdateInterval.Value > TimeSpan.Zero)
            {
                this.updateTimer = TimerFactory.Current.CreateTimer("ManagementPartRefresh-" + config.Path);
                this.updateTimer.AutoReset = true;
                this.updateTimer.Interval = config.UpdateInterval.Value;
                this.updateTimer.Elapsed += (s, e) => this.RefreshValue();
            }

            Logger.Debug("ManagementSplashScreenPart() is calling StartLoadProvider.");
            this.StartLoadProvider();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            var remote = this.provider as IRemoteManagementProvider;
            if (remote != null)
            {
                // only dispose the provider if it's a remote provider (otherwise we actually modify the local tree)
                remote.Dispose();
            }

            if (this.updateTimer != null)
            {
                this.updateTimer.Dispose();
            }
        }

        /// <summary>
        /// Calculates the string to be shown on the splash screen.
        /// This string can contain multiple lines.
        /// </summary>
        /// <returns>
        /// The string to be shown on the splash screen.
        /// </returns>
        protected override string GetDisplayString()
        {
            if (this.loadProviderFailed)
            {
                // the initial loading failed, let's try again
                Logger.Debug("GetDisplayString() is calling StartLoadProvider.");
                this.StartLoadProvider();
            }

            var label = string.IsNullOrEmpty(this.config.Label) ? this.propertyName : this.config.Label;
            var value = this.property == null ? null : this.property.StringValue;

            return string.Format(
                "{0}: {1}",
                label,
                this.property == null ? "n/a" : string.Format(this.config.ValueFormat, value));
        }

        private void StartLoadProvider()
        {
            this.loadProviderFailed = false;
            if (this.updateTimer != null)
            {
                this.updateTimer.Enabled = false;
            }

            ThreadPool.QueueUserWorkItem(s => this.LoadProvider());
        }

        private void LoadProvider()
        {
            IManagementProvider root;
            if (!string.IsNullOrEmpty(this.config.Application))
            {
                var unit = string.IsNullOrEmpty(this.config.Unit)
                               ? MessageDispatcher.Instance.LocalAddress.Unit
                               : this.config.Unit;
                var address = new MediAddress(unit, this.config.Application);

                // ping the address until we get a response, otherwise the management requests will time out
                using (var pinger = new Pinger(MessageDispatcher.Instance))
                {
                    while (pinger.Ping(address, 500) == -1)
                    {
                        Logger.Trace("Coudln't find {0}, trying to ping it again", address);
                    }
                }

                root = MessageDispatcher.Instance.ManagementProviderFactory.CreateRemoteProvider(address);
            }
            else
            {
                root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            }

            var path = this.config.Path.Trim('\\').Split('\\');
            if (path.Length < 2)
            {
                Logger.Warn("Path is too short: {0}", this.config.Path);
                return;
            }

            var parent = new string[path.Length - 1];
            Array.Copy(path, parent, parent.Length);
            this.propertyName = path[path.Length - 1];

            try
            {
                this.provider = root.GetDescendant(parent) as IManagementObjectProvider;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't get object: \\" + string.Join("\\", parent));
                this.loadProviderFailed = true;
                return;
            }

            if (this.provider == null)
            {
                Logger.Warn("Object not found: \\{0}", string.Join("\\", parent));
                this.loadProviderFailed = true;
                return;
            }

            try
            {
                this.UpdateProperty();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't load property");
                this.loadProviderFailed = true;
                return;
            }

            if (this.property == null)
            {
                Logger.Warn("Property not found: {0}", this.config.Path);
                this.loadProviderFailed = true;
                return;
            }

            if (this.updateTimer != null)
            {
                this.updateTimer.Enabled = true;
            }
        }

        private void RefreshValue()
        {
            var remote = this.provider as IRemoteManagementProvider;
            if (remote != null)
            {
                remote.BeginReload(this.ProviderReloaded, null);
            }
            else
            {
                this.UpdateProperty();
            }
        }

        private void UpdateProperty()
        {
            if (this.provider != null && this.propertyName != null)
            {
                this.property = this.provider.GetProperty(this.propertyName);
            }
        }

        private void ProviderReloaded(IAsyncResult ar)
        {
            try
            {
                var remote = (IRemoteManagementProvider)this.provider;
                remote.EndReload(ar);
                this.UpdateProperty();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't reload remote provider, recreating it");
                Logger.Debug("ProviderReloaded() is calling StartLoadProvider.");
                this.StartLoadProvider();
            }
        }
    }
}