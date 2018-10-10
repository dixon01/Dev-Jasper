// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Setup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Migrations
{
    /// <summary>
    /// Additional methods for the 'Setup' migration.
    /// </summary>
    public partial class Setup
    {
        /// <summary>
        /// Sets default value for the Version columns.
        /// </summary>
        private void SetDefaultValueForVersion()
        {
            this.Sql(
                "ALTER TABLE dbo.Authorizations ADD CONSTRAINT DF_Authorizations_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.UserRoles ADD CONSTRAINT DF_UserRoles_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Tenants ADD CONSTRAINT DF_Tenants_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Users ADD CONSTRAINT DF_Users_Version DEFAULT 1 FOR [Version];");
            const string AssociationTenantUserUserRolesSql = "ALTER TABLE dbo.AssociationTenantUserUserRoles ADD"
                                                             + " CONSTRAINT DF_AssociationTenantUserUserRoles_Version"
                                                             + " DEFAULT 1 FOR [Version];";
            this.Sql(
                AssociationTenantUserUserRolesSql);
            this.Sql("ALTER TABLE dbo.ProductTypes ADD CONSTRAINT DF_ProductTypes_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Units ADD CONSTRAINT DF_Units_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Resources ADD CONSTRAINT DF_Resources_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.UpdateGroups ADD CONSTRAINT DF_UpdateGroups_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.UpdateParts ADD CONSTRAINT DF_UpdateParts_Version DEFAULT 1 FOR [Version];");
            this.Sql(
                "ALTER TABLE dbo.UpdateCommands ADD CONSTRAINT DF_UpdateCommands_Version DEFAULT 1 FOR [Version];");
            this.Sql(
                "ALTER TABLE dbo.UpdateFeedbacks ADD CONSTRAINT DF_UpdateFeedbacks_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Documents ADD CONSTRAINT DF_Documents_Version DEFAULT 1 FOR [Version];");
            this.Sql(
                "ALTER TABLE dbo.DocumentVersions ADD CONSTRAINT DF_DocumentVersions_Version DEFAULT 1 FOR [Version];");
            this.Sql("ALTER TABLE dbo.Packages ADD CONSTRAINT DF_Packages_Version DEFAULT 1 FOR [Version];");
            this.Sql(
                "ALTER TABLE dbo.PackageVersions ADD CONSTRAINT DF_PackageVersions_Version DEFAULT 1 FOR [Version];");
            const string UnitConfigurationsSql = "ALTER TABLE dbo.UnitConfigurations ADD CONSTRAINT"
                                                 + " DF_UnitConfigurations_Version DEFAULT 1 FOR [Version];";
            this.Sql(UnitConfigurationsSql);
        }
    }
}