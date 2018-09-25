// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSetFactoryTests.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSetFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using System.Linq;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;
    using Gorba.Motion.Update.Core.Agent;
    using Gorba.Motion.Update.Core.Tests.Utility;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests that the <see cref="UpdateSetFactory"/> behaves as defined.
    /// </summary>
    [TestClass]
    public class UpdateSetFactoryTests
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when the root directory is empty.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_Complete()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            var command = CreateUpdateCommand();

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(2, updateSet.Folders.Count);

            var folderA = updateSet.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ActionType.Create, folderA.Action);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as UpdateFolder;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ActionType.Create, folderC.Action);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as UpdateFolder;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ActionType.Create, folderD.Action);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as UpdateFile;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ActionType.Create, fileD.Action);
            Assert.AreEqual(new ResourceId("A16FE846EF0607B17F50A07FEACA5559"), fileD.ResourceId);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as UpdateFile;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ActionType.Create, fileB.Action);
            Assert.AreEqual(new ResourceId("189695EBB418847603AD6953B3BC876F"), fileB.ResourceId);

            var fileC = folderC.Items.Find(n => n.Name == "c.txt") as UpdateFile;
            Assert.IsNotNull(fileC);
            Assert.AreEqual("c.txt", fileC.Name);
            Assert.AreEqual(ActionType.Create, fileC.Action);
            Assert.AreEqual(new ResourceId("6C38A71A20767230981DD499326EDC0E"), fileC.ResourceId);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as UpdateFile;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ActionType.Create, fileA.Action);
            Assert.AreEqual(new ResourceId("3FC606EAEB6BD188C17CBDBEE65ACBF3"), fileA.ResourceId);

            var folderB = updateSet.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ActionType.Create, folderB.Action);
            Assert.AreEqual(1, folderB.Items.Count);

            var folderE = folderB.Items[0] as UpdateFolder;
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ActionType.Create, folderE.Action);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when the root directory already contains all items.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_Empty()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is A");
            FileSystemMock.CreateDirectory(root, @"B\E");

            var command = CreateUpdateCommand();

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(0, updateSet.Folders.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when there is a file too much available.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_Delete()
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

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(1, updateSet.Folders.Count);

            var folderB = updateSet.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ActionType.Update, folderB.Action);
            Assert.AreEqual(1, folderB.Items.Count);

            var fileE = folderB.Items[0] as UpdateFile;
            Assert.IsNotNull(fileE);
            Assert.AreEqual("e.txt", fileE.Name);
            Assert.AreEqual(ActionType.Delete, fileE.Action);
            Assert.IsNull(fileE.ResourceId);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when some files have to be updated/deleted/created.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_Update()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is not B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\C\e.txt", "This is E");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is not A");
            FileSystemMock.CreateDirectory(root, @"B\E");

            var command = CreateUpdateCommand();

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(1, updateSet.Folders.Count);

            var folderA = updateSet.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ActionType.Update, folderA.Action);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderC = folderA.Items.Find(n => n.Name == "C") as UpdateFolder;
            Assert.IsNotNull(folderC);
            Assert.AreEqual("C", folderC.Name);
            Assert.AreEqual(ActionType.Update, folderC.Action);
            Assert.AreEqual(3, folderC.Items.Count);

            var folderD = folderC.Items.Find(n => n.Name == "D") as UpdateFolder;
            Assert.IsNotNull(folderD);
            Assert.AreEqual("D", folderD.Name);
            Assert.AreEqual(ActionType.Create, folderD.Action);
            Assert.AreEqual(1, folderD.Items.Count);

            var fileD = folderD.Items[0] as UpdateFile;
            Assert.IsNotNull(fileD);
            Assert.AreEqual("d.txt", fileD.Name);
            Assert.AreEqual(ActionType.Create, fileD.Action);
            Assert.AreEqual(new ResourceId("A16FE846EF0607B17F50A07FEACA5559"), fileD.ResourceId);

            var fileB = folderC.Items.Find(n => n.Name == "b.txt") as UpdateFile;
            Assert.IsNotNull(fileB);
            Assert.AreEqual("b.txt", fileB.Name);
            Assert.AreEqual(ActionType.Update, fileB.Action);
            Assert.AreEqual(new ResourceId("189695EBB418847603AD6953B3BC876F"), fileB.ResourceId);

            var fileE = folderC.Items.Find(n => n.Name == "e.txt") as UpdateFile;
            Assert.IsNotNull(fileE);
            Assert.AreEqual("e.txt", fileE.Name);
            Assert.AreEqual(ActionType.Delete, fileE.Action);
            Assert.IsNull(fileE.ResourceId);

            var fileA = folderA.Items.Find(n => n.Name == "a.txt") as UpdateFile;
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ActionType.Update, fileA.Action);
            Assert.AreEqual(new ResourceId("3FC606EAEB6BD188C17CBDBEE65ACBF3"), fileA.ResourceId);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when there are files where there should be folders and vice versa.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_FileFolder()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateDirectory(root, @"A\a.txt");
            FileSystemMock.CreateFile(root, @"B\E", "This is E");

            var command = CreateUpdateCommand();

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(2, updateSet.Folders.Count);

            var folderA = updateSet.Folders.Find(f => f.Name == "A");
            Assert.IsNotNull(folderA);
            Assert.AreEqual("A", folderA.Name);
            Assert.AreEqual(ActionType.Update, folderA.Action);
            Assert.AreEqual(2, folderA.Items.Count);

            var folderATxt = folderA.Items.OfType<UpdateFolder>().FirstOrDefault();
            Assert.IsNotNull(folderATxt);
            Assert.AreEqual("a.txt", folderATxt.Name);
            Assert.AreEqual(ActionType.Delete, folderATxt.Action);
            Assert.AreEqual(0, folderATxt.Items.Count);

            var fileA = folderA.Items.OfType<UpdateFile>().FirstOrDefault();
            Assert.IsNotNull(fileA);
            Assert.AreEqual("a.txt", fileA.Name);
            Assert.AreEqual(ActionType.Create, fileA.Action);
            Assert.AreEqual(new ResourceId("3FC606EAEB6BD188C17CBDBEE65ACBF3"), fileA.ResourceId);

            var folderB = updateSet.Folders.Find(f => f.Name == "B");
            Assert.IsNotNull(folderB);
            Assert.AreEqual("B", folderB.Name);
            Assert.AreEqual(ActionType.Update, folderB.Action);
            Assert.AreEqual(2, folderB.Items.Count);

            var fileE = folderB.Items.OfType<UpdateFile>().FirstOrDefault();
            Assert.IsNotNull(fileE);
            Assert.AreEqual("E", fileE.Name);
            Assert.AreEqual(ActionType.Delete, fileE.Action);
            Assert.IsNull(fileE.ResourceId);

            var folderE = folderB.Items.OfType<UpdateFolder>().FirstOrDefault();
            Assert.IsNotNull(folderE);
            Assert.AreEqual("E", folderE.Name);
            Assert.AreEqual(ActionType.Create, folderE.Action);
            Assert.AreEqual(0, folderE.Items.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when the root directory already contains all items, but
        /// has some additional files and folders (only in root).
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_AdditionalRootItem()
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

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(0, updateSet.Folders.Count);
        }

        /// <summary>
        /// Tests that the <see cref="UpdateSetFactory.CreateUpdateSet"/>
        /// behaves as defined when the root directory already contains all items
        /// and there is a CF debug file ("514c36bf-c13e-4091-a3a7-1e566227b20d") in one of the directories.
        /// </summary>
        [TestMethod]
        public void TestCreateUpdateSet_WithCompactFrameworkDebugFile()
        {
            IWritableFileSystem fileSystem = new TestingFileSystem();
            var root = fileSystem.CreateDirectory(@"D:\Root");
            FileSystemMock.CreateFile(root, @"A\C\D\d.txt", "This is D");
            FileSystemMock.CreateFile(root, @"A\C\b.txt", "This is B");
            FileSystemMock.CreateFile(root, @"A\C\c.txt", "This is C");
            FileSystemMock.CreateFile(root, @"A\a.txt", "This is A");
            FileSystemMock.CreateDirectory(root, @"B\E");
            FileSystemMock.CreateFile(root, @"A\C\514c36bf-c13e-4091-a3a7-1e566227b20d", "127.0.0.1");

            var command = CreateUpdateCommand();

            var target = new UpdateSetFactory(root, command, null);
            var updateSet = target.CreateUpdateSet();

            Assert.AreEqual(0, updateSet.Folders.Count);
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
            var command = new UpdateCommand();

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
