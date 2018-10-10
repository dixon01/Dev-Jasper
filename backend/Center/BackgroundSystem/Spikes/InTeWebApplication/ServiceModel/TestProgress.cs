// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestProgress.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestProgress type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.ServiceModel
{
    using System;

    /// <summary>
    /// The test progress.
    /// </summary>
    public class TestProgress
    {
        /// <summary>
        /// Gets or sets the run id.
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the test is completed.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}