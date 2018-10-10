// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedObservableCollectionTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExtendedObservableCollectionTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.Extensions
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The extended observable collection test.
    /// </summary>
    [TestClass]
    public class ExtendedObservableCollectionTest
    {
        /// <summary>
        /// Verifies the setting of the <c>IsDirty</c> flag if the collection is changed.
        /// </summary>
        [TestMethod]
        public void IsDirtyTest()
        {
            var dataViewModelTest = new DataViewModelTest();
            var collection = new ExtendedObservableCollection<DataViewModelTest>();
            Assert.IsFalse(collection.IsDirty, "The collection is dirty when created");
            collection.Add(dataViewModelTest);
            Assert.IsTrue(collection.IsDirty, "The collection is not dirty after adding an element");
            collection.ClearDirty();
            Assert.IsFalse(collection.IsDirty, "The collection is still dirty after clearing");
            dataViewModelTest.MakeDirty();
            Assert.IsTrue(collection.IsDirty, "The collection is not dirty when an inner element becomes dirty");
            collection.ClearDirty();
            Assert.IsFalse(collection.IsDirty, "The collection is still dirty after clearing");
            dataViewModelTest.IsValueChanged = true;
            Assert.IsTrue(collection.IsDirty, "The collection is not dirty when an inner element changes its value(s)");
            collection.ClearDirty();
            Assert.IsFalse(collection.IsDirty, "The collection is still dirty after clearing");
            collection.Remove(dataViewModelTest);
            Assert.IsTrue(collection.IsDirty, "The collection is not dirty after removing an element");
        }

        private class DataViewModelTest : DataViewModelBase
        {
            private bool isValueChanged;

            public bool IsValueChanged
            {
                get
                {
                    return this.isValueChanged;
                }

                set
                {
                    this.SetProperty(ref this.isValueChanged, value, () => this.IsValueChanged);
                }
            }
        }
    }
}