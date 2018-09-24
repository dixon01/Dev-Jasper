// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisServiceLocatorConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisServiceLocatorConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Discovery
{
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration class base for the configuration of a <see cref="IbisServiceLocatorBase"/>.
    /// <seealso cref="IbisServiceLocatorBase.Configure"/>
    /// </summary>
    public abstract class IbisServiceLocatorConfigBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of received HTTP requests.
        /// </summary>
        [XmlAttribute]
        public bool ValidateHttpRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of created HTTP responses.
        /// </summary>
        [XmlAttribute]
        public bool ValidateHttpResponses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to verify the version
        /// (the <c>ver=x.y</c> in the TXT record) in services provided through DNS-SD.
        /// </summary>
        [XmlAttribute]
        public bool VerifyVersion { get; set; }
    }
}
