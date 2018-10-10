// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiStatus.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.MultiStatus
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// A single multi-status object as received from ScreenGate.
    /// </summary>
    /// <remarks>
    /// All URLs found inside this objects are relative to the given <see cref="Base"/> URL.
    /// </remarks>
    public class MultiStatus
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss 'UTC'";

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStatus"/> class.
        /// </summary>
        public MultiStatus()
        {
            this.Responses = new List<MultiStatusResponse>();
        }

        /// <summary>
        /// Gets or sets the base URL which is to be used to resolve URLs in this object.
        /// </summary>
        [XmlAttribute("base", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Base { get; set; }

        /// <summary>
        /// Gets or sets the timestamp at which the data was updated.
        /// </summary>
        [XmlIgnore]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UpdatedAt"/> value serialized the way it is done by GateMedia.
        /// </summary>
        [XmlAttribute("updated_at", Namespace = "http://www.gatemedia.ch")]
        public string UpdatedAtString
        {
            get
            {
                return this.UpdatedAt.ToUniversalTime().ToString(DateTimeFormat);
            }

            set
            {
                var dateTime = DateTime.ParseExact(
                    value,
                    DateTimeFormat,
                    CultureInfo.InvariantCulture);
                this.UpdatedAt = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Gets or sets the list of responses.
        /// </summary>
        [XmlElement("response", typeof(MultiStatusResponse))]
        public List<MultiStatusResponse> Responses { get; set; }

        /// <summary>
        /// Gets a mapping of all responses.
        /// The key is the full <see cref="Uri"/> of the response, the value its ETag.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary{TKey,TValue}"/>.
        /// </returns>
        public IDictionary<Uri, string> GetResponses()
        {
            return
                this.Responses.Where(r => r.PropStat != null && r.PropStat.Prop != null)
                    .ToDictionary(r => new Uri(this.Base + r.HypertextReference), r => r.PropStat.Prop.GetETag);
        }
    }
}