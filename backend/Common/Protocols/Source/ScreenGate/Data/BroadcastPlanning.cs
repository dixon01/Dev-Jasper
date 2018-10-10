// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BroadcastPlanning.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BroadcastPlanning type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.ScreenGate.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The broadcast planning for a <see cref="PlaylistElement"/>.
    /// Every property in this class is a list of tuples: start time, end time.
    /// The arrays stored in the list therefore always have two elements.
    /// </summary>
    public class BroadcastPlanning
    {
        /// <summary>
        /// Gets or sets the schedule for Monday.
        /// </summary>
        [JsonProperty("monday")]
        public List<string[]> Monday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Tuesday.
        /// </summary>
        [JsonProperty("tuesday")]
        public List<string[]> Tuesday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Wednesday.
        /// </summary>
        [JsonProperty("wednesday")]
        public List<string[]> Wednesday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Thursday.
        /// </summary>
        [JsonProperty("thursday")]
        public List<string[]> Thursday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Friday.
        /// </summary>
        [JsonProperty("friday")]
        public List<string[]> Friday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Saturday.
        /// </summary>
        [JsonProperty("saturday")]
        public List<string[]> Saturday { get; set; }

        /// <summary>
        /// Gets or sets the schedule for Sunday.
        /// </summary>
        [JsonProperty("sunday")]
        public List<string[]> Sunday { get; set; }
    }
}