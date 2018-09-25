// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddMediaConfiguration.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AddMediaConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    /// <summary>
    /// Additional methods for the 'AddMediaConfiguration' migration.
    /// </summary>
    public partial class AddMediaConfiguration
    {
        /// <summary>
        /// Sets default value for the Version column on MediaConfigurations table.
        /// </summary>
        public void SetDefaultValueForVersions()
        {
            const string Sql = "ALTER TABLE dbo.MediaConfigurations ADD CONSTRAINT DF_MediaConfigurations_Version"
                               + " DEFAULT 1 FOR [Version];";
            this.Sql(Sql);
        }
    }
}