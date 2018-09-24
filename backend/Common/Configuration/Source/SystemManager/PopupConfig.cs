// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PopupConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a single popup window that should be prevented.
    /// </summary>
    [Serializable]
    public class PopupConfig
    {
        /// <summary>
        /// Gets or sets the class name of the window to close or empty if
        /// the window should be searched by <see cref="WindowName"/> only.
        /// </summary>
        [XmlAttribute]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets the window name (caption) of the window to close or empty if
        /// the window should be searched by <see cref="ClassName"/> only.
        /// </summary>
        [XmlAttribute]
        public string WindowName { get; set; }
    }
}