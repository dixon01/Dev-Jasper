// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteredObservableCollectionTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FilteredObservableCollectionTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core.Tests.Collections
{
    using System;
    using System.Collections.Specialized;

    using Gorba.Center.Common.Wpf.Core.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="FilteredObservableCollection{T}"/>
    /// </summary>
    [TestClass]
    public class FilteredObservableCollectionTest
    {
        private static readonly Func<Item, bool> Filter = i => i.IsVisible;

        /// <summary>
        /// Tests that the constructor filters correctly.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(1, target[0].Index);
            Assert.AreEqual(3, target[1].Index);
        }

        /// <summary>
        /// Tests adding an item that should show up in the filtered list.
        /// </summary>
        [TestMethod]
        public void TestAddVisibleItem()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            var newItem = new Item(4);
            var changeCount = 0;
            target.CollectionChanged += (sender, args) =>
                {
                    Assert.AreEqual(1, ++changeCount);
                    Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
                    Assert.AreEqual(1, args.NewItems.Count);
                    Assert.AreEqual(newItem, args.NewItems[0]);
                };

            list.Add(newItem);
            Assert.AreEqual(1, changeCount);
            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(2, target.IndexOf(newItem));
        }

        /// <summary>
        /// Tests adding an item that should not show up in the filtered list.
        /// </summary>
        [TestMethod]
        public void TestAddInvisibleItem()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            var newItem = new Item(4, false);
            target.CollectionChanged += (sender, args) => Assert.Fail();

            list.Add(newItem);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(-1, target.IndexOf(newItem));
        }

        /// <summary>
        /// Tests removing an item that was in the filtered list.
        /// </summary>
        [TestMethod]
        public void TestRemoveVisibleItem()
        {
            var list = new ObservableItemCollection<Item>
                           {
                               new Item(1),
                               new Item(2, false),
                               new Item(3)
                           };
            var remoteItem = new Item(4);
            list.Add(remoteItem);
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(3, target.Count);

            var changeCount = 0;
            target.CollectionChanged += (sender, args) =>
                {
                    Assert.AreEqual(1, ++changeCount);
                    Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
                    Assert.AreEqual(1, args.OldItems.Count);
                    Assert.AreEqual(remoteItem, args.OldItems[0]);
                };

            list.Remove(remoteItem);
            Assert.AreEqual(1, changeCount);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(-1, target.IndexOf(remoteItem));
        }

        /// <summary>
        /// Tests removing an item that was not the filtered list.
        /// </summary>
        [TestMethod]
        public void TestRemoveInvisibleItem()
        {
            var list = new ObservableItemCollection<Item>
                           {
                               new Item(1),
                               new Item(2, false),
                               new Item(3)
                           };
            var remoteItem = new Item(4, false);
            list.Add(remoteItem);
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            target.CollectionChanged += (sender, args) => Assert.Fail();

            list.Remove(remoteItem);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(-1, target.IndexOf(remoteItem));
        }

        /// <summary>
        /// Tests moving an item that was in the filtered list.
        /// </summary>
        [TestMethod]
        public void TestMoveVisibleItem()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            var changeCount = 0;
            target.CollectionChanged += (sender, args) =>
                {
                    Assert.AreEqual(1, ++changeCount);
                    Assert.AreEqual(NotifyCollectionChangedAction.Move, args.Action);
                    Assert.AreEqual(1, args.NewItems.Count);
                    Assert.AreEqual(1, ((Item)args.NewItems[0]).Index);
                };

            list.Move(0, 2);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(3, target[0].Index);
            Assert.AreEqual(1, target[1].Index);
        }

        /// <summary>
        /// Tests moving an item that was in the filtered list,
        /// but moving has no effect since the index stays the same.
        /// </summary>
        [TestMethod]
        public void TestMoveVisibleItemNoEffect()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            target.CollectionChanged += (sender, args) => Assert.Fail();

            list.Move(0, 1);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(1, target[0].Index);
            Assert.AreEqual(3, target[1].Index);
        }

        /// <summary>
        /// Tests moving an item that was not in the filtered list.
        /// </summary>
        [TestMethod]
        public void TestMoveInvisibleItem()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            target.CollectionChanged += (sender, args) => Assert.Fail();

            list.Move(1, 0);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(1, target[0].Index);
            Assert.AreEqual(3, target[1].Index);
        }

        /// <summary>
        /// Tests that an item is added to the filtered list
        /// when the filter property changes.
        /// </summary>
        [TestMethod]
        public void TestMakeItemVisible()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(2, target.Count);

            var item = list[1];
            var changeCount = 0;
            target.CollectionChanged += (sender, args) =>
                {
                    Assert.AreEqual(1, ++changeCount);
                    Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
                    Assert.AreEqual(1, args.NewItems.Count);
                    Assert.AreEqual(item, args.NewItems[0]);
                };

            item.IsVisible = true;
            Assert.AreEqual(1, changeCount);
            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(1, target.IndexOf(item));
        }

        /// <summary>
        /// Tests that an item is removed from the filtered list
        /// when the filter property changes.
        /// </summary>
        [TestMethod]
        public void TestMakeItemInvisible()
        {
            var list = new ObservableItemCollection<Item> { new Item(1), new Item(2, false), new Item(3), new Item(4) };
            var target = new FilteredObservableCollection<Item>(list, Filter);

            Assert.AreEqual(3, target.Count);

            var item = list[2];
            var changeCount = 0;
            target.CollectionChanged += (sender, args) =>
                {
                    Assert.AreEqual(1, ++changeCount);
                    Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
                    Assert.AreEqual(1, args.OldItems.Count);
                    Assert.AreEqual(item, args.OldItems[0]);
                };

            item.IsVisible = false;
            Assert.AreEqual(1, changeCount);
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(-1, target.IndexOf(item));
        }

        private class Item : ViewModelBase
        {
            private bool isVisible;

            public Item(int index, bool isVisible = true)
            {
                this.Index = index;
                this.IsVisible = isVisible;
            }

            public int Index { get; private set; }

            public bool IsVisible
            {
                get
                {
                    return this.isVisible;
                }

                set
                {
                    this.SetProperty(ref this.isVisible, value, () => this.IsVisible);
                }
            }
        }
    }
}
