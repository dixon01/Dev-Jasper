// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeHelperTests.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TreeHelperTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.Core.Agent;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests that the <see cref="TreeHelper"/> behaves as defined.
    /// </summary>
    [TestClass]
    public class TreeHelperTests
    {
        /// <summary>
        /// Tests that the <see cref="TreeHelper.FindFolder(UpdateSet,string)"/>
        /// behaves as defined.
        /// </summary>
        [TestMethod]
        public void TestFindFolderUpdateSet()
        {
            var updateSet = CreateUpdateSet();

            var folder = TreeHelper.FindFolder(updateSet, @"\A");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"\A\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"A\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"A");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"\B\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("B", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"\C\");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateSet, @"\B\C");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateSet, @"\A\C");
            Assert.IsNotNull(folder);
            Assert.AreEqual("C", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"\A\C\D\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("D", folder.Name);

            folder = TreeHelper.FindFolder(updateSet, @"\A\a.txt");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateSet, @"\A\C\D\d.txt");
            Assert.IsNull(folder);
        }

        /// <summary>
        /// Tests that the <see cref="TreeHelper.FindFolder(UpdateCommand,string)"/>
        /// behaves as defined.
        /// </summary>
        [TestMethod]
        public void TestFindFolderUpdateCommand()
        {
            var updateCommand = CreateUpdateCommand();

            var folder = TreeHelper.FindFolder(updateCommand, @"\A");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"\A\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"A\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"A");
            Assert.IsNotNull(folder);
            Assert.AreEqual("A", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"\B\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("B", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"\C\");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateCommand, @"\B\C");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateCommand, @"\A\C");
            Assert.IsNotNull(folder);
            Assert.AreEqual("C", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"\A\C\D\");
            Assert.IsNotNull(folder);
            Assert.AreEqual("D", folder.Name);

            folder = TreeHelper.FindFolder(updateCommand, @"\A\a.txt");
            Assert.IsNull(folder);

            folder = TreeHelper.FindFolder(updateCommand, @"\A\C\D\d.txt");
            Assert.IsNull(folder);
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
        /// </summary>
        /// <returns>
        /// The update set containing the tree.
        /// </returns>
        private static UpdateSet CreateUpdateSet()
        {
            var updateSet = new UpdateSet();

            var folderA = new UpdateFolder(null) { Name = "A" };
            var folderB = new UpdateFolder(null) { Name = "B" };
            var folderC = new UpdateFolder(null) { Name = "C" };
            var folderD = new UpdateFolder(null) { Name = "D" };

            var fileA = new UpdateFile(folderA)
                            {
                                Name = "a.txt",
                                Action = ActionType.Create,
                                ResourceId = new ResourceId("7B14C6DDD9F00D78CCC53B29DBD01C39")
                            };
            var fileB = new UpdateFile(folderC)
                            {
                                Name = "b.txt",
                                Action = ActionType.Create,
                                ResourceId = new ResourceId("58F9590C091B9857722D2733EA24C2F3")
                            };
            var fileC = new UpdateFile(folderC)
                            {
                                Name = "c.txt",
                                Action = ActionType.Create,
                                ResourceId = new ResourceId("AA224F2BE32EA556282E24824771EB04")
                            };
            var fileD = new UpdateFile(folderD)
                            {
                                Name = "d.txt",
                                Action = ActionType.Create,
                                ResourceId = new ResourceId("C6A5F5DB7AACC7FEE713488A09F131C4")
                            };

            folderA.Items.Add(fileA);
            folderC.Items.Add(fileB);
            folderC.Items.Add(fileC);
            folderD.Items.Add(fileD);

            updateSet.Folders.Add(folderA);
            updateSet.Folders.Add(folderB);
            folderA.Items.Add(folderC);
            folderC.Items.Add(folderD);
            return updateSet;
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

            var fileA = new FileUpdate
            {
                Name = "a.txt",
                Hash = "7B14C6DDD9F00D78CCC53B29DBD01C39"
            };
            var fileB = new FileUpdate
            {
                Name = "b.txt",
                Hash = "58F9590C091B9857722D2733EA24C2F3"
            };
            var fileC = new FileUpdate
            {
                Name = "c.txt",
                Hash = "AA224F2BE32EA556282E24824771EB04"
            };
            var fileD = new FileUpdate
            {
                Name = "d.txt",
                Hash = "C6A5F5DB7AACC7FEE713488A09F131C4"
            };

            folderA.Items.Add(fileA);
            folderC.Items.Add(fileB);
            folderC.Items.Add(fileC);
            folderD.Items.Add(fileD);

            command.Folders.Add(folderA);
            command.Folders.Add(folderB);
            folderA.Items.Add(folderC);
            folderC.Items.Add(folderD);
            return command;
        }
    }
}
