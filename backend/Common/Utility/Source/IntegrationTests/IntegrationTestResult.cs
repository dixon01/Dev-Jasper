// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestResult.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegrationTestReview type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.IntegrationTests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the review information for an integration test.
    /// </summary>
    public class IntegrationTestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestResult"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public IntegrationTestResult(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestResult"/> class.
        /// </summary>
        public IntegrationTestResult()
        {
            this.Failures = new List<Failure>();
        }

        /// <summary>
        /// Gets or sets the name of the test.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the test started.
        /// </summary>
        /// <value>
        /// The date and time when the test started.
        /// </value>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the test ended.
        /// </summary>
        /// <value>
        /// The date and time when the test ended.
        /// </value>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the test failed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the test failed; otherwise, <c>false</c>.
        /// </value>
        public bool Failed { get; set; }

        /// <summary>
        /// Gets or sets the list of failures.
        /// </summary>
        public ICollection<Failure> Failures { get; set; }
    }
}
