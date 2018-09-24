// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceReferenceDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceReferenceDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the data view model for a resource reference.
    /// </summary>
    public class ResourceReferenceDataViewModel : DataViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string hash;
        private ResourceInfoDataViewModel resourceInfoReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceDataViewModel"/> class.
        /// </summary>
        /// <param name="resourceReference">The resource reference.</param>
        public ResourceReferenceDataViewModel(ResourceReference resourceReference)
            : this()
        {
            this.Hash = resourceReference.Hash;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceDataViewModel"/> class.
        /// </summary>
        /// <param name="resourceReference">The resource reference.</param>
        public ResourceReferenceDataViewModel(ResourceReferenceDataViewModel resourceReference)
            : this()
        {
            this.Hash = resourceReference.Hash;
            this.DisplayText = resourceReference.DisplayText;
            this.ResourceInfo = resourceReference.ResourceInfo.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceDataViewModel"/> class.
        /// </summary>
        public ResourceReferenceDataViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the hash of the resource.
        /// </summary>
        /// <value>
        /// The hash of the resource.
        /// </value>
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.SetProperty(ref this.hash, value, () => this.Hash);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ResourceInfoDataViewModel"/> related to the property
        /// <see cref="ResourceReferenceDataViewModel.Hash"/>. If the resource with the given Hash could not be found,
        /// <c>null</c> will be returned.
        /// </summary>
        public ResourceInfoDataViewModel ResourceInfo
        {
            get
            {
                return this.resourceInfoReference ?? (this.resourceInfoReference = this.FindReference());
            }

            set
            {
                this.SetProperty(ref this.resourceInfoReference, value, () => this.ResourceInfo);
                if (value != null)
                {
                    this.Hash = value.Hash;
                }
            }
        }

        /// <summary>
        /// Converts this data view model to an equivalent <see cref="ResourceReference"/>.
        /// </summary>
        /// <returns>A <see cref="ResourceReference"/> equivalent to this data view model.</returns>
        public ResourceReference ToDataModel()
        {
            return new ResourceReference { Hash = this.Hash };
        }

        /// <summary>
        /// Clone function
        /// </summary>
        /// <returns>the cloned object</returns>
        public ResourceReferenceDataViewModel Clone()
        {
            return new ResourceReferenceDataViewModel(this);
        }

        private ResourceInfoDataViewModel FindReference()
        {
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (applicationState.CurrentProject == null || applicationState.CurrentProject.Resources == null)
            {
                return null;
            }

            foreach (var resource in applicationState.CurrentProject.Resources)
            {
                if (resource.Hash == this.Hash)
                {
                    return resource;
                }
            }

            Logger.Trace("Resource reference with hash {0} not found in the current project.", this.Hash);
            return null;
        }
    }
}