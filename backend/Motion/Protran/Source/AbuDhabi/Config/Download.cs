// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Download.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of the settings about a download process.
    /// </summary>
    [Serializable]
    public class Download
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Download"/> class.
        /// </summary>
        public Download()
        {
            this.SourceDirectory = "./";
            this.DestinationDirectory = "D:\\UpdateData";
            this.CuExtensions = new List<string>();
            this.TopboxExtensions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the XML field called SourceDirectory
        /// (alphanumeric string.)
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the XML field(s) called Extension.
        /// </summary>
        [XmlElement("CuExtension")]
        public List<string> CuExtensions { get; set; }

        /// <summary>
        /// Gets or sets the XML field(s) called Extension.
        /// </summary>
        [XmlElement("TopboxExtension")]
        public List<string> TopboxExtensions { get; set; }

        /// <summary>
        /// Gets or sets the destination directory.
        /// </summary>
        public string DestinationDirectory { get; set; }
    }
}
