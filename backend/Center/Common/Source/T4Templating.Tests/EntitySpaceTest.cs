// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitySpaceTest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntitySpaceTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.T4Templating.TestsT4Templating.Tests
{
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.T4Templating;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines tests for the <see cref="EntitySpace"/> class.
    /// </summary>
    [TestClass]
    public class EntitySpaceTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Tests the loading of the entity space with the SimpleTest.xml file.
        /// </summary>
        [TestMethod]
        [DeploymentItem("SimpleTest.xml")]
        public void SimpleTest()
        {
            var xmlResource = this.GetResource("SimpleTest.xml");
            var entitySpace = EntitySpace.Load(xmlResource);
            Assert.AreEqual(1, entitySpace.Partitions.Count, "EntitySpace doesn't contain 1 partition as expected");
            var partition = entitySpace.Partitions.Single();
            Assert.AreEqual("Partition1", partition.Name, "Partition name not valid");
            Assert.AreEqual(1, partition.Entities.Count, "The partition doesn't contain 1 entity as expected");
            var entity = partition.Entities.Single();
            Assert.AreEqual("Entity1", entity.Name, "Entity name not valid");
            Assert.AreSame(partition, entity.Partition);
            Assert.AreEqual(1, entity.Properties.Count, "The entity doesn't contain 1 property as expected");
            var property = entity.Properties.Single();
            Assert.AreEqual("Property1", property.Name, "Property name not valid");
        }

        private Stream GetResource(string name)
        {
            var fullName = Path.Combine(this.TestContext.DeploymentDirectory, name);
            var memoryStream = new MemoryStream();
            using (var stream = File.Open(fullName, FileMode.Open))
            {
                stream.CopyTo(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}