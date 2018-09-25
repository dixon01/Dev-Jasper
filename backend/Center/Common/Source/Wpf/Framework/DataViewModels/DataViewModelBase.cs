// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Core.Validation;

    /// <summary>
    /// Defines the base class for data view models.
    /// </summary>
    public abstract class DataViewModelBase : ValidationViewModelBase, IDataViewModel, IDisposable
    {
        private string displayText;

        private bool isDirty;

        private bool isItemSelected;

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public virtual string DisplayText
        {
            get
            {
                return this.displayText;
            }

            set
            {
                this.SetProperty(ref this.displayText, value, () => this.DisplayText);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this item is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }

            set
            {
                this.SetProperty(ref this.isItemSelected, value, () => this.IsItemSelected);
            }
        }

        /// <summary>
        /// Gets a value indicating whether any property of this instance was modified.
        /// </summary>
        /// <value>
        /// <c>true</c> if any property of this instance was modified; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            private set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public virtual void MakeDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag. It clears the flag on the current object and all
        /// its children.
        /// </summary>
        public virtual void ClearDirty()
        {
            this.isDirty = false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.DisplayText) ? base.ToString() : this.DisplayText;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// compares two view models
        /// </summary>
        /// <param name="obj">the other view model</param>
        /// <returns>true if equal</returns>
        public virtual bool EqualsViewModel(object obj)
        {
            return true;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Nothing to dispose here
        }

        /// <summary>
        /// Maps all properties of this class with the given model.
        /// </summary>
        /// <param name="model">
        /// The infomedia model that will be exported.
        /// </param>
        /// <param name="exportParameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        protected virtual void DoExport(object model, object exportParameters)
        {
        }

        /// <summary>
        /// Maps all properties of this class with the given data model.
        /// </summary>
        /// <param name="dataModel">
        /// The data model to map.
        /// </param>
        protected virtual void ConvertToDataModel(object dataModel)
        {
        }

        /// <summary>
        /// Registers to the property changed event for the <c>IsDirty</c> flag.
        /// </summary>
        /// <param name="notifyPropertyChanged">
        /// The notify property changed object that could raise the property changed event for the
        /// <c>IsDirty</c> flag.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notifyPropertyChanged"/> argument is null.
        /// </exception>
        protected virtual void RegisterIsDirtyPropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            if (notifyPropertyChanged == null)
            {
                return;
            }

            notifyPropertyChanged.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Registers to the property changed event for the <c>IsDirty</c> flag.
        /// </summary>
        /// <param name="notifyPropertyChanged">
        /// The notify property changed object that could raise the property changed event for the
        /// <c>IsDirty</c> flag.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notifyPropertyChanged"/> argument is null.
        /// </exception>
        protected virtual void UnregisterIsDirtyPropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            if (notifyPropertyChanged == null)
            {
                return;
            }

            notifyPropertyChanged.PropertyChanged -= this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.IsDirty);
        }
    }
}