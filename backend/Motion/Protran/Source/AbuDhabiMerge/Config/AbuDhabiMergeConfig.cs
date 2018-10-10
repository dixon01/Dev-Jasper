// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiMergeConfig.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration of file extensions for Merge.
    /// </summary>
    [XmlRoot("Merge")]
    public class AbuDhabiMergeConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiMergeConfig"/> class.
        /// </summary>
        public AbuDhabiMergeConfig()
        {
            this.Extensions = new Extensions();
        }

        /// <summary>
        /// Gets or sets the XML element called Extensions.
        /// </summary>
        public Extensions Extensions { get; set; }
    }
}
