// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;

    /// <summary>
    /// The xml editor view model.
    /// </summary>
    public class XmlEditorViewModel : EditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlEditorViewModel"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public XmlEditorViewModel(string filename)
        {
            this.Config = new ExportXmlConfigFile(filename, null);
            this.Config.PropertyChanged += this.OnConfigChanged;
            this.Config.ErrorsChanged += this.OnConfigErrorsChanged;
            this.UpdateErrors();
        }

        /// <summary>
        /// Gets the xml config file.
        /// </summary>
        public ExportXmlConfigFile Config { get; private set; }

        private void OnConfigChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                this.IsDirty = this.Config.IsDirty;
            }
        }

        private void OnConfigErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            this.UpdateErrors();
        }

        private void UpdateErrors()
        {
            this.ClearErrors("Config");
            foreach (var error in this.Config.GetErrorMessages(null))
            {
                this.SetError("Config", error);
            }

            this.RaiseErrorsChanged("Config");
        }
    }
}
