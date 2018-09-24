// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestHubClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITestHubClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Hubs
{
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Models;

    /// <summary>
    /// The TestHubClient interface.
    /// </summary>
    public interface ITestHubClient
    {
        /// <summary>
        /// Updates the result on clients.
        /// </summary>
        /// <param name="testResult">
        /// The test result.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task UpdateResultAsync(TestResult testResult);
    }
}