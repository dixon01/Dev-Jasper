// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderLayerHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This is a test class for  and is intended
//   to contain all  Unit Tests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for <see cref="OrderLayerHistoryEntry"/> and is intended
    /// to contain all <see cref="OrderLayerHistoryEntry"/> Unit Tests
    /// </summary>
    [TestClass]
    public class OrderLayerHistoryEntryTest
    {
        /// <summary>
        /// Tests the Undo and Redo of layer ordering moving 2 elements somewhere in the middle of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoElementsMiddleTest()
        {
            var text1 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 5 },
                                ElementName = new DataValue<string>("Text1")
                            };
            var text2 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 4 },
                                ElementName = new DataValue<string>("Text2")
                            };
            var text3 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 3 },
                                ElementName = new DataValue<string>("Text3")
                            };
            var text4 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 2 },
                                ElementName = new DataValue<string>("Text4")
                            };
            var text5 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 1 },
                                ElementName = new DataValue<string>("Text5")
                            };
            var text6 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 0 },
                                ElementName = new DataValue<string>("Text6")
                            };
            var elementList = new List<DrawableElementDataViewModelBase> { text1, text2, text3, text4, text5, text6 };
            var dataList = new List<object> { text4, text5 };
            var selectionList = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            const int InsertIndex = 1;
            var entry = new OrderLayerHistoryEntry(
                InsertIndex,
                elementList,
                dataList,
                selectionList,
                "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("Text3", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("Text4", elementList[3].ElementName.Value);
            Assert.AreEqual("Text5", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("Text3", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);
        }

        /// <summary>
        /// Tests moving 2 elements to the top of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoElementsMoveToTopTest()
        {
            var text1 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 5 },
                                ElementName = new DataValue<string>("Text1")
                            };
            var text2 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 4 },
                                ElementName = new DataValue<string>("Text2")
                            };
            var text3 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 3 },
                                ElementName = new DataValue<string>("Text3")
                            };
            var text4 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 2 },
                                ElementName = new DataValue<string>("Text4")
                            };
            var text5 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 1 },
                                ElementName = new DataValue<string>("Text5")
                            };
            var text6 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 0 },
                                ElementName = new DataValue<string>("Text6")
                            };
            var elementList = new List<DrawableElementDataViewModelBase> { text1, text2, text3, text4, text5, text6 };
            var dataList = new List<object> { text4, text5 };
            var selectionList = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            const int InsertIndex = 0;
            var entry = new OrderLayerHistoryEntry(
                InsertIndex,
                elementList,
                dataList,
                selectionList,
                "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text4", elementList[0].ElementName.Value);
            Assert.AreEqual("Text5", elementList[1].ElementName.Value);
            Assert.AreEqual("Text1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("Text3", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("Text4", elementList[3].ElementName.Value);
            Assert.AreEqual("Text5", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Redo
            entry.Do();
            Assert.AreEqual("Text4", elementList[0].ElementName.Value);
            Assert.AreEqual("Text5", elementList[1].ElementName.Value);
            Assert.AreEqual("Text1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("Text3", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);
        }

        /// <summary>
        /// Tests moving 2 elements to the end of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoElementsMoveToBottomTest()
        {
            var text1 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 5 },
                                ElementName = new DataValue<string>("Text1")
                            };
            var text2 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 4 },
                                ElementName = new DataValue<string>("Text2")
                            };
            var text3 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 3 },
                                ElementName = new DataValue<string>("Text3")
                            };
            var text4 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 2 },
                                ElementName = new DataValue<string>("Text4")
                            };
            var text5 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 1 },
                                ElementName = new DataValue<string>("Text5")
                            };
            var text6 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 0 },
                                ElementName = new DataValue<string>("Text6")
                            };
            var elementList = new List<DrawableElementDataViewModelBase> { text1, text2, text3, text4, text5, text6 };
            var dataList = new List<object> { text2, text3 };
            const int InsertIndex = 6;
            var selectionList = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            var entry = new OrderLayerHistoryEntry(
                InsertIndex,
                elementList,
                dataList,
                selectionList,
                "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text6", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("Text4", elementList[3].ElementName.Value);
            Assert.AreEqual("Text5", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text6", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);
        }

        /// <summary>
        /// Tests the Undo and Redo of layer ordering if the items were selected from bottom to top.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoElementsSelectedFromBottomToTopTest()
        {
            var text1 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 5 },
                                ElementName = new DataValue<string>("Text1")
                            };
            var text2 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 4 },
                                ElementName = new DataValue<string>("Text2")
                            };
            var text3 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 3 },
                                ElementName = new DataValue<string>("Text3")
                            };
            var text4 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 2 },
                                ElementName = new DataValue<string>("Text4")
                            };
            var text5 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 1 },
                                ElementName = new DataValue<string>("Text5")
                            };
            var text6 = new StaticTextElementDataViewModel(null)
                            {
                                ZIndex = { Value = 0 },
                                ElementName = new DataValue<string>("Text6")
                            };
            var elementList = new List<DrawableElementDataViewModelBase> { text1, text2, text3, text4, text5, text6 };
            var dataList = new List<object> { text3, text2 };
            const int InsertIndex = 6;
            var selectionList = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            var entry = new OrderLayerHistoryEntry(
                InsertIndex,
                elementList,
                dataList,
                selectionList,
                "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text6", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("Text4", elementList[3].ElementName.Value);
            Assert.AreEqual("Text5", elementList[4].ElementName.Value);
            Assert.AreEqual("Text6", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text4", elementList[1].ElementName.Value);
            Assert.AreEqual("Text5", elementList[2].ElementName.Value);
            Assert.AreEqual("Text6", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
            this.VerifySelectedElements(selectionList, dataList);
        }

        private void VerifySelectedElements(
            ExtendedObservableCollection<LayoutElementDataViewModelBase> selectionList,
            List<object> movedElements)
        {
            foreach (var movedElement in movedElements)
            {
                Assert.IsTrue(selectionList.Contains(movedElement));
            }
        }
    }
}
