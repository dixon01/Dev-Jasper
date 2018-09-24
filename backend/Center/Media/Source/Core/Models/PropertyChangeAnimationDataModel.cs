// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeAnimationDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The property change animation data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// The property change animation data model.
    /// </summary>
    public class PropertyChangeAnimationDataModel
    {
        /// <summary>
        /// Gets or sets the kind of animation to use.
        /// </summary>
        public PropertyChangeAnimationType Type { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation as an XML serializable string.
        /// </summary>
        [XmlElement("Duration")]
        public string DurationXml
        {
            get
            {
                return XmlConvert.ToString(this.Duration);
            }

            set
            {
                this.Duration = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
