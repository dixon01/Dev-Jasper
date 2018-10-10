// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The layout element data view model base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// The layout element data view model base.
    /// </summary>
    public abstract class LayoutElementDataViewModelBase : DataViewModelBase, ICloneable,  IPlacementTarget
    {
        private DataValue<string> elementName;
        private DataValue<bool> isResizable;

        private IMediaShell mediaShell;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutElementDataViewModelBase"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public LayoutElementDataViewModelBase(IMediaShell mediaShell, LayoutElementDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.ElementName = new DataValue<string>(string.Empty);
            this.IsResizable = new DataValue<bool>(true);
            if (dataModel != null)
            {
                this.ElementName.Value = dataModel.ElementName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutElementDataViewModelBase"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        public LayoutElementDataViewModelBase(IMediaShell mediaShell, LayoutElementDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ElementName = (DataValue<string>)dataViewModel.ElementName.Clone();
            this.ElementName.PropertyChanged += this.ElementNameChanged;
            this.ElementName.Value = dataViewModel.ElementName.Value;
            this.IsResizable = new DataValue<bool>(true);
            this.IsResizable.PropertyChanged += this.IsResizableChanged;
            this.IsResizable.Value = dataViewModel.IsResizable.Value;
        }

        /// <summary>
        /// Gets or sets the element name.
        /// </summary>
        [UserVisibleProperty("Layout")]
        public DataValue<string> ElementName
        {
            get
            {
                return this.elementName;
            }

            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.elementName);
                if (this.elementName != null)
                {
                    this.elementName.PropertyChanged -= this.ElementNameChanged;
                }

                this.SetProperty(ref this.elementName, value, () => this.ElementName);
                if (value != null)
                {
                    this.elementName.PropertyChanged += this.ElementNameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        public DataValue<int> X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        public DataValue<int> Y { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public DataValue<int> Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public DataValue<int> Height { get; set; }

        /// <summary>
        /// Gets or sets the use mouse position.
        /// </summary>
        public DataValue<bool> UseMousePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element is resizable.
        /// </summary>
        public DataValue<bool> IsResizable
        {
            get
            {
                return this.isResizable;
            }

            set
            {
                this.SetProperty(ref this.isResizable, value, () => this.IsResizable);
            }
        }

        /// <summary>
        /// Creates a deep clone.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public abstract object Clone();

        /// <summary>
        /// Exports the instance.
        /// </summary>
        /// <param name="parameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        /// <returns>
        /// The <see cref="ElementBase"/>.
        /// </returns>
        public abstract ElementBase Export(object parameters = null);

        /// <summary>
        /// Converts this instance to its data model representation.
        /// </summary>
        /// <returns>
        /// The <see cref="LayoutElementDataModelBase"/>.
        /// </returns>
        public abstract LayoutElementDataModelBase ToDataModel();

        /// <summary>
        ///  The element unsets his referenced media if it has one.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public virtual void UnsetMediaReference(ICommandRegistry commandRegistry)
        {
        }

        /// <summary>
        /// The element sets his referenced media if it has one.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public virtual void SetMediaReference(ICommandRegistry commandRegistry)
        {
        }

        /// <summary>
        /// Validates a value to be a positive double.
        /// </summary>
        /// <param name="dataValue">
        /// The data value.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        internal static void ValidatePositiveDouble(object dataValue, out string errorMessage)
        {
            if (!IsValidRequiredValue(dataValue, out errorMessage))
            {
                return;
            }

            double doubleValue;
            if (!double.TryParse(dataValue.ToString(), out doubleValue))
            {
                errorMessage = "A valid number is needed here";
                return;
            }

            if (doubleValue > 0)
            {
                errorMessage = null;
                return;
            }

            errorMessage = "The value must be greater than 0";
        }

        /// <summary>
        /// Checks if a compatibility conversion for CSV mapping is required
        /// </summary>
        /// <param name="exportParameters">
        /// The export parameters.
        /// </param>
        /// <returns>
        /// True if is required.
        /// </returns>
        protected bool CsvMappingCompatibilityRequired(object exportParameters)
        {
            var parameters = exportParameters as ExportCompatibilityParameters;
            return parameters != null && parameters.CsvMappingCompatibilityRequired;
        }

        /// <summary>
        /// The decrease media reference by hash.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        protected void DecreaseMediaReferenceByHash(string hash, ICommandRegistry commandRegistry)
        {
            var selectionParameters = new SelectResourceParameters { PreviousSelectedResourceHash = hash };
            commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
        }

        /// <summary>
        /// The increase media reference by hash.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        protected void IncreaseMediaReferenceByHash(string hash, ICommandRegistry commandRegistry)
        {
            var selectionParameters = new SelectResourceParameters { CurrentSelectedResourceHash = hash };
            commandRegistry.GetCommand(CommandCompositionKeys.Project.SelectResource).Execute(selectionParameters);
        }

        private static bool IsValidRequiredValue(object dataValue, out string errorMessage)
        {
            if (dataValue == null)
            {
                errorMessage = "Please insert a value";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void ElementNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ElementName);
        }

        private void IsResizableChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.IsResizable);
        }
    }
}