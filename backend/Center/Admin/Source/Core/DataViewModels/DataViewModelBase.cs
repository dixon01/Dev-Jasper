// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System;
    using System.Linq.Expressions;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The base class for writable entity data view models.
    /// </summary>
    public abstract class DataViewModelBase : ErrorViewModelBase, IDisposable
    {
        private bool isDisposed;

        private bool isLoading;

        private bool isDisposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModelBase"/> class.
        /// </summary>
        /// <param name="readOnlyDataViewModel">
        /// The read-only data view model.
        /// </param>
        /// <param name="factory">
        /// The factory that created this object.
        /// </param>
        protected DataViewModelBase(
            ReadOnlyDataViewModelBase readOnlyDataViewModel, DataViewModelFactory factory)
        {
            this.ReadOnlyDataViewModel = readOnlyDataViewModel;
            this.Factory = factory;
        }

        /// <summary>
        /// Event that is risen whenever this object is being disposed of.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Gets the read-only data view model.
        /// </summary>
        public ReadOnlyDataViewModelBase ReadOnlyDataViewModel { get; private set; }

        /// <summary>
        /// Gets the data view model factory.
        /// </summary>
        public DataViewModelFactory Factory { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this entity is currently loading.
        /// This property gets set when reference properties are being filled with possible references.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                if (this.SetProperty(ref this.isLoading, value, () => this.IsLoading) && !value)
                {
                    // often the property is reset "asynchronously" (without user interaction)
                    // therefore we need to trigger the command manager
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets the display text of this data view model.
        /// </summary>
        public abstract string DisplayText { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposing = true;
            var handler = this.Disposing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }

            this.isDisposed = true;
            this.Dispose(true);
        }

        /// <summary>
        /// Raises the <see cref="ViewModelBase.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected override void RaisePropertyChanged(string propertyName)
        {
            if (!this.isDisposing)
            {
                base.RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Raises the <see cref="ViewModelBase.PropertyChanged"/> event.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected override void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            if (!this.isDisposing)
            {
                base.RaisePropertyChanged(expression);
            }
        }

        /// <summary>
        /// Implementation of the disposing.
        /// </summary>
        /// <param name="disposing">
        /// A flag indicating if the object is being disposed or finalized.
        /// </param>
        protected abstract void Dispose(bool disposing);
    }
}
