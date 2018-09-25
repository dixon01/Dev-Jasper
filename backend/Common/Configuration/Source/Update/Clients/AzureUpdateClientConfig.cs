// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureUpdateClientConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureUpdateClientConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Clients
{
    using System;

    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// The Azure update client config.
    /// </summary>
    [Serializable]
    [Implementation("Gorba.Common.Update.AzureClient.AzureUpdateClient, Gorba.Common.Update.AzureClient")]
    public class AzureUpdateClientConfig : UpdateClientConfigBase
    {
        /// <summary>
        /// Gets or sets the entire URL of the repository.xml file (HTTP or HTTPS).
        /// </summary>
        public string RepositoryUrl { get; set; }
    }
}
