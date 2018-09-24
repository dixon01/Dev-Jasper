// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererOptionGroup.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Options
{
    using System.Runtime.Serialization;

    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// The renderer option group.
    /// </summary>
    [DataContract(Name = "RendererGroup")]
    public class RendererOptionGroup : OptionGroupBase
    {
        /// <summary>
        /// Gets or sets the text mode.
        /// </summary>
        [DataMember]
        public string TextMode { get; set; }

        /// <summary>
        /// Gets or sets the font quality.
        /// </summary>
        [DataMember]
        public string FontQuality { get; set; }

        /// <summary>
        /// Gets or sets the video mode.
        /// </summary>
        [DataMember]
        public string VideoMode { get; set; }
    }
}
