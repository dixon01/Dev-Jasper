// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogsFeedbackHandlerProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The azure logs feedback handler provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Update
{
    using Gorba.Center.BackgroundSystem.Core.Update;
    using Gorba.Center.BackgroundSystem.Core.Update.Azure;

    /// <summary>
    /// The azure logs feedback handler provider.
    /// </summary>
    public class AzureLogsFeedbackHandlerProvider : LogsFeedbackHandlerProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="AzureLogsFeedbackHandler"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="LogsFeedbackHandlerBase"/>.
        /// </returns>
        public override LogsFeedbackHandlerBase Create()
        {
            return new AzureLogsFeedbackHandler();
        }
    }
}
