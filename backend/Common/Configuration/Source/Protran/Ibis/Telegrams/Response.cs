// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about an answer's response.
    /// </summary>
    [Serializable]
    public class Response
    {
        private int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
            this.Value = 0;
            this.Status = Status.NoData;
        }

        /// <summary>
        /// Gets or sets the response's value.
        /// </summary>
        [XmlText]
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value < 0 || value > 9)
                {
                    throw new ArgumentOutOfRangeException("value", "Response value can only be 0...9");
                }

                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML attribute called from.
        /// </summary>
        [XmlAttribute("Status")]
        public Status Status { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Status, this.Value);
        }
    }
}
