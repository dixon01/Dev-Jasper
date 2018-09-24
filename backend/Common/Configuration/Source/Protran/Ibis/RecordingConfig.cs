// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;

    /// <summary>
    /// Container of all the settings required
    /// to store a session with a specific IBIS master.
    /// </summary>
    [Serializable]
    public class RecordingConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingConfig"/> class.
        /// </summary>
        public RecordingConfig()
        {
            this.FileAbsPath = "./ibis.log";
            this.Active = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// this registration is enabled or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the format in which to record telegrams.
        /// </summary>
        public RecordingFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the absolute path of the
        /// file that will contain the whole registration.
        /// </summary>
        public string FileAbsPath { get; set; }
    }
}
