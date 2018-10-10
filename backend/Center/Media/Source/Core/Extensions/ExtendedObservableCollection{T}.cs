// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedObservableCollection{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework;

    /// <summary>
    /// Extends the <see cref="ObservableCollection{T}"/> by adding a range of items and only firing one event.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items.
    /// </typeparam>
    public class ExtendedObservableCollection<T> : ObservableCollection<T>, IDirty
    {
        private bool suppressNotification;

        private bool isDirty;

        private bool listenOnChildChanged;

        /// <summary>
        /// The changed child handler.
        /// </summary>
        public delegate void ChangedChildHandler();

        /// <summary>
        /// The child changed.
        /// </summary>
        public event ChangedChildHandler ChildChanged;

        /// <summary>
        /// Gets or sets a value indicating whether listen on child changed.
        /// </summary>
        public bool ListenOnChildChanged
        {
            get
            {
                return this.listenOnChildChanged;
            }

            set
            {
                if (this.listenOnChildChanged && value == false)
                {
                    foreach (var item in this.Items)
                    {
                        this.UnsubscribeForChildChangeNotification(item);
                    }
                }
                else if (this.listenOnChildChanged == false && value)
                {
                    foreach (var item in this.Items)
                    {
                        this.SubscribeForChildChangeNotification(item);
                    }
                }

                this.listenOnChildChanged = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes, making it <c>dirty</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return this.isDirty || this.Items.OfType<IDirty>().Any(dirty => dirty.IsDirty);
            }

            private set
            {
                if (value == this.isDirty)
                {
                    return;
                }

                this.isDirty = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsDirty"));
            }
        }

        /// <summary>
        /// Adds a list of items and sends only one <see cref="OnCollectionChanged"/> event with the new items.
        /// </summary>
        /// <param name="newItems">
        /// The list of items to add.
        /// </param>
        public void AddRange(IEnumerable<T> newItems)
        {
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }

            this.suppressNotification = true;
            var list = newItems.ToList();
            foreach (T item in list)
            {
                this.Add(item);
            }

            this.suppressNotification = false;
            var notifyCollectionChangedEventArgs =
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list);
            this.OnCollectionChanged(notifyCollectionChangedEventArgs);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Sorts the collection by removing all elements and re adding them.
        /// </summary>
        /// <param name="comparer">
        /// The comparer.
        /// </param>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentException("No comparer.");
            }

            if (!this.NeedsSorting(comparer))
            {
                return;
            }

            var sortedList = this.OrderBy(e => e, comparer).ToList();

            this.Clear();
            foreach (var sortedItem in sortedList)
            {
                this.Add(sortedItem);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public void MakeDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag. The default behavior clears the flag on the current object and all
        /// its children.
        /// </summary>
        public void ClearDirty()
        {
            var dirtyItems = this.Items.OfType<IDirty>();
            foreach (var dirtyItem in dirtyItems)
            {
                dirtyItem.ClearDirty();
            }

            this.IsDirty = false;
        }

        /// <summary>
        /// The on collection changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.listenOnChildChanged)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var newItem in e.NewItems)
                    {
                        if (newItem is T)
                        {
                            var newNotificationItem = (T)newItem;
                            this.SubscribeForChildChangeNotification(newNotificationItem);
                        }
                        else
                        {
                            throw new NotSupportedException("Unupported type");
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var removedItem in e.OldItems)
                    {
                        if (removedItem is T)
                        {
                            var removedNotificationItem = (T)removedItem;
                            this.UnsubscribeForChildChangeNotification(removedNotificationItem);
                        }
                        else
                        {
                            throw new NotSupportedException("Unupported type");
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    foreach (var removedItem in e.OldItems)
                    {
                        if (removedItem is T)
                        {
                            var removedNotificationItem = (T)removedItem;
                            this.UnsubscribeForChildChangeNotification(removedNotificationItem);
                        }
                        else
                        {
                            throw new NotSupportedException("Unupported type");
                        }
                    }

                    foreach (var newItem in e.NewItems)
                    {
                        if (newItem is T)
                        {
                            var newNotificationItem = (T)newItem;
                            this.SubscribeForChildChangeNotification(newNotificationItem);
                        }
                        else
                        {
                            throw new NotSupportedException("Supported type");
                        }
                    }
                }
            }

            if (!this.suppressNotification)
            {
                this.RegisterIsDirtyPropertyChanged(e);
                base.OnCollectionChanged(e);
            }
        }

        /// <summary>
        /// Clears all items ensuring that for each removed item an event is raised.
        /// </summary>
        protected override void ClearItems()
        {
            while (this.Count > 0)
            {
                this.Remove(this.First());
            }

            this.MakeDirty();
        }

        /// <summary>
        /// Registers to the property changed event for the <c>IsDirty</c> flag.
        /// </summary>
        /// <param name="notifyPropertyChanged">
        /// The notify property changed object that could raise the property changed event for the
        /// <c>IsDirty</c> flag.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notifyPropertyChanged"/> argument is null.
        /// </exception>
        protected virtual void RegisterIsDirtyPropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            if (notifyPropertyChanged == null)
            {
                return;
            }

            notifyPropertyChanged.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// Registers to the property changed event for the <c>IsDirty</c> flag.
        /// </summary>
        /// <param name="notifyPropertyChanged">
        /// The notify property changed object that could raise the property changed event for the
        /// <c>IsDirty</c> flag.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="notifyPropertyChanged"/> argument is null.
        /// </exception>
        protected virtual void UnregisterIsDirtyPropertyChanged(INotifyPropertyChanged notifyPropertyChanged)
        {
            if (notifyPropertyChanged == null)
            {
                return;
            }

            notifyPropertyChanged.PropertyChanged -= this.OnPropertyChanged;
        }

        private void RegisterIsDirtyPropertyChanged(NotifyCollectionChangedEventArgs notifyPropertyChanged)
        {
            switch (notifyPropertyChanged.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.RegisterIsDirtyPropertyChangedOnAdd(notifyPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.RegisterIsDirtyPropertyChangedOnRemove(notifyPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.RegisterIsDirtyPropertyChangedOnReplace(notifyPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.RegisterIsDirtyPropertyChangedOnMove(notifyPropertyChanged);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.RegisterIsDirtyPropertyChangedOnReset(notifyPropertyChanged);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RegisterIsDirtyPropertyChangedOnAdd(NotifyCollectionChangedEventArgs eventArgs)
        {
            eventArgs.NewItems.OfType<IDirty>().ToList().ForEach(this.RegisterIsDirtyPropertyChanged);
            this.MakeDirty();
        }

        private void RegisterIsDirtyPropertyChangedOnRemove(NotifyCollectionChangedEventArgs eventArgs)
        {
            eventArgs.OldItems.OfType<IDirty>().ToList().ForEach(this.UnregisterIsDirtyPropertyChanged);
            this.MakeDirty();
        }

        private void RegisterIsDirtyPropertyChangedOnReplace(NotifyCollectionChangedEventArgs eventArgs)
        {
            // TODO
        }

        private void RegisterIsDirtyPropertyChangedOnMove(NotifyCollectionChangedEventArgs eventArgs)
        {
            // TODO
        }

        private void RegisterIsDirtyPropertyChangedOnReset(NotifyCollectionChangedEventArgs eventArgs)
        {
            // TODO
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsItemSelected" || e.PropertyName == "IsChildItemSelected"
                || e.PropertyName == "IsExpanded" || e.PropertyName == "CurrentZoomLevel")
            {
                return;
            }

            this.MakeDirty();
        }

        private void OnChildChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.listenOnChildChanged)
            {
                if (e.PropertyName == "IsItemSelected" || e.PropertyName == "IsChildItemSelected"
                    || e.PropertyName == "IsExpanded" || e.PropertyName == "CurrentZoomLevel")
                {
                    return;
                }

                this.MakeDirty();

                if (this.ChildChanged != null)
                {
                    this.ChildChanged();
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var dirtyItems = this.Items.OfType<IDirty>();
            foreach (var item in dirtyItems)
            {
                this.UnregisterIsDirtyPropertyChanged(item);
            }
        }

        private void SubscribeForChildChangeNotification(T item)
        {
            if (this.listenOnChildChanged)
            {
                var child = item as INotifyPropertyChanged;
                if (child != null)
                {
                    child.PropertyChanged += this.OnChildChanged;
                }
            }
        }

        private void UnsubscribeForChildChangeNotification(T item)
        {
            if (this.listenOnChildChanged)
            {
                var child = item as INotifyPropertyChanged;
                if (child != null)
                {
                    child.PropertyChanged -= this.OnChildChanged;
                }
            }
        }

        private bool NeedsSorting(IComparer<T> comparer)
        {
            for (var i = 1; i < this.Count; i++)
            {
                if (comparer.Compare(this[i - 1], this[i]) > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}