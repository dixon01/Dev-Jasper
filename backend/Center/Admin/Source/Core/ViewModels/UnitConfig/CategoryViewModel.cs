// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CategoryViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Data;

    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// View model representing a category (group) in the unit configurator navigation tree.
    /// </summary>
    public class CategoryViewModel : UnitConfigTreeNodeViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private readonly CollectionViewSource filteredPartsSource = new CollectionViewSource();

        private bool isExpanded;

        private bool isVisible;

        private bool canBeVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryViewModel"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public CategoryViewModel(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Parts = new ObservableItemCollection<PartViewModelBase>();

            this.filteredPartsSource.Source = this.Parts;
            this.filteredPartsSource.Filter += this.FilteredPartsSourceOnFilter;

            this.Parts.ItemPropertyChanged += this.PartsOnItemPropertyChanged;
            this.Parts.CollectionChanged += this.PartsOnCollectionChanged;
        }

        /// <summary>
        /// Gets a value indicating whether this category is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }

            private set
            {
                this.SetProperty(ref this.isVisible, value, () => this.IsVisible);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this category can be visible.
        /// This can be set to false to completely disable a category and is used during data initialization.
        /// </summary>
        public bool CanBeVisible
        {
            get
            {
                return this.canBeVisible;
            }

            set
            {
                if (this.SetProperty(ref this.canBeVisible, value, () => this.CanBeVisible))
                {
                    this.UpdateIsVisible();
                }
            }
        }

        /// <summary>
        /// Gets the error state of this node.
        /// </summary>
        public override ErrorState ErrorState
        {
            get
            {
                var errorState = ErrorState.Ok;
                foreach (var part in this.Parts.Where(p => p.IsVisible))
                {
                    if (part.ErrorState <= errorState)
                    {
                        continue;
                    }

                    errorState = part.ErrorState;
                    if (errorState == ErrorState.Error)
                    {
                        break;
                    }
                }

                return errorState;
            }
        }

        /// <summary>
        /// Gets all errors of the <see cref="Parts"/> in this category.
        /// </summary>
        public override ICollection<ErrorItem> Errors
        {
            get
            {
                return
                    this.Parts.Where(p => p.IsVisible).SelectMany(
                        part => part.Errors,
                        (part, error) => new ErrorItem(error.State, part.DisplayName + ": " + error.Message)).ToList();
            }
        }

        /// <summary>
        /// Gets the parts of this category.
        /// </summary>
        public ObservableItemCollection<PartViewModelBase> Parts { get; private set; }

        /// <summary>
        /// Gets the parts filtered by their visibility.
        /// </summary>
        public ICollectionView FilteredParts
        {
            get
            {
                return this.filteredPartsSource.View;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this category is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.SetProperty(ref this.isExpanded, value, () => this.IsExpanded);
            }
        }

        /// <summary>
        /// Clears the <see cref="UnitConfigTreeNodeViewModelBase.IsDirty"/> flag.
        /// The default behavior clears the flag on the current object and all its children.
        /// </summary>
        public override void ClearDirty()
        {
            base.ClearDirty();
            foreach (var part in this.Parts)
            {
                part.ClearDirty();
            }
        }

        private bool Filter(object item)
        {
            var part = item as PartViewModelBase;
            if (part == null)
            {
                return false;
            }

            return part.IsVisible;
        }

        private void FilteredPartsSourceOnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = this.Filter(e.Item);
        }

        private void PartsOnItemPropertyChanged(object sender, ItemPropertyChangedEventArgs<PartViewModelBase> e)
        {
            switch (e.PropertyName)
            {
                case "IsVisible":
                case "ErrorState":
                    if (e.PropertyName == "IsVisible")
                    {
                        this.filteredPartsSource.View.Refresh();
                        this.UpdateIsVisible();
                    }

                    this.RaisePropertyChanged(() => this.ErrorState);
                    break;
                case "Errors":
                    this.RaisePropertyChanged(() => this.Errors);
                    break;
                case "IsDirty":
                    this.IsDirty = this.Parts.Any(p => p.IsDirty);
                    break;
            }
        }

        private void UpdateIsVisible()
        {
            this.IsVisible = this.CanBeVisible && !this.filteredPartsSource.View.IsEmpty;
        }

        private void PartsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateIsVisible();
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var part in e.NewItems.OfType<PartViewModelBase>())
            {
                part.Setup(this, this.commandRegistry);
            }
        }
    }
}