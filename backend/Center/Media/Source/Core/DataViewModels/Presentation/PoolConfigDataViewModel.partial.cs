// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The pool config data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// The pool config data view model.
    /// </summary>
    public partial class PoolConfigDataViewModel : IUsageTrackedObject
    {
        private int referencesCount;

        private bool isUsed;

        private bool isInEditMode;

        private ExtendedObservableCollection<ResourceReferenceDataViewModel> resourceReferences;

        /// <summary>
        /// Gets a value indicating whether this resource is referenced by any entity of the system.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this resource is referenced by any entity of the system; otherwise, <c>false</c>.
        /// </value>
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
        /// Gets or sets a value indicating whether this resource is in edit mode
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                this.SetProperty(ref this.isInEditMode, value, () => this.IsInEditMode);
            }
        }

        /// <summary>
        /// Gets or sets the references count.
        /// </summary>
        /// <value>
        /// The references count.
        /// </value>
        public int ReferencesCount
        {
            get
            {
                return this.referencesCount;
            }

            set
            {
                this.SetProperty(ref this.referencesCount, value, () => this.ReferencesCount);
                this.IsUsed = this.ReferencesCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets the resource references.
        /// </summary>
        /// <value>
        /// The resource references.
        /// </value>
        public ExtendedObservableCollection<ResourceReferenceDataViewModel> ResourceReferences
        {
            get
            {
                return this.resourceReferences;
            }

            set
            {
                this.SetProperty(ref this.resourceReferences, value, () => this.ResourceReferences);
            }
        }

        /// <summary>
        /// the string conversion method for a pool
        /// </summary>
        /// <returns>the string representation</returns>
        public override string ToString()
        {
            return this.Name != null ? this.Name.Value : string.Empty;
        }

        partial void Initialize(Models.Presentation.PoolConfigDataModel dataModel)
        {
            this.ResourceReferences = new ExtendedObservableCollection<ResourceReferenceDataViewModel>();
            if (dataModel != null)
            {
                var resourceReferences =
                    dataModel.ResourceReferences.Select(r => new ResourceReferenceDataViewModel(r));
                this.ResourceReferences = new ExtendedObservableCollection<ResourceReferenceDataViewModel>();
                this.ResourceReferences.AddRange(resourceReferences);
                this.ReferencesCount = dataModel.ReferencesCount;
            }
        }

        partial void Initialize(PoolConfigDataViewModel dataViewModel)
        {
            this.ResourceReferences = new ExtendedObservableCollection<ResourceReferenceDataViewModel>();
            if (dataViewModel != null)
            {
                var resourceReferences = dataViewModel.ResourceReferences.Select(r => r.Clone());
                this.ResourceReferences = new ExtendedObservableCollection<ResourceReferenceDataViewModel>();
                this.ResourceReferences.AddRange(resourceReferences);
                this.ReferencesCount = dataViewModel.ReferencesCount;
            }
        }

        partial void ConvertNotGeneratedToDataModel(ref Models.Presentation.PoolConfigDataModel dataModel)
        {
            dataModel.ResourceReferences = this.ResourceReferences.Select(r => r.ToDataModel()).ToList();
            dataModel.ReferencesCount = this.ReferencesCount;
        }
    }
}
