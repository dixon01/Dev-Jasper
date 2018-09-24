// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Base class for all element composers.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The type of data view model this composer is handling.
    /// </typeparam>
    public abstract class ComposerBase<TViewModel> : IComposer
        where TViewModel : DataViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="context">
        /// The composer context.
        /// </param>
        /// <param name="parent">
        /// The parent composer. Can be null if the view model doesn't have a parent.
        /// </param>
        /// <param name="viewModel">
        /// The data view model.
        /// </param>
        protected ComposerBase(IComposerContext context, IComposer parent, TViewModel viewModel)
        {
            this.Context = context;
            this.Parent = parent;
            this.ViewModel = viewModel;
            this.ViewModel.PropertyChanged += this.ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Gets the data view model.
        /// </summary>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets the parent composer.
        /// </summary>
        protected IComposer Parent { get; private set; }

        /// <summary>
        /// Gets the composer context.
        /// </summary>
        protected IComposerContext Context { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.ViewModel.PropertyChanged -= this.ViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Method to be overridden by subclasses to handle the change of a property of the data view model.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void HandlePropertyChange(string propertyName)
        {
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandlePropertyChange(e.PropertyName);
        }
    }
}