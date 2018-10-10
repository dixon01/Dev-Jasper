// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about an IBIS telegram.
    /// </summary>
    [Serializable]
    public abstract class TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramConfig"/> class.
        /// </summary>
        protected TelegramConfig()
        {
            this.Enabled = false;
            this.Answer = new Answer();
            this.TransfRef = string.Empty;
        }

        /// <summary>
        /// Gets or sets the XML attribute called from.
        /// </summary>
        [XmlAttribute(AttributeName = "TransfRef")]
        [DefaultValue("")]
        public string TransfRef { get; set; }

        /// <summary>
        /// Gets or sets the name of the telegram (which is also the name of the XML element).
        /// Free text (case insensitive)
        /// </summary>
        [XmlIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this telegram has to be parsed or ignored.
        /// Default value is false.
        /// </summary>
        [XmlAttribute(AttributeName = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// This element is XML-ignored because some telegrams need a different order of elements (or no usage at all).
        /// </summary>
        [XmlIgnore]
        public virtual GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// This element is XML-ignored because some telegrams need a different order of elements (or no answer at all).
        /// </summary>
        [XmlIgnore]
        public virtual Answer Answer { get; set; }

        /// <summary>
        /// Used for XML serialization.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Answer"/> should be serialized.
        /// </returns>
        public bool ShouldSerializeAnswer()
        {
            return this.Answer != null && this.Answer.Telegram != null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, UsedFor: {1}", this.Name, this.UsedFor);
        }
    }
}
