// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeAnimation.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyChangeAnimation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Common
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Animation of a property.
    /// </summary>
    [Serializable]
    public class PropertyChangeAnimation
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
