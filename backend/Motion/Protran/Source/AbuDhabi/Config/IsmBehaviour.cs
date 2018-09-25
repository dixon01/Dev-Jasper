// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsmBehaviour.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsmBehaviour type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Behavior tag of the ISM configuration.
    /// </summary>
    [Serializable]
    public class IsmBehaviour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsmBehaviour"/> class.
        /// </summary>
        public IsmBehaviour()
        {
            this.Authentication = new AuthenticationConfig();
            this.Download = new Download();

            this.PollTime = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Gets or sets the authentication information.
        /// </summary>
        public AuthenticationConfig Authentication { get; set; }

        /// <summary>
        /// Gets or sets the download information.
        /// </summary>
        public Download Download { get; set; }

        /// <summary>
        /// Gets or sets PollTime
        /// </summary>
        [XmlIgnore]
        public TimeSpan PollTime { get; set; }

        /// <summary>
        /// Gets or sets PollTime in an XML serializable format.
        /// </summary>
        [XmlElement("PollTime", DataType = "duration")]
        public string PollTimeString
        {
            get
            {
                return XmlConvert.ToString(this.PollTime);
            }

            set
            {
                this.PollTime = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
