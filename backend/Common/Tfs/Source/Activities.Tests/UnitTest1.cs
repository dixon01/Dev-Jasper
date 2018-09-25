// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitTest1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities.Tests
{
    using System.IO;
    using System.Xml;

    using Gorba.Common.Tfs.Activities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines tests for the TFS activities.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Texts the XML reader configuration.
        /// </summary>
        [TestMethod]
        public void TextXmlReaderConfiguration()
        {
            const string Xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><PublishedWebsites xmlns:xsi="
                               + "\"http://www.w3.org/2001/XMLSchema-instance\" xmlns="
                               + "\"http://schemas.gorba.com/tfs/build/publishedWebsitesConfiguration\">"
                               + "<Website deployIisAppPath=\"Default Web Site/BackgroundSystem\" deployServiceUrl="
                               + "\"localhost:8172/msdeploy.axd\" password=\"Gorba\" projectName=\"BackgroundSystem\" "
                               + "username=\"tfs\"/></PublishedWebsites>";
            using (var stringReader = new StringReader(Xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    using (var configurationReader = new PublishedWebsitesConfigurationReader(reader))
                    {
                        var variables = configurationReader.Read("BackgroundSystem");
                        Assert.AreEqual("Default Web Site/BackgroundSystem", variables.DeployIisAppPath);
                        Assert.AreEqual("BackgroundSystem", variables.ProjectName);
                        Assert.AreEqual("tfs", variables.Username);
                    }
                }
            }
        }
    }
}
