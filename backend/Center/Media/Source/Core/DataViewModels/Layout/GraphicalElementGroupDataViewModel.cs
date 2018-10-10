// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalElementGroupDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines a group of <see cref="GraphicalElementDataViewModelBase"/> items.
    /// </summary>
    public class GraphicalElementGroupDataViewModel : ViewModelBase
    {
        private readonly ObservableCollection<GraphicalElementDataViewModelBase> items;

        private string groupName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalElementGroupDataViewModel"/> class.
        /// </summary>
        public GraphicalElementGroupDataViewModel()
        {
            this.items = new ObservableCollection<GraphicalElementDataViewModelBase>();
        }

        /// <summary>
        /// Gets the collection of <see cref="GraphicalElementDataViewModelBase"/>.
        /// </summary>
        public ObservableCollection<GraphicalElementDataViewModelBase> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.groupName;
            }

            set
            {
                if (this.TrySetPropertyBackValue(ref this.groupName, value))
                {
                    this.RaisePropertyChanged(() => this.GroupName);
                }
            }
        }
    }
}
