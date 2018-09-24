// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigatorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NavigatorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Navigator
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The view model for the entire navigator.
    /// </summary>
    public class NavigatorViewModel : ViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private readonly CollectionViewSource filteredPartitionsSource = new CollectionViewSource();

        private string searchText;

        private bool hasRemovableMedia;

        private NavigatorEntityViewModel selectedEntity;

        private bool homeIsSelected;

        private RemovableMediaViewModel selectedRemovableMedia;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">the command registry</param>
        public NavigatorViewModel(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Partitions = new ObservableCollection<PartitionViewModel>();
            this.RemovableMedia = new ObservableCollection<RemovableMediaViewModel>();

            this.filteredPartitionsSource.Source = this.Partitions;
            this.filteredPartitionsSource.Filter += this.FilteredPartitionsSourceOnFilter;
        }

        /// <summary>
        /// Gets or sets the search text for searching entity types.
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

                foreach (var partition in this.Partitions)
                {
                    partition.SearchText = value;
                }

                this.FilteredPartitions.Refresh();
            }
        }

        /// <summary>
        /// Gets all available partitions.
        /// </summary>
        public ObservableCollection<PartitionViewModel> Partitions { get; private set; }

        /// <summary>
        /// Gets the list of partitions filtered by <see cref="SearchText"/>.
        /// Only partitions are shown that contain filtered items (i.e. "empty" partitions are hidden).
        /// </summary>
        public ICollectionView FilteredPartitions
        {
            get
            {
                return this.filteredPartitionsSource.View;
            }
        }

        /// <summary>
        /// Gets or sets the selected entity.
        /// </summary>
        public NavigatorEntityViewModel SelectedEntity
        {
            get
            {
                return this.selectedEntity;
            }

            set
            {
                this.SetProperty(ref this.selectedEntity, value, () => this.SelectedEntity);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether has the local system has removable media attached.
        /// </summary>
        public bool HasRemovableMedia
        {
            get
            {
                return this.hasRemovableMedia;
            }

            set
            {
                this.SetProperty(ref this.hasRemovableMedia, value, () => this.HasRemovableMedia);
            }
        }

        /// <summary>
        /// Gets the list of removable media (USB sticks).
        /// </summary>
        public ObservableCollection<RemovableMediaViewModel> RemovableMedia { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected removable media (USB stick).
        /// </summary>
        public RemovableMediaViewModel SelectedRemovableMedia
        {
            get
            {
                return this.selectedRemovableMedia;
            }

            set
            {
                this.SetProperty(ref this.selectedRemovableMedia, value, () => this.SelectedRemovableMedia);
            }
        }

        /// <summary>
        /// Gets the go home command (which will show the home stage).
        /// </summary>
        public ICommand GoHomeCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Navigator.GoHome);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether home is selected.
        /// </summary>
        public bool HomeIsSelected
        {
            get
            {
                return this.homeIsSelected;
            }

            set
            {
                this.SetProperty(ref this.homeIsSelected, value, () => this.HomeIsSelected);
            }
        }

        private void FilteredPartitionsSourceOnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = this.Filter(e.Item);
        }

        private bool Filter(object item)
        {
            var partition = item as PartitionViewModel;
            if (partition == null)
            {
                return false;
            }

            return partition.FilteredEntities.OfType<NavigatorEntityViewModel>().Any();
        }
    }
}
