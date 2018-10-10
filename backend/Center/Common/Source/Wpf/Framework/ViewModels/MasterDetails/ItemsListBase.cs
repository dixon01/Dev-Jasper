// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsListBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemsListBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.MasterDetails
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Defines the base class for list of items.
    /// </summary>
    public abstract class ItemsListBase : ViewModelBase
    {
        private readonly Lazy<ICollectionView> itemsView;

        private readonly Lazy<IMasterDetailsStage> stage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsListBase"/> class.
        /// </summary>
        /// <param name="stage">The stage.</param>
        protected ItemsListBase(Lazy<IMasterDetailsStage> stage)
        {
            this.stage = stage;
            this.itemsView = new Lazy<ICollectionView>(this.CreateItemsView);
            this.SortCommand = new RelayCommand(this.Sort);
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public ListSortDirection SortDirection { get; private set; }

        /// <summary>
        /// Gets the stage.
        /// </summary>
        public IMasterDetailsStage Stage
        {
            get
            {
                return this.stage.Value;
            }
        }

        /// <summary>
        /// Gets the view of tenants supporting sorting and filtering.
        /// </summary>
        public ICollectionView ItemsView
        {
            get
            {
                return this.itemsView.Value;
            }
        }

        /// <summary>
        /// Determines whether the given column should be shown in the header.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>
        /// The properties of the column with the given name.
        /// </returns>
        public abstract ColumnDescriptor GetColumnDescriptor(string name);

        /// <summary>
        /// Lists the selected items.
        /// </summary>
        /// <returns>The selected items.</returns>
        public IEnumerable<IDataViewModel> ListSelectedItems()
        {
            return this.ItemsView.OfType<IDataViewModel>().Where(model => model.IsItemSelected);
        }

        /// <summary>
        /// Lists the selected items of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the selected items.</typeparam>
        /// <returns>
        /// The selected items.
        /// </returns>
        public IEnumerable<T> ListSelectedItems<T>()
            where T : IDataViewModel
        {
            return this.ItemsView.OfType<T>().Where(model => model.IsItemSelected);
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="item">The item to be selected.</param>
        /// <param name="reset">if set to <c>true</c> all other items are deselected.</param>
        public virtual void SelectItem(object item, bool reset = false)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var items = this.ItemsView.OfType<IDataViewModel>().ToList();
            items.ForEach(
                i =>
                i.IsItemSelected =
                reset ? object.ReferenceEquals(i, item) : (i.IsItemSelected || object.ReferenceEquals(i, item)));
        }

        /// <summary>
        /// Creates the view of the items displayed in the list.
        /// </summary>
        /// <returns>The view of the items displayed in the list.</returns>
        protected abstract ICollectionView CreateItemsView();

        private void Sort(object parameter)
        {
            var column = parameter as string;
            if (string.IsNullOrWhiteSpace(column))
            {
                // TODO: log
            }

            this.ItemsView.SortDescriptions.Clear();
            this.ItemsView.SortDescriptions.Add(new SortDescription(column, this.SortDirection));
            switch (this.SortDirection)
            {
                case ListSortDirection.Ascending:
                    this.SortDirection = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    this.SortDirection = ListSortDirection.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
