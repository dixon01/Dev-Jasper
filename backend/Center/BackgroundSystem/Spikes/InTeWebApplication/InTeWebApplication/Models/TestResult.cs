// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestResult.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Models
{
    using System;

    /// <summary>
    /// The test result.
    /// </summary>
    public class TestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="runId">
        /// The run id.
        /// </param>
        public TestResult(string name, Guid runId)
            : this()
        {
            this.Name = name;
            this.RunId = runId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        public TestResult()
        {
            this.ResultType = TestResultType.Inconclusive;
            this.ElapsedTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets the run id.
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the result type.
        /// </summary>
        public TestResultType ResultType { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }
    }
}