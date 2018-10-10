// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSetSerializationTests.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSetSerializationTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Agent
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Motion.Update.Core.Agent;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for XML serialization of <see cref="UpdateSet"/>.
    /// </summary>
    [TestClass]
    public class UpdateSetSerializationTests
    {
        /// <summary>
        /// Tests that the serialization is working properly.
        /// </summary>
        [TestMethod]
        public void TestSerialization()
        {
            var updateSet = new UpdateSet();

            var folderA = new UpdateFolder(null) { Action = ActionType.Create, Name = "A" };

            var fileA = new UpdateFile(folderA)
                            {
                                Action = ActionType.Create,
                                Name = "a.txt",
                                ResourceId = new ResourceId("EAA705D93281BD614259435BBEE590B4")
                            };
            folderA.Items.Add(fileA);

            updateSet.Folders.Add(folderA);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var target = new XmlSerializer(updateSet.GetType());
            var writer = new StringWriter();
            var xmlWriter = XmlWriter.Create(
                writer,
                new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true });
            target.Serialize(xmlWriter, updateSet, namespaces);
        }
    }
}
