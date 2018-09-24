// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AhdlcRenderer
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for a single sign handled with AHDLC.
    /// </summary>
    [Serializable]
    public class SignConfig
    {
        private int address;

        /// <summary>
        /// Gets or sets the screen id used for this sign.
        /// If this property is null or empty, the <see cref="Address"/> is taken as the screen id.
        /// </summary>
        [XmlAttribute("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the address value is not between 1 and 15.
        /// </exception>
        [XmlAttribute("Address")]
        public int Address
        {
            get
            {
                return this.address;
            }

            set
            {
                if (value < 1 || value > 15)
                {
                    throw new ArgumentOutOfRangeException("value", "Address can only be between 1 and 15");
                }

                this.address = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode of the sign.
        /// </summary>
        [XmlAttribute("Mode")]
        public SignMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the width of the sign in pixels.
        /// </summary>
        [XmlAttribute("Width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the sign in pixels.
        /// </summary>
        [XmlAttribute("Height")]
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the brightness level of the sign.
        /// </summary>
        [XmlAttribute("Brightness")]
        public SignBrightness Brightness { get; set; }
    }
}