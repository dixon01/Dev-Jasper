// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableAzureUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnableAzureUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Configuration
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.PowerShell.Base;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;

    /// <summary>
    /// Configures the Azure update provider.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, CmdletNouns.AzureUpdateProvider)]
    public class EnableAzureUpdateProvider : AsyncDataServiceCmdletBase
    {
        /// <summary>
        /// Gets or sets the retry interval to send update commands.
        /// </summary>
        [Parameter(Mandatory = true)]
        public TimeSpan RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the name. If not set, the default name &quot;AzureUpdateProvider&quot; will be used.
        /// </summary>
        [Parameter]
        public string Name { get; set; }

        /// <summary>
        /// Gets the name of the entity.
        /// </summary>
        public override string EntityName
        {
            get
            {
                return CmdletNouns.SystemConfig;
            }
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ProcessRecordAsync()
        {
            using (var scope = this.CreateDataChannelScope<ISystemConfigDataService>())
            {
                var config =
                    (await
                     scope.Channel.QueryAsync(SystemConfigQuery.Create().IncludeSettings().OrderBySystemIdDescending()))
                        .First();
                var settings = (BackgroundSystemSettings)config.Settings.Deserialize();
                var name = string.IsNullOrEmpty(this.Name) ? CmdletNouns.AzureUpdateProvider : this.Name;
                settings.AzureUpdateProvider = new AzureUpdateProviderConfig
                {
                    Name = name,
                    RetryInterval = this.RetryInterval
                };
                config.Settings = new XmlData(settings);
                this.WriteObject(await scope.Channel.UpdateAsync(config));
            }
        }
    }
}