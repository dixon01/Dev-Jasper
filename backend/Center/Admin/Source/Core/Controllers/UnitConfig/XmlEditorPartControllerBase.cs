// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlEditorPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// Base class for all parts that only use an Xml editor to edit the whole configuration.
    /// </summary>
    public abstract class XmlEditorPartControllerBase : PartControllerBase<XmlPartViewModel>
    {
        private readonly string configurationKey;

        private readonly string configFileName;

        private readonly string displayName;

        private readonly string description;

        private readonly Lazy<string> lazyDefaultConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEditorPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The part key.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        /// <param name="configurationKey">
        /// The configuration Key.
        /// </param>
        /// <param name="configFilename">
        /// The config Filename.
        /// </param>
        /// <param name="displayName">
        /// The display name of the part.
        /// </param>
        /// <param name="description">
        /// The description of the part.
        /// </param>
        protected XmlEditorPartControllerBase(
            string key,
            CategoryControllerBase parent,
            string configurationKey,
            string configFilename,
            string displayName,
            string description)
            : base(key, parent)
        {
            this.configurationKey = configurationKey;
            this.configFileName = configFilename;
            this.displayName = displayName;
            this.description = description;
            this.lazyDefaultConfig = new Lazy<string>(this.GetDefaultConfig);
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.ViewModel.Editor.ClearErrors();

            // Hack to be sure that the errors are shown when creating a very new UnitConfiguration
            this.ViewModel.Editor.Config.Document = null;
            this.ViewModel.Editor.Config.Document = partData.GetValue(
                this.lazyDefaultConfig.Value,
                this.configurationKey);
            this.ViewModel.Editor.Config.IsDirty = false;
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.ViewModel.Editor.Config.Document, this.configurationKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="XmlPartViewModel"/>.
        /// </returns>
        protected override XmlPartViewModel CreateViewModel()
        {
            var viewModel = new XmlPartViewModel(new XmlEditorViewModel(this.configFileName));
            viewModel.DisplayName = this.displayName;
            viewModel.Description = this.description;
            viewModel.Editor.PropertyChanged += (s, e) => this.RaiseViewModelUpdated(e);
            return viewModel;
        }

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        /// <returns>
        /// The default configuration as string
        /// </returns>
        protected abstract string GetDefaultConfig();
    }
}
