// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines a section with resources.
    /// </summary>
    public class ResourceSectionViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSectionViewModel"/> class.
        /// </summary>
        public ResourceSectionViewModel()
        {
            this.Resources = new ObservableCollection<ResourceViewModel>();
        }

        /// <summary>
        /// Gets or sets the selected resource.
        /// </summary>
        public ResourceViewModel SelectedResource { get; set; }

        /// <summary>
        /// Gets the collection of resources.
        /// </summary>
        public ObservableCollection<ResourceViewModel> Resources { get; private set; }
    }
}