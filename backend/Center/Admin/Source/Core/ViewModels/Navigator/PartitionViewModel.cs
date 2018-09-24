// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartitionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartitionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Navigator
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Data;

    /// <summary>
    /// The view model for a partition shown in the navigator.
    /// </summary>
    public class PartitionViewModel : NavigatorViewModelBase
    {
        private readonly CollectionViewSource filteredEntitiesSource = new CollectionViewSource();

        private string searchText;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the partition.
        /// </param>
        public PartitionViewModel(string name)
            : base(name)
        {
            this.Entities = new ObservableCollection<NavigatorEntityViewModel>();

            this.filteredEntitiesSource.Source = this.Entities;
            this.filteredEntitiesSource.Filter += this.FilteredEntitiesSourceOnFilter;
        }

        /// <summary>
        /// Gets or sets the search text for searching entity types in this partition.
        /// </summary>
        public string SearchText
        {
            get
            {
                return this.searchText;
            }

            set
            {
                if (!this.SetProperty(ref this.searchText, value, () => this.SearchText))
                {
                    return;
                }

                this.filteredEntitiesSource.View.Refresh();
            }
        }

        /// <summary>
        /// Gets all available entities.
        /// </summary>
        public ObservableCollection<NavigatorEntityViewModel> Entities { get; private set; }

        /// <summary>
        /// Gets the list of entities filtered by <see cref="SearchText"/>.
        /// Only partitions are shown that contain the given string (case insensitive).
        /// </summary>
        public ICollectionView FilteredEntities
        {
            get
            {
                return this.filteredEntitiesSource.View;
            }
        }

        private void FilteredEntitiesSourceOnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = this.Filter(e.Item);
        }

        private bool Filter(object item)
        {
            var entity = item as NavigatorEntityViewModel;
            if (entity == null || !entity.IsAllowed)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.SearchText))
            {
                return true;
            }

            var search = this.SearchText.Trim();
            if (entity.DisplayName.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                return false;
            }

            this.IsExpanded = true;
            return true;
        }
    }
}