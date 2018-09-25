// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpUpdatePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpUpdatePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The FTP update part controller.
    /// </summary>
    public class FtpUpdatePartController : UpdatePartControllerBase
    {
        private const string FtpServersKeyStart = "Servers.";

        private const string PollIntervalKey = "PollInterval";

        private MultiSelectEditorViewModel ftpServers;

        private TimeSpanEditorViewModel ftpUpdateIntervall;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpUpdatePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public FtpUpdatePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.Ftp, parent)
        {
        }

        /// <summary>
        /// Gets the text mode.
        /// </summary>
        public TimeSpan FtpUpdateIntervall
        {
            get
            {
                return this.ftpUpdateIntervall.Value.HasValue
                    ? this.ftpUpdateIntervall.Value.Value
                    : TimeSpan.FromMinutes(5);
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            base.Load(partData);

            this.ftpServers.Options.Clear();
            foreach (var provider in this.MethodsController.GetFtpUpdateProviders())
            {
                this.ftpServers.Options.Add(new CheckableOptionViewModel(GetLabel(provider), provider));
            }

            var values = partData.Values.Where(v => v.Key.StartsWith(FtpServersKeyStart)).ToList();
            foreach (var option in this.ftpServers.Options)
            {
                option.IsChecked = !values.Any() || values.Any(v => v.Value.Equals(option.Label));
            }

            var defaultConfig = TimeSpan.FromMinutes(5);
            this.ftpUpdateIntervall.Value = partData.GetValue(defaultConfig, PollIntervalKey);

            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            base.Save(partData);

            for (int i = 0; i < this.ftpServers.Options.Count; i++)
            {
                partData.SetValue(this.ftpServers.Options[i].Label, FtpServersKeyStart + i);
            }

            partData.SetValue(this.ftpUpdateIntervall.Value, PollIntervalKey);
        }

        /// <summary>
        /// Gets the list of selected FTP update providers.
        /// </summary>
        /// <returns>
        /// The FTP update providers selected by the user.
        /// </returns>
        public IEnumerable<FtpUpdateProviderConfig> GetSelectedProviders()
        {
            return this.ftpServers.GetCheckedValues().Cast<FtpUpdateProviderConfig>();
        }

        /// <summary>
        /// Updates the visibility of the view model of this part controller.
        /// </summary>
        protected override void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.MethodsController.HasFtpChecked;
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
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_FTP;
            viewModel.Description = AdminStrings.UnitConfig_Update_FTP_Description;

            this.ftpServers = new MultiSelectEditorViewModel();
            this.ftpServers.Label = AdminStrings.UnitConfig_Update_FTP_Servers;
            viewModel.Editors.Add(this.ftpServers);

            this.ftpUpdateIntervall = new TimeSpanEditorViewModel();
            this.ftpUpdateIntervall.Label = AdminStrings.UnitConfig_Update_FTP_UpdateInterval;
            viewModel.Editors.Add(this.ftpUpdateIntervall);

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

        private static string GetLabel(FtpUpdateProviderConfig provider)
        {
            return string.Format(
                "ftp://{0}@{1}:{2}/{3}",
                provider.Username,
                provider.Host,
                provider.Port,
                provider.RepositoryBasePath);
        }

        private void UpdateErrors()
        {
            this.ftpServers.SetError(
                "Options",
                this.ftpServers.GetCheckedOptions().Any() ? ErrorState.Ok : ErrorState.Error,
                AdminStrings.Errors_SelectOneAtLeast);
        }
    }
}