// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionedSettingValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VersionedSettingValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Conclusion;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;

    /// <summary>
    /// Validator for settings that are only available from a certain version.
    /// </summary>
    public class VersionedSettingValidator
    {
        private readonly EditorViewModelBase editor;

        private readonly object defaultValue;

        private readonly string packageId;

        private readonly Version minimumVersion;

        private readonly UnitConfiguratorController rootController;

        private readonly Func<EditorViewModelBase, object> valueGetter;

        private readonly string propertyName;

        private SoftwareVersionsPartController softwareVersions;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedSettingValidator"/> class.
        /// </summary>
        /// <param name="editor">
        /// The editor for which this validator is responsible.
        /// </param>
        /// <param name="defaultValue">
        /// The default value which should be set in the editor if the given
        /// <paramref name="minimumVersion"/> has not been reached (an older software was selected).
        /// </param>
        /// <param name="packageId">
        /// The package for which the <paramref name="minimumVersion"/> should be checked.
        /// This should be one of the values of <see cref="PackageIds"/>.
        /// </param>
        /// <param name="minimumVersion">
        /// The minimum version of <paramref name="minimumVersion"/> that needs to be selected
        /// for the editor to be allowed.
        /// </param>
        /// <param name="rootController">
        /// The <see cref="UnitConfiguratorController"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If the <paramref name="editor"/> type is not yet supported by this class.
        /// </exception>
        public VersionedSettingValidator(
            EditorViewModelBase editor,
            object defaultValue,
            string packageId,
            Version minimumVersion,
            UnitConfiguratorController rootController)
        {
            this.editor = editor;
            this.defaultValue = defaultValue;
            this.packageId = packageId;
            this.minimumVersion = minimumVersion;
            this.rootController = rootController;

            if (editor is CheckableEditorViewModel)
            {
                this.valueGetter =
                    e =>
                        {
                            var ed = (CheckableEditorViewModel)e;
                            return ed.IsChecked.HasValue && ed.IsChecked.Value;
                        };
                this.propertyName = "IsChecked";
            }
            else if (editor is TextEditorViewModel)
            {
                this.valueGetter = e => ((TextEditorViewModel)e).Text;
                this.propertyName = "Text";
            }
            else if (editor is SelectionEditorViewModel)
            {
                this.valueGetter = e => ((SelectionEditorViewModel)e).SelectedValue;
                this.propertyName = "SelectedOption";
            }
            else if (editor is NumberEditorViewModel)
            {
                this.defaultValue = Convert.ToDecimal(defaultValue);
                this.valueGetter = e => ((NumberEditorViewModel)e).Value;
                this.propertyName = "Value";
            }
            else
            {
                throw new NotSupportedException("Editor not yet supported: " + editor.GetType().Name);
            }
        }

        /// <summary>
        /// Starts this validator and listens to changes in the associated view models.
        /// </summary>
        public void Start()
        {
            this.softwareVersions = this.rootController.GetPart<SoftwareVersionsPartController>();
            this.softwareVersions.ViewModelUpdated += (s, e) => this.UpdateError();
            this.editor.PropertyChanged += this.EditorOnPropertyChanged;
            this.UpdateError();
        }

        private void EditorOnPropertyChanged(object s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.propertyName || e.PropertyName == "IsEnabled")
            {
                this.UpdateError();
            }
        }

        private void UpdateError()
        {
            PackageVersionReadableModel version;
            var errorState = ErrorState.Ok;
            if (this.softwareVersions.GetSelectedPackageVersions().TryGetValue(this.packageId, out version)
                && new Version(version.SoftwareVersion) < this.minimumVersion
                && !object.Equals(this.defaultValue, this.valueGetter(this.editor))
                && this.editor.IsEnabled)
            {
                errorState = ErrorState.Warning;
            }

            var message = string.Format(
                AdminStrings.UnitConfig_SettingValidator_UnsupportedValueFormat,
                this.editor.Label,
                this.defaultValue,
                version != null ? version.Package.ProductName : string.Empty,
                this.minimumVersion);
            this.editor.SetError(this.propertyName, errorState, message);
        }
    }
}
