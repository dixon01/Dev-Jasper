// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateGroupSelectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateGroupSelectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.RemovableMedia
{
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// View model that allows to select a unit group in the export update tree.
    /// </summary>
    public class UpdateGroupSelectionViewModel : SynchronizableViewModelBase
    {
        private bool? isChecked = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGroupSelectionViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public UpdateGroupSelectionViewModel(UpdateGroupReadableModel model)
        {
            this.Model = model;
            this.Model.PropertyChanged += this.ModelOnPropertyChanged;
            this.Model.Units.CollectionChanged += this.ItemsOnCollectionChanged;

            this.Units = new ObservableItemCollection<UnitSelectionViewModel>();
            this.Units.ItemPropertyChanged += this.UnitsOnItemPropertyChanged;
            foreach (var unit in model.Units)
            {
                this.Units.Add(new UnitSelectionViewModel(unit));
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public UpdateGroupReadableModel Model { get; private set; }

        /// <summary>
        /// Gets the name of the update group.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        /// <summary>
        /// Gets the list of units.
        /// </summary>
        public ObservableItemCollection<UnitSelectionViewModel> Units { get; private set; }

        /// <summary>
        /// Gets or sets a flag indicating if this unit group is selected.
        /// If the value is not set, the unit group has some selected units and some non-selected units.
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (this.isChecked.HasValue && !value.HasValue)
                {
                    value = !this.isChecked.Value;
                }

                if (!this.SetProperty(ref this.isChecked, value, () => this.IsChecked) || !value.HasValue)
                {
                    return;
                }

                foreach (var unit in this.Units)
                {
                    unit.IsChecked = value.Value;
                }
            }
        }

        private void UpdateCheckState()
        {
            var hasChecked = this.Units.Any(u => u.IsChecked);
            var hasUnchecked = this.Units.Any(u => !u.IsChecked);
            this.SetProperty(
                ref this.isChecked,
                hasChecked && hasUnchecked ? null : (bool?)hasChecked,
                () => this.IsChecked);
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.StartNew(
                () =>
                    {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Reset:
                                this.Units.Clear();
                                break;
                            case NotifyCollectionChangedAction.Add:
                                this.Units.Insert(
                                    e.NewStartingIndex,
                                    new UnitSelectionViewModel((UnitReadableModel)e.NewItems[0]));
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                this.Units.Remove(this.Units[e.OldStartingIndex]);
                                break;
                        }

                        this.UpdateCheckState();
                    });
        }

        private void UnitsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<UnitSelectionViewModel> e)
        {
            this.StartNew(this.UpdateCheckState);
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