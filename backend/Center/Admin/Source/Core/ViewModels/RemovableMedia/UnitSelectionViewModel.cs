// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitSelectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitSelectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.RemovableMedia
{
    using System.ComponentModel;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// View model that allows to select a unit in the export update tree.
    /// </summary>
    public class UnitSelectionViewModel : SynchronizableViewModelBase
    {
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitSelectionViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public UnitSelectionViewModel(UnitReadableModel model)
        {
            this.Model = model;
            this.Model.PropertyChanged += this.ModelOnPropertyChanged;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public UnitReadableModel Model { get; private set; }

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this unit is selected.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                this.SetProperty(ref this.isChecked, value, () => this.IsChecked);
            }
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                this.StartNew(() => this.RaisePropertyChanged(() => this.Name));
            }
        }
    }
}