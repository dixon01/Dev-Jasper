// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDefinedProperties.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Additional methods for the 'UserDefinedProperties' migration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    /// <summary>
    /// Additional methods for the 'UserDefinedProperties' migration.
    /// </summary>
    public partial class UserDefinedProperties
    {
        /// <summary>
        /// Sets default value for the Version column on UserDefinedProperties table.
        /// </summary>
        public void SetDefaultValueForVersions()
        {
            const string Sql = "ALTER TABLE dbo.UserDefinedProperties ADD CONSTRAINT DF_UserDefinedProperties_Version"
                               + " DEFAULT 1 FOR [Version];";
            this.Sql(Sql);
        }
    }
}