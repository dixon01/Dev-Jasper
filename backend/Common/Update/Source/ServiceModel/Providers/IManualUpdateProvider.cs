// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManualUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManualUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Providers
{
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Interface for all update providers that allow for manual feedback checks.
    /// </summary>
    public interface IManualUpdateProvider : IUpdateProvider
    {
        /// <summary>
        /// Forces this update provider to immediately check if there is feedback available and
        /// download it if so.
        /// This method won't do anything if <see cref="IUpdateSink.IsAvailable"/> returns false.
        /// </summary>
        /// <param name="progress">
        /// The progress monitor.
        /// </param>
        /// <exception cref="UpdateException">
        /// if there was an error while getting the feedback.
        /// </exception>
        void CheckForFeedback(IProgressMonitor progress);
    }
}