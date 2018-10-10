// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CycleNavigationTreeViewDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This class contains all cycle packages related to a physical screen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// This class contains all cycle packages related to a physical screen type.
    /// </summary>
    public class CycleNavigationTreeViewDataViewModel : DataViewModelBase
    {
        private PhysicalScreenType physicalScreenType;

        private ExtendedObservableCollection<CyclePackageConfigDataViewModel> cyclePackages;

        private bool isExpanded;

        private bool isChildItemSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="CycleNavigationTreeViewDataViewModel"/> class.
        /// </summary>
        /// <param name="physicalScreenType">
        /// The physical Screen Type.
        /// </param>
        /// <param name="packages">
        /// The packages.
        /// </param>
        public CycleNavigationTreeViewDataViewModel(
            PhysicalScreenType physicalScreenType,
            ExtendedObservableCollection<CyclePackageConfigDataViewModel> packages)
        {
            this.PhysicalScreenType = physicalScreenType;
            this.CyclePackages = packages;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is expanded.
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
        /// Gets or sets a value indicating whether one of the child items is selected.
        /// </summary>
        public bool IsChildItemSelected
        {
            get
            {
                return this.isChildItemSelected;
            }

            set
            {
                this.SetProperty(ref this.isChildItemSelected, value, () => this.IsChildItemSelected);
            }
        }

        /// <summary>
        /// Gets the type of the physical screen.
        /// </summary>
        public PhysicalScreenType PhysicalScreenType
        {
            get
            {
                return this.physicalScreenType;
            }

            private set
            {
                this.SetProperty(ref this.physicalScreenType, value, () => this.PhysicalScreenType);
            }
        }

        /// <summary>
        /// Gets the cycle packages.
        /// </summary>
        public ExtendedObservableCollection<CyclePackageConfigDataViewModel> CyclePackages
        {
            get
            {
                return this.cyclePackages;
            }

            private set
            {
                this.SetProperty(ref this.cyclePackages, value, () => this.CyclePackages);
            }
        }

        /// <summary>
        /// Collapses all tree view nodes.
        /// </summary>
        public void CollapseAll()
        {
            this.SetExpandedState(false);
        }

        /// <summary>
        /// Expands all tree view nodes.
        /// </summary>
        public void ExpandAll()
        {
            this.SetExpandedState(true);
        }

        private void SetExpandedState(bool expand)
        {
            foreach (var cyclePackage in this.CyclePackages)
            {
                foreach (var cycle in cyclePackage.StandardCycles)
                {
                    foreach (var section in cycle.Reference.Sections)
                    {
                        section.IsExpanded = expand;
                    }

                    cycle.IsExpanded = expand;
                }

                foreach (var cycle in cyclePackage.EventCycles)
                {
                    foreach (var section in cycle.Reference.Sections)
                    {
                        section.IsExpanded = expand;
                    }

                    cycle.IsExpanded = expand;
                }

                cyclePackage.IsExpanded = expand;
            }

            this.IsExpanded = expand;
        }
    }
}
