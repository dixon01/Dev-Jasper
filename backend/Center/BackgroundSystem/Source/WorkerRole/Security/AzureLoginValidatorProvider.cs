// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLoginValidatorProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The factory to create a new <see cref="AzureLoginValidator" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Security
{
    using Gorba.Center.BackgroundSystem.Core.Security;

    using Microsoft.WindowsAzure;

    /// <summary>
    /// The factory to create a new <see cref="AzureLoginValidator"/>.
    /// </summary>
    public class AzureLoginValidatorProvider : LoginValidatorProvider
    {
        private volatile AzureLoginValidator validator;

        /// <summary>
        /// Provides a login validator.
        /// </summary>
        /// <returns>
        /// The <see cref="LoginValidatorBase"/>.
        /// </returns>
        public override LoginValidatorBase Provide()
        {
            if (this.validator != null)
            {
                return this.validator;
            }

            lock (this)
            {
                if (this.validator == null)
                {
                    var tenant = CloudConfigurationManager.GetSetting(PredefinedAzureItems.Settings.Tenant);
                    var clientId = CloudConfigurationManager.GetSetting(PredefinedAzureItems.Settings.ClientId);
                    var resourceUrl = CloudConfigurationManager.GetSetting(PredefinedAzureItems.Settings.ResourceUrl);
                    this.validator = new AzureLoginValidator(tenant, clientId, resourceUrl);
                }
            }

            return this.validator;
        }
    }
}
