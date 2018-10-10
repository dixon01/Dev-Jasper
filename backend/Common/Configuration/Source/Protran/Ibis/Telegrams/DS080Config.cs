// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS080Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Configuration for the DS080 telegram (door opened)
    /// </summary>
    [Serializable]
    public class DS080Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS080Config"/> class.
        /// </summary>
        public DS080Config()
        {
            this.OpenValue = "1";
            this.CloseValue = "0";
            this.ResetWithDS010B = true;
        }

        /// <summary>
        /// Gets or sets the value to be set when the DS080 telegram is received.
        /// </summary>
        public string OpenValue { get; set; }

        /// <summary>
        /// Gets or sets the value to be set when the DS010B telegram is received
        /// and <see cref="ResetWithDS010B"/> is enabled.
        /// </summary>
        public string CloseValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DS010b telegram is handled
        /// and when it changes the <see cref="CloseValue"/> will be emit.
        /// </summary>
        [XmlElement("ResetWithDS010b")]
        public bool ResetWithDS010B { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }
    }
}
