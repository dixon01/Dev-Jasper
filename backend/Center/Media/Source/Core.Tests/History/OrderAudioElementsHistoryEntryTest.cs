// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderAudioElementsHistoryEntryTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Tests for <see cref="OrderAudioElementsHistoryEntry" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Tests.History
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.History;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="OrderAudioElementsHistoryEntry"/>.
    /// </summary>
    [TestClass]
    public class OrderAudioElementsHistoryEntryTest
    {
        /// <summary>
        /// Tests the Undo and Redo of audio elements ordering by moving 2 elements somewhere in the middle of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoAudioElementsMiddleTest()
        {
            var elementList = CreateAudioElementsList().ToList();
            var dataList = new List<object> { elementList[2], elementList[3] };
            const int InsertIndex = 1;
            var entry = new OrderAudioElementsHistoryEntry(InsertIndex, elementList, dataList, "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text3", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text3", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);
        }

        /// <summary>
        /// Tests moving 2 audio elements to the top of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoaudioElementsMoveToTopTest()
        {
            var elementList = CreateAudioElementsList().ToList();
            var dataList = new List<object> { elementList[2], elementList[3] };
            const int InsertIndex = 0;
            var entry = new OrderAudioElementsHistoryEntry(InsertIndex, elementList, dataList, "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text3", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("Text1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Redo
            entry.Do();
            Assert.AreEqual("Text3", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("Text1", elementList[2].ElementName.Value);
            Assert.AreEqual("Text2", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);
        }

        /// <summary>
        /// Tests moving 2 audio elements to the end of the list.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoAudioElementsMoveToBottomTest()
        {
            var elementList = CreateAudioElementsList().ToList();
            var dataList = new List<object> { elementList[1], elementList[2] };
            const int InsertIndex = 6;
            var entry = new OrderAudioElementsHistoryEntry(InsertIndex, elementList, dataList, "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[2].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[2].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
        }

        /// <summary>
        /// Tests the Undo and Redo of 2 audio elements ordering if the items were selected from bottom to top.
        /// </summary>
        [TestMethod]
        public void UndoRedoTwoAudioElementsSelectedFromBottomToTopTest()
        {
            var elementList = CreateAudioElementsList().ToList();
            var dataList = new List<object> { elementList[2], elementList[1] };
            const int InsertIndex = 6;
            var entry = new OrderAudioElementsHistoryEntry(InsertIndex, elementList, dataList, "UndoTest");

            // First Do
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[2].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);

            // Undo
            entry.Undo();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("Text2", elementList[1].ElementName.Value);
            Assert.AreEqual("Text3", elementList[2].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[3].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[4].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[5].ElementName.Value);

            // Redo
            entry.Do();
            Assert.AreEqual("Text1", elementList[0].ElementName.Value);
            Assert.AreEqual("AudioFile1", elementList[1].ElementName.Value);
            Assert.AreEqual("AudioFile2", elementList[2].ElementName.Value);
            Assert.AreEqual("Pause1", elementList[3].ElementName.Value);
            Assert.AreEqual("Text2", elementList[4].ElementName.Value);
            Assert.AreEqual("Text3", elementList[5].ElementName.Value);
        }

        private static IEnumerable<AudioElementDataViewModelBase> CreateAudioElementsList()
        {
            var text1 = new TextToSpeechElementDataViewModel(null)
            {
                ListIndex = { Value = 0 },
                ElementName = new DataValue<string>("Text1")
            };
            var text2 = new TextToSpeechElementDataViewModel(null)
            {
                ListIndex = { Value = 1 },
                ElementName = new DataValue<string>("Text2")
            };
            var text3 = new DynamicTtsElementDataViewModel(null)
            {
                ListIndex = { Value = 2 },
                ElementName = new DataValue<string>("Text3")
            };
            var file1 = new AudioFileElementDataViewModel(null)
            {
                ListIndex = { Value = 3 },
                ElementName = new DataValue<string>("AudioFile1")
            };
            var file2 = new AudioFileElementDataViewModel(null)
            {
                ListIndex = { Value = 4 },
                ElementName = new DataValue<string>("AudioFile2")
            };
            var pause1 = new AudioPauseElementDataViewModel(null)
            {
                ListIndex = { Value = 5 },
                ElementName = new DataValue<string>("Pause1")
            };

            return new List<AudioElementDataViewModelBase> { text1, text2, text3, file1, file2, pause1 };
        }
    }
}
