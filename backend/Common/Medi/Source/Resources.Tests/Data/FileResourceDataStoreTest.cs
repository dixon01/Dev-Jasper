// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceDataStoreTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResourceDataStoreTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Tests.Data
{
    using System.IO;
    using System.Linq;
    using System.Text;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Resources.Data;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="FileResourceDataStore"/>.
    /// </summary>
    [TestClass]
    public class FileResourceDataStoreTest
    {
        /// <summary>
        /// Tests the <see cref="FileResourceDataStore.Initialize"/> method.
        /// </summary>
        [TestMethod]
        public void TestInitialize()
        {
            var fileSystem = new TestingFileSystem();

            var rootDirectory = fileSystem.CreateDirectory("C:\\Resources");
            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);
            }
        }

        /// <summary>
        /// Tests the <see cref="FileResourceDataStore.Add"/> and <see cref="FileResourceDataStore.Get"/> methods.
        /// </summary>
        [TestMethod]
        public void TestAddGet()
        {
            var fileSystem = new TestingFileSystem();

            var rootDirectory = fileSystem.CreateDirectory("C:\\Resources");
            var id = new ResourceId("AFD0340B3BB29D0EB04125F3AD0CEA17");
            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                var resource = target.Create(id);
                resource.IsTemporary = false;
                resource.OriginalFileName = "test.txt";
                resource.Size = 245;
                resource.Source = new MediAddress("U", "A");
                resource.State = ResourceState.Announced;
                target.Add(resource);

                Assert.AreEqual(1, target.GetAll().Count());
            }

            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                Assert.AreEqual(1, target.GetAll().Count());

                var resource = target.Get(id);
                Assert.IsNotNull(resource);
                Assert.AreEqual(id, resource.Id);
                Assert.IsFalse(resource.IsTemporary);
                Assert.AreEqual("test.txt", resource.OriginalFileName);
                Assert.AreEqual(245, resource.Size);
                Assert.AreEqual(new MediAddress("U", "A"), resource.Source);
                Assert.AreEqual(ResourceState.Announced, resource.State);
                Assert.IsNotNull(resource.References);
                Assert.IsNull(resource.StoreReference);
                Assert.AreEqual(0, resource.References.Count);
            }
        }

        /// <summary>
        /// Tests the <see cref="FileResourceDataStore.Add"/> and <see cref="FileResourceDataStore.Update"/> methods.
        /// </summary>
        [TestMethod]
        public void TestAddUpdateGet()
        {
            var fileSystem = new TestingFileSystem();

            var rootDirectory = fileSystem.CreateDirectory("C:\\Resources");
            var id = new ResourceId("AFD0340B3BB29D0EB04125F3AD0CEA17");
            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                var resource = target.Create(id);
                resource.IsTemporary = false;
                resource.OriginalFileName = "test.txt";
                resource.Size = 245;
                resource.Source = new MediAddress("U", "A");
                resource.State = ResourceState.Announced;
                target.Add(resource);

                Assert.AreEqual(1, target.GetAll().Count());
            }

            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                Assert.AreEqual(1, target.GetAll().Count());

                var resource = target.Get(id);
                resource.State = ResourceState.Available;
                resource.StoreReference = string.Format("C:\\Resources\\{0}.rx", id.Hash);
                target.Update(resource);
            }

            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                Assert.AreEqual(1, target.GetAll().Count());

                var resource = target.Get(id);
                Assert.IsNotNull(resource);
                Assert.AreEqual(id, resource.Id);
                Assert.IsFalse(resource.IsTemporary);
                Assert.AreEqual("test.txt", resource.OriginalFileName);
                Assert.AreEqual(245, resource.Size);
                Assert.AreEqual(new MediAddress("U", "A"), resource.Source);
                Assert.AreEqual(ResourceState.Available, resource.State);
                Assert.IsNotNull(resource.References);
                Assert.AreEqual("C:\\Resources\\AFD0340B3BB29D0EB04125F3AD0CEA17.rx", resource.StoreReference);
                Assert.AreEqual(0, resource.References.Count);
            }
        }

        /// <summary>
        /// Tests the <see cref="FileResourceDataStore.Add"/> and <see cref="FileResourceDataStore.Get"/> methods
        /// for the case that the primary resource information file was corrupted.
        /// </summary>
        [TestMethod]
        public void TestCorruptedResourceInfo()
        {
            var fileSystem = new TestingFileSystem();

            var rootDirectory = fileSystem.CreateDirectory("C:\\Resources");
            var id = new ResourceId("AFD0340B3BB29D0EB04125F3AD0CEA17");
            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                var resource = target.Create(id);
                resource.IsTemporary = false;
                resource.OriginalFileName = "test.txt";
                resource.Size = 245;
                resource.Source = new MediAddress("U", "A");
                resource.State = ResourceState.Announced;
                target.Add(resource);

                Assert.AreEqual(1, target.GetAll().Count());
            }

            // now we corrupt the file, so the resource store needs to load it from the backup
            var resFile = ((IWritableFileSystem)fileSystem).GetFile(
                Path.Combine(rootDirectory.FullName, id.Hash + ".rin"));
            using (var output = resFile.OpenWrite())
            {
                var corruptData = Encoding.ASCII.GetBytes("<>corrupted");
                output.Write(corruptData, 0, corruptData.Length);
            }

            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                Assert.AreEqual(1, target.GetAll().Count());

                var resource = target.Get(id);
                Assert.IsNotNull(resource);
                Assert.AreEqual(id, resource.Id);
                Assert.IsFalse(resource.IsTemporary);
                Assert.AreEqual("test.txt", resource.OriginalFileName);
                Assert.AreEqual(245, resource.Size);
                Assert.AreEqual(new MediAddress("U", "A"), resource.Source);
                Assert.AreEqual(ResourceState.Announced, resource.State);
                Assert.IsNotNull(resource.References);
                Assert.IsNull(resource.StoreReference);
                Assert.AreEqual(0, resource.References.Count);
            }
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Tests loading a <code>.rin</code> file from version 2.0.1330.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\V2_0_1330.rin")]
        public void TestLoading_V2_0_1330()
        {
            var fileSystem = new TestingFileSystem();

            var rootDirectory = fileSystem.CreateDirectory("C:\\Resources");
            var id = new ResourceId("AFD0340B3BB29D0EB04125F3AD0CEA17");

            var resFile = ((IWritableFileSystem)fileSystem).CreateFile(
                Path.Combine(rootDirectory.FullName, id.Hash + ".rin"));
            using (var output = resFile.OpenWrite())
            {
                var xml = File.ReadAllText("V2_0_1330.rin");
                var data = Encoding.ASCII.GetBytes(xml);
                output.Write(data, 0, data.Length);
            }

            resFile.CopyTo(Path.Combine(rootDirectory.FullName, id.Hash + ".rin.bak"));

            using (var target = new FileResourceDataStore())
            {
                target.Initialize(rootDirectory);

                Assert.AreEqual(1, target.GetAll().Count());

                var resource = target.Get(id);
                Assert.IsNotNull(resource);
                Assert.AreEqual(id, resource.Id);
                Assert.IsFalse(resource.IsTemporary);
                Assert.AreEqual("test.txt", resource.OriginalFileName);
                Assert.AreEqual(245, resource.Size);
                Assert.AreEqual(new MediAddress("U", "A"), resource.Source);
                Assert.AreEqual(ResourceState.Announced, resource.State);
                Assert.IsNotNull(resource.References);
                Assert.IsNull(resource.StoreReference);
                Assert.AreEqual(0, resource.References.Count);
            }
        }

        // ReSharper restore InconsistentNaming
    }
}
