// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublishedWebsitesConfigurationReader.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PublishedWebsitesConfigurationReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Activities
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    /// <summary>
    /// Simple implementation of the <see cref="IPublishedWebsitesConfigurationReader"/>
    /// which takes the configuration from an xml file.
    /// </summary>
    public class PublishedWebsitesConfigurationReader : IPublishedWebsitesConfigurationReader, IDisposable
    {
        /// <summary>
        /// Stores the reference to the <see cref="System.Xml.XmlReader"/>.
        /// </summary>
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedWebsitesConfigurationReader"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        public PublishedWebsitesConfigurationReader(XmlReader reader)
        {
            Contract.Requires(reader != null);
            Contract.Ensures(this.reader != null);
            this.reader = reader;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PublishedWebsitesConfigurationReader"/> class.
        /// </summary>
        ~PublishedWebsitesConfigurationReader()
        {
            this.Dispose(false);
        }

        /// <inheritdoc />
        public WebDeploymentVariables Read(string projectName)
        {
            Contract.Requires(this.reader != null);
            Contract.Assert(this.reader.NameTable != null, "The path cannot be null.");
            var namespaceManager = new XmlNamespaceManager(this.reader.NameTable);
            namespaceManager.AddNamespace("pw", Consts.PublishedWebsitesConfigurationNamespace);
            var document = XDocument.Load(this.reader);
            const string QueryFormat = "/pw:PublishedWebsites/pw:Website[@projectName='{0}']";
            var query = string.Format(QueryFormat, projectName);
            var websiteElement = document.XPathSelectElement(query, namespaceManager);
            Contract.Assert(websiteElement != null, "The website element was not found in the file.");
            var deployIisAppPath = this.GetVariable(websiteElement, "deployIisAppPath");
            var password = this.GetVariable(websiteElement, "password");
            var username = this.GetVariable(websiteElement, "username");
            var webDeploymentVariables = new WebDeploymentVariables
            {
                DeployIisAppPath = deployIisAppPath,
                Password = password,
                ProjectName = projectName,
                Username = username
            };
            return webDeploymentVariables;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current instance.
        /// </summary>
        /// <param name="disposing">If <b>true</b>, managed resources are disposed also.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        private string GetVariable(XElement websiteElement, string variableName)
        {
            Contract.Requires(websiteElement != null);
            Contract.Requires(!string.IsNullOrEmpty(variableName));
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
            var attribute = websiteElement.Attribute(variableName);
            Contract.Assert(attribute != null);
            return attribute.Value;
        }
    }
}