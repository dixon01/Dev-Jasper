// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InitialConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Init
{
    using Newtonsoft.Json;

    /// <summary>
    /// The initial config downloaded from the SG server under "/config".
    /// </summary>
    public class InitialConfig
    {
        /// <summary>
        /// Gets or sets the XML url where the "multi status" file can be downloaded.
        /// </summary>
        [JsonProperty("xml_url")]
        public string XmlUrl { get; set; }
    }
}
