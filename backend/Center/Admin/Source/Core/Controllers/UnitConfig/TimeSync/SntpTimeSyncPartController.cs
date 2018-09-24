// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpTimeSyncPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpTimeSyncPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareManager;

    /// <summary>
    /// The part controller for time synchronization using SNTP (directly, not via VDV 301).
    /// </summary>
    public class SntpTimeSyncPartController : SntpTimeSyncPartControllerBase<SntpConfig>
    {
        private EditableSelectionEditorViewModel hostName;

        private NumberEditorViewModel portNumber;

        private TimeSpanEditorViewModel updateInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpTimeSyncPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public SntpTimeSyncPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.TimeSync.Sntp, IncomingData.Sntp, parent)
        {
        }

        /// <summary>
        /// Gets the SNTP configuration.
        /// </summary>
        /// <returns>
        /// The configuration.
        /// </returns>
        public override SntpConfig GetConfig()
        {
            var config = base.GetConfig();
            config.Host = this.hostName.Value;
            config.Port = (int)this.portNumber.Value;
            if (this.updateInterval.Value.HasValue)
            {
                config.UpdateInterval = this.updateInterval.Value.Value;
            }

            return config;
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_TimeSync_Sntp;
            viewModel.Description = AdminStrings.UnitConfig_TimeSync_Sntp_Description;

            var index = 0;
            this.hostName = new EditableSelectionEditorViewModel();
            viewModel.Editors.Insert(index++, this.hostName);
            this.hostName.Label = AdminStrings.UnitConfig_TimeSync_Sntp_Host;
            foreach (var server in GetSntpServers())
            {
                this.hostName.Options.Add(server.HostNameOrAddress);
            }

            this.portNumber = new NumberEditorViewModel();
            viewModel.Editors.Insert(index++, this.portNumber);
            this.portNumber.Label = AdminStrings.UnitConfig_TimeSync_Sntp_Port;
            this.portNumber.IsInteger = true;
            this.portNumber.MinValue = 1;
            this.portNumber.MaxValue = ushort.MaxValue;

            this.updateInterval = new TimeSpanEditorViewModel();
            viewModel.Editors.Insert(index, this.updateInterval);
            this.updateInterval.Label = AdminStrings.UnitConfig_TimeSync_Sntp_UpdateInterval;
            this.updateInterval.IsNullable = false;

            this.UpdateErrors();
            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);

            this.UpdateErrors();
        }

        /// <summary>
        /// Loads the data from the given <paramref name="config"/> into the view model.
        /// </summary>
        /// <param name="config">
        /// The config to load the data from.
        /// </param>
        protected override void LoadFrom(SntpConfig config)
        {
            base.LoadFrom(config);
            this.hostName.Value = config.Host;
            this.portNumber.Value = config.Port;
            this.updateInterval.Value = config.UpdateInterval >= TimeSpan.FromDays(1)
                                            ? TimeSpan.FromDays(1).Add(TimeSpan.FromMilliseconds(-1))
                                            : config.UpdateInterval;
        }

        private static IEnumerable<RemoteSntpServer> GetSntpServers()
        {
            var fields = typeof(RemoteSntpServer).GetFields(BindingFlags.Public | BindingFlags.Static);
            return fields.SelectMany(f =>
                {
                    if (f.FieldType == typeof(RemoteSntpServer))
                    {
                        return new[] { (RemoteSntpServer)f.GetValue(null) };
                    }

                    if (typeof(IEnumerable<RemoteSntpServer>).IsAssignableFrom(f.FieldType))
                    {
                        return (IEnumerable<RemoteSntpServer>)f.GetValue(null);
                    }

                    return Enumerable.Empty<RemoteSntpServer>();
                });
        }

        private void UpdateErrors()
        {
            if (this.hostName == null || this.ViewModel == null)
            {
                return;
            }

            var missingHost = string.IsNullOrWhiteSpace(this.hostName.Value);
            this.hostName.SetError(
                "Value",
                missingHost ? (this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing) : ErrorState.Ok,
                AdminStrings.Errors_TextNotWhitespace);
        }
    }
}