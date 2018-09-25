// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureConfigurationException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Exceptions
{
    using System;

    /// <summary>
    /// Defines an exception thrown when there is an error in the Azure configuration.
    /// </summary>
    public class AzureConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureConfigurationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public AzureConfigurationException(string message)
        : base(message)
        {
        }
    }
}