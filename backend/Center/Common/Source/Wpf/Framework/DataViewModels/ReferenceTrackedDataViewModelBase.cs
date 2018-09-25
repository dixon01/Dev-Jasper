// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceTrackedDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Serialization;

    using NLog;

    /// <summary>
    /// The tracked resource data view model base.
    /// </summary>
    public abstract class ReferenceTrackedDataViewModelBase : DataViewModelBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly ObservableCollection<TrackedResourceReference> references =
            new ObservableCollection<TrackedResourceReference>();

        private readonly Dictionary<DataViewModelReferenceKey, TrackedResourceReference> referenceDictionary =
            new Dictionary<DataViewModelReferenceKey, TrackedResourceReference>(
                DataViewModelReferenceKey.KeyComparer);

        private bool isUsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTrackedDataViewModelBase"/> class.
        /// </summary>
        protected ReferenceTrackedDataViewModelBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.References = new ReadOnlyObservableCollection<TrackedResourceReference>(this.references);
        }

        /// <summary>
        /// Gets a value indicating whether is used.
        /// </summary>
        [IgnoreDataMember]
        public bool IsUsed
        {
            get
            {
                return this.isUsed;
            }

            private set
            {
                this.SetProperty(ref this.isUsed, value, () => this.IsUsed);
            }
        }

        /// <summary>
        /// Gets the references.
        /// </summary>
        public ReadOnlyObservableCollection<TrackedResourceReference> References { get; private set; }

        /// <summary>
        /// Sets a reference for the given referrer view model and optionally for a specific
        /// property name.
        /// </summary>
        /// <param name="referrerViewModel">The referrer view model.</param>
        /// <param name="propertyName">The optional name of a specific property.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="referrerViewModel"/> is null.
        /// </exception>
        public virtual void SetReference(
            DataViewModelBase referrerViewModel,
            string propertyName = null)
        {
            if (referrerViewModel == null)
            {
                throw new ArgumentNullException("referrerViewModel");
            }

            lock (this.references)
            {
                var key = new DataViewModelReferenceKey(referrerViewModel);
                if (this.referenceDictionary.ContainsKey(key))
                {
                    this.Logger.Debug("Reference already present");
                    return;
                }

                var reference = new TrackedResourceReference(referrerViewModel, propertyName);

                this.referenceDictionary.Add(key, reference);
                this.references.Add(reference);
                this.IsUsed = this.GetIsUsed();
            }
        }

        /// <summary>
        /// Unsets a reference for the given referrer view model, optionally scoped to the given property name.
        /// </summary>
        /// <param name="referrerViewModel">
        /// The referrer view model.
        /// </param>
        /// <param name="propertyName">
        /// The optional name of the property.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="referrerViewModel"/> is null.
        /// </exception>
        public virtual void UnsetReference(DataViewModelBase referrerViewModel, string propertyName = null)
        {
            if (referrerViewModel == null)
            {
                throw new ArgumentNullException("referrerViewModel");
            }

            lock (this.references)
            {
                var key = new DataViewModelReferenceKey(referrerViewModel);
                if (!this.referenceDictionary.ContainsKey(key))
                {
                    this.Logger.Debug("Reference not present");
                    return;
                }

                var reference = new TrackedResourceReference(referrerViewModel, propertyName);

                this.references.Remove(reference);
                this.referenceDictionary.Remove(key);
                this.IsUsed = this.GetIsUsed();
            }
        }

        /// <summary>
        /// Gets the value for the <see cref="IsUsed"/> flag.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected virtual bool GetIsUsed()
        {
            return this.References.Any();
        }
    }
}