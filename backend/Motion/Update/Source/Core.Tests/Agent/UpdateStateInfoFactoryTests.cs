// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStateInfoFactoryTests.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStateInfoFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;
    using Gorba.Motion.Update.Core.Agent;
    using Gorba.Motion.Update.Core.Tests.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests that the <see cref="UpdateStateInfoFactory"/> behaves as defined.
    /// </summary>
    [TestClass]
    public class UpdateStateInfoFactoryTests
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when the root directory is empty.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateStateInfo_NothingUpdated()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderA.State);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderC.State);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileD.State);
            Assert.IsNull(fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileB.State);
            Assert.IsNull(fileB.Hash);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileC.State);
            Assert.IsNull(fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileA.State);
            Assert.IsNull(fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderB.State);
            Assert.AreEqual(1, folderB.Items.Count);

            var folderE = folderB.Items[0] as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when the root directory already contains all items.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateStateInfo_EverythingUpdated()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is A");
            FileSystemMock.CreateDirectory(root, @"B\E");

            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderA.State);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderC.State);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileD.State);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileB.State);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.Hash);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileC.State);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileA.State);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderB.State);
            Assert.AreEqual(1, folderB.Items.Count);

            var folderE = folderB.Items[0] as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when there is a file too much available.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateUpdateStateInfo_NotDeleted()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is A");
            FileSystemMock.CreateDirectory(root, @"B\E");
            FileSystemMock.CreateFile(root, @"B\e.txt", "This is E");

            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderA.State);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderC.State);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileD.State);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileB.State);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.Hash);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileC.State);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileA.State);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.PartiallyUpdated, folderB.State);
            Assert.AreEqual(2, folderB.Items.Count);

            var folderE = folderB.Items.Find(n => n.Name == "E") as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);

            var fileE = folderB.Items.Find(n => n.Name == "e.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileE);
            Assert.AreEqual("e.txt", fileE.Name);
            Assert.AreEqual(ItemUpdateState.NotDeleted, fileE.State);
            Assert.AreEqual("ADA6C873F59B2001986BBE6DE439DCB9", fileE.Hash);
            Assert.IsNull(fileE.ExpectedHash);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when some files should have been updated/deleted/created.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateUpdateStateInfo_PartialUpdate()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is not B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\C\e.txt", "This is E");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is not A");
            FileSystemMock.CreateDirectory(root, @"B\E");

            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.PartiallyUpdated, folderA.State);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.PartiallyUpdated, folderC.State);
            Assert.AreEqual(4, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileD.State);
            Assert.IsNull(fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.NotUpdated, fileB.State);
            Assert.AreEqual("AC14FFF6BE5A2F517895B931855494A0", fileB.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileC.State);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileE = folderC.Items.Find(n => n.Name == "e.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileE);
            Assert.AreEqual("e.txt", fileE.Name);
            Assert.AreEqual(ItemUpdateState.NotDeleted, fileE.State);
            Assert.AreEqual("ADA6C873F59B2001986BBE6DE439DCB9", fileE.Hash);
            Assert.IsNull(fileE.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.NotUpdated, fileA.State);
            Assert.AreEqual("6FFA656FC5AF1F8F692C55F469479764", fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderB.State);
            Assert.AreEqual(1, folderB.Items.Count);

            var folderE = folderB.Items[0] as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when there are files where there should be folders and vice versa.
        /// </summary>
        [TestMethod]
        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Unit Test Method")]
        public void TestCreateUpdateStateInfo_FileFolder()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateDirectory(root, @"A\a.txt");
            FileSystemMock.CreateFile(root, @"B\E", "This is E");

            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.PartiallyUpdated, folderA.State);
            Assert.AreEqual(3, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderC.State);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileD.State);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileB.State);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.Hash);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileC.State);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt" && n is FileUpdateInfo) as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, fileA.State);
            Assert.IsNull(fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderATxt = folderA.Items.Find(n => n.Name == "a.txt" && n is FolderUpdateInfo) as FolderUpdateInfo;
            Assert.IsNotNull(folderATxt);
            Assert.AreEqual("a.txt", folderATxt.Name);
            Assert.AreEqual(ItemUpdateState.NotDeleted, folderATxt.State);
            Assert.AreEqual(0, folderATxt.Items.Count);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.NotUpdated, folderB.State);
            Assert.AreEqual(2, folderB.Items.Count);

            var fileE = folderB.Items.Find(n => n.Name == "E" && n is FileUpdateInfo) as FileUpdateInfo;
            Assert.IsNotNull(fileE);
            Assert.AreEqual("E", fileE.Name);
            Assert.AreEqual(ItemUpdateState.NotDeleted, fileE.State);
            Assert.AreEqual("ADA6C873F59B2001986BBE6DE439DCB9", fileE.Hash);
            Assert.IsNull(fileE.ExpectedHash);

            var folderE = folderB.Items.Find(n => n.Name == "E" && n is FolderUpdateInfo) as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.NotCreated, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateStateInfoFactory.CreateFeedback"/>
        /// behaves as defined when the root directory already contains all items, but
        /// has some additional files and folders (only in root).
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateStateInfo_AdditionalRootItem()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is A");
            FileSystemMock.CreateDirectory(root, @"B\E");

            // additional items
            FileSystemMock.CreateDirectory(root, @"New");
            FileSystemMock.CreateFile(root, @"New.txt", "This is new");

            var command = CreateUpdateCommand();

            var target = new UpdateStateInfoFactory(command, root);
            var feedback = target.CreateFeedback(UpdateState.PartiallyInstalled, "Couldn't install everything");

            Assert.AreEqual(2, feedback.Folders.Count);

            var folderA = feedback.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderA.State);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as FolderUpdateInfo;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderC.State);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as FolderUpdateInfo;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderD.State);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as FileUpdateInfo;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileD.State);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.Hash);
            Assert.AreEqual("A16FE846EF0607B17F50A07FEACA5559", fileD.ExpectedHash);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileB.State);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.Hash);
            Assert.AreEqual("189695EBB418847603AD6953B3BC876F", fileB.ExpectedHash);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileC.State);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.Hash);
            Assert.AreEqual("6C38A71A20767230981DD499326EDC0E", fileC.ExpectedHash);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as FileUpdateInfo;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, fileA.State);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.Hash);
            Assert.AreEqual("3FC606EAEB6BD188C17CBDBEE65ACBF3", fileA.ExpectedHash);

            var folderB = feedback.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderB.State);
            Assert.AreEqual(1, folderB.Items.Count);

            var folderE = folderB.Items[0] as FolderUpdateInfo;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ItemUpdateState.UpToDate, folderE.State);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Creates the following tree:
        /// + A
        ///   + C
        ///     + D
        ///       - d.txt
        ///     - b.txt
        ///     - c.txt
        ///   - a.txt
        /// + B
        ///   + E
        /// </summary>
        /// <returns>
        /// The update command containing the tree.
        /// </returns>
        private static UpdateCommand CreateUpdateCommand()
        {
            var command = new UpdateCommand
                              {
                                  UnitId = new UnitId("UnitA"),
                                  UpdateId = new UpdateId(Guid.NewGuid().ToString(), 42)
                              };

            var folderA = new FolderUpdate { Name = "A" };
            var folderB = new FolderUpdate { Name = "B" };
            var folderC = new FolderUpdate { Name = "C" };
            var folderD = new FolderUpdate { Name = "D" };
            var folderE = new FolderUpdate { Name = "E" };

            var fileA = new FileUpdate
            {
                Name = "a.txt",
                Hash = "3FC606EAEB6BD188C17CBDBEE65ACBF3" // hash for "This is A"
            };
            var fileB = new FileUpdate
            {
                Name = "b.txt",
                Hash = "189695EBB418847603AD6953B3BC876F" // hash for "This is B"
            };
            var fileC = new FileUpdate
            {
                Name = "c.txt",
                Hash = "6C38A71A20767230981DD499326EDC0E" // hash for "This is C"
            };
            var fileD = new FileUpdate
            {
                Name = "d.txt",
                Hash = "A16FE846EF0607B17F50A07FEACA5559" // hash for "This is D"
            };

            folderA.Items.Add(fileA);
            folderC.Items.Add(fileB);
            folderC.Items.Add(fileC);
            folderD.Items.Add(fileD);

            command.Folders.Add(folderA);
            command.Folders.Add(folderB);
            folderA.Items.Add(folderC);
            folderC.Items.Add(folderD);
            folderB.Items.Add(folderE);
            return command;
        }

        // ReSharper restore InconsistentNaming
    }
}
