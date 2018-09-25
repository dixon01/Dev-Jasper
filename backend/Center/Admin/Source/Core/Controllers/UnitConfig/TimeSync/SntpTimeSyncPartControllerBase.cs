// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpTimeSyncPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpTimeSyncPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync
{
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager;

    /// <summary>
    /// Base class for time sync controllers that are configuring a <see cref="SntpConfigBase"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The implementation type of <see cref="SntpConfigBase"/> to be created by this controller.
    /// </typeparam>
    public abstract class SntpTimeSyncPartControllerBase<T> : MultiEditorPartControllerBase
        where T : SntpConfigBase, new()
    {
        private const string SntpConfigKey = "SntpConfig";

        private readonly IncomingData type;

        private SelectionEditorViewModel versionNumber;

        private TimeSpanEditorViewModel retryInterval;

        private NumberEditorViewModel retryCount;

        private TimeSourcePartController timeSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpTimeSyncPartControllerBase{T}"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="type">
        /// The type of incoming data that is being handled by this controller,
        /// this should be only a single value, not a combination of flags.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected SntpTimeSyncPartControllerBase(string key, IncomingData type, CategoryControllerBase parent)
            : base(key, parent)
        {
            this.type = type;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public sealed override void Load(UnitConfigPart partData)
        {
            this.LoadFrom(partData.GetXmlValue(new T(), SntpConfigKey));
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public sealed override void Save(UnitConfigPart partData)
        {
            partData.SetXmlValue(this.GetConfig(), SntpConfigKey);
        }

        /// <summary>
        /// Gets the SNTP configuration.
        /// </summary>
        /// <returns>
        /// The configuration.
        /// </returns>
        public virtual T GetConfig()
        {
            var config = new T { Enabled = this.ViewModel.IsVisible };

            if (this.versionNumber.SelectedOption != null)
            {
                config.VersionNumber = (SntpVersionNumber)this.versionNumber.SelectedValue;
            }

            if (this.retryInterval.Value.HasValue)
            {
                config.RetryInterval = this.retryInterval.Value.Value;
            }

            config.RetryCount = (int)this.retryCount.Value;
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
            var viewModel = new MultiEditorPartViewModel();

            this.versionNumber = new SelectionEditorViewModel();
            this.versionNumber.Label = AdminStrings.UnitConfig_TimeSync_Sntp_Version;
            this.versionNumber.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_TimeSync_Sntp_Version3,
                    SntpVersionNumber.Version3));
            this.versionNumber.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_TimeSync_Sntp_Version4,
                    SntpVersionNumber.Version4));
            viewModel.Editors.Add(this.versionNumber);

            this.retryCount = new NumberEditorViewModel();
            this.retryCount.Label = AdminStrings.UnitConfig_TimeSync_Sntp_RetryCount;
            this.retryCount.IsInteger = true;
            this.retryCount.MinValue = 0;
            this.retryCount.MaxValue = int.MaxValue;
            this.retryCount.PropertyChanged += (s, e) => this.UpdateRetryIntervalEnabled();
            viewModel.Editors.Add(this.retryCount);

            this.retryInterval = new TimeSpanEditorViewModel();
            this.retryInterval.Label = AdminStrings.UnitConfig_TimeSync_Sntp_RetryInterval;
            this.retryInterval.IsNullable = false;
            viewModel.Editors.Add(this.retryInterval);

            this.UpdateRetryIntervalEnabled();

            return viewModel;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            this.timeSource = this.GetPart<TimeSourcePartController>();
            this.timeSource.ViewModelUpdated += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
        }

        /// <summary>
        /// Loads the data from the given <paramref name="config"/> into the view model.
        /// </summary>
        /// <param name="config">
        /// The config to load the data from.
        /// </param>
        protected virtual void LoadFrom(T config)
        {
            this.versionNumber.SelectValue(config.VersionNumber);
            this.retryInterval.Value = config.RetryInterval;
            this.retryCount.Value = config.RetryCount;
        }

        private void UpdateVisibility()
        {
            this.ViewModel.IsVisible = this.timeSource.GetSelectedSource() == this.type;
        }

        private void UpdateRetryIntervalEnabled()
        {
            this.retryInterval.IsEnabled = this.retryCount.Value > 0;
        }
    }
}