// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableNotifiableCollection.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.Common.MVVM.ViewModelHelpers
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class ObservableNotifiableCollection<T> : ObservableCollection<T> where T : class, INotifyPropertyChanged
    {

        public event EventHandler<ItemPropertyChangedEventArgs> ItemChanged;



        protected override void ClearItems()
        {

            foreach (var item in this.Items)
            {

                item.PropertyChanged -= this.ItemPropertyChanged;

            }

            base.ClearItems();

        }



        protected override void SetItem(int index, T item)
        {

            this.Items[index].PropertyChanged -= this.ItemPropertyChanged;

            base.SetItem(index, item);

            this.Items[index].PropertyChanged += this.ItemPropertyChanged;

        }



        protected override void RemoveItem(int index)
        {

            this.Items[index].PropertyChanged -= this.ItemPropertyChanged;

            base.RemoveItem(index);

        }



        protected override void InsertItem(int index, T item)
        {

            base.InsertItem(index, item);

            item.PropertyChanged += this.ItemPropertyChanged;

        }



        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            T changedItem = sender as T;

            this.OnItemChanged(this.IndexOf(changedItem), e.PropertyName);

        }



        private void OnItemChanged(int index, string propertyName)
        {

            if (this.ItemChanged != null)
            {

                this.ItemChanged(this, new ItemPropertyChangedEventArgs(index, propertyName));

            }

        }

    }
}