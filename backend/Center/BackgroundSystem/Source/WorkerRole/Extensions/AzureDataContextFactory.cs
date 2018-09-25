// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureDataContextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureDataContextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Extensions
{
    using System.Data.Entity.Infrastructure;

    using Gorba.Center.BackgroundSystem.Data;

    using Microsoft.WindowsAzure;

    /// <summary>
    /// Factory to create the data context using information from the <see cref="CloudConfigurationManager"/>.
    /// </summary>
    public class AzureDataContextFactory : DataContextFactory
    {
        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <returns>A new context.</returns>
        public override CenterDataContext Create()
        {
            return
                new CenterDataContext(
                    CloudConfigurationManager.GetSetting(Common.Azure.PredefinedAzureItems.Settings.CenterDataContext));
        }

        /// <summary>
        /// Gets the database connection info.
        /// </summary>
        /// <returns>
        /// The <see cref="System.Data.Entity.Infrastructure.DbConnectionInfo"/>.
        /// </returns>
        public override DbConnectionInfo GetDbConnectionInfo()
        {
            return new DbConnectionInfo(
                CloudConfigurationManager.GetSetting(Common.Azure.PredefinedAzureItems.Settings.CenterDataContext),
                "System.Data.SqlClient");
        }
    }
}