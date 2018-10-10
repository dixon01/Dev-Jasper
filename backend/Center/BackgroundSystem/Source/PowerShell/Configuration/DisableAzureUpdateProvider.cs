// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableAzureUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisableAzureUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.PowerShell.Configuration
{
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.PowerShell.Base;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;

    /// <summary>
    /// Disables the Azure update provider.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, CmdletNouns.AzureUpdateProvider)]
    public class DisableAzureUpdateProvider : AsyncDataServiceCmdletBase
    {
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
                if (settings.AzureUpdateProvider == null)
                {
                    this.WriteObject(config);
                    return;
                }

                settings.AzureUpdateProvider = null;
                config.Settings = new XmlData(settings);
                this.WriteObject(await scope.Channel.UpdateAsync(config));
            }
        }
    }
}