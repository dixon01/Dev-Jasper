// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Behaviour.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of the settings about all the software's behaviors.
    /// </summary>
    [Serializable]
    public class Behaviour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Behaviour"/> class.
        /// </summary>
        public Behaviour()
        {
            this.CheckCrc = true;
            this.ByteType = ByteType.Ascii7;
            this.ConnectionTimeOut = TimeSpan.FromSeconds(60);
            this.ProcessPriority = ProcessPriorityClass.AboveNormal;
            this.IbisAddresses = new List<int>();
        }

        /// <summary>
        /// Gets or sets the XML field(s) called IbisAddress
        /// Values admitted: {0 ... 15} Default: 8.
        /// </summary>
        [XmlElement("IbisAddress")]
        public List<int> IbisAddresses { get; set; }

        /// <summary>
        /// Gets or sets the field called ConnectionTimeOut.
        /// Values admitted: positive, non-zero timespan. Default: 60 seconds.
        /// </summary>
        [XmlIgnore]
        public TimeSpan ConnectionTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the XML field called ConnectionTimeOut.
        /// Values admitted: positive, non-zero timespan. Default: 60 seconds.
        /// </summary>
        [XmlElement("ConnectionTimeOut", DataType = "duration")]
        public string ConnectionTimeOutString
        {
            get
            {
                return XmlConvert.ToString(this.ConnectionTimeOut);
            }

            set
            {
                this.ConnectionTimeOut = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the usage of the connection status to fill a generic cell.
        /// </summary>
        public GenericUsage ConnectionStatusUsedFor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// the checksum has to be calculated or not.
        /// Values admitted: {true, false} (case insensitive) Default: true
        /// </summary>
        public bool CheckCrc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we use 8 or 16 bit IBIS telegram.
        /// </summary>
        [Obsolete("Use ByteType instead")]
        public bool Is16Bit
        {
            get
            {
                return this.ByteType == ByteType.UnicodeBigEndian;
            }

            set
            {
                this.ByteType = value ? ByteType.UnicodeBigEndian : ByteType.Ascii7;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we use 7, 8 or 16 bit IBIS telegram.
        /// </summary>
        public ByteType ByteType { get; set; }

        /// <summary>
        /// Gets or sets the priority of the Protran process in case IBIS is used.
        /// Default value is AboveNormal.
        /// </summary>
        public ProcessPriorityClass ProcessPriority { get; set; }
    }
}
