namespace Gorba.Center.BackgroundSystem.Data
{
    using System.Data.Entity;

    using Gorba.Center.BackgroundSystem.Data.Model.AccessControl;
    using Gorba.Center.BackgroundSystem.Data.Model.Configurations;
    using Gorba.Center.BackgroundSystem.Data.Model.Documents;
    using Gorba.Center.BackgroundSystem.Data.Model.Log;
    using Gorba.Center.BackgroundSystem.Data.Model.Membership;
    using Gorba.Center.BackgroundSystem.Data.Model.Meta;
    using Gorba.Center.BackgroundSystem.Data.Model.Resources;
    using Gorba.Center.BackgroundSystem.Data.Model.Software;
    using Gorba.Center.BackgroundSystem.Data.Model.Units;
    using Gorba.Center.BackgroundSystem.Data.Model.Update;

    public partial class CenterDataContext : DbContext
    {
        public IDbSet<Authorization> Authorizations { get; set; }

        public IDbSet<UserRole> UserRoles { get; set; }

        public IDbSet<UnitConfiguration> UnitConfigurations { get; set; }

        public IDbSet<MediaConfiguration> MediaConfigurations { get; set; }

        public IDbSet<Document> Documents { get; set; }

        public IDbSet<DocumentVersion> DocumentVersions { get; set; }

        public IDbSet<LogEntry> LogEntries { get; set; }

        public IDbSet<Tenant> Tenants { get; set; }

        public IDbSet<User> Users { get; set; }

        public IDbSet<AssociationTenantUserUserRole> AssociationTenantUserUserRoles { get; set; }

        public IDbSet<UserDefinedProperty> UserDefinedProperties { get; set; }

        public IDbSet<SystemConfig> SystemConfigs { get; set; }

        public IDbSet<Resource> Resources { get; set; }

        public IDbSet<ContentResource> ContentResources { get; set; }

        public IDbSet<Package> Packages { get; set; }

        public IDbSet<PackageVersion> PackageVersions { get; set; }

        public IDbSet<ProductType> ProductTypes { get; set; }

        public IDbSet<Unit> Units { get; set; }

        public IDbSet<UpdateGroup> UpdateGroups { get; set; }

        public IDbSet<UpdatePart> UpdateParts { get; set; }

        public IDbSet<UpdateCommand> UpdateCommands { get; set; }

        public IDbSet<UpdateFeedback> UpdateFeedbacks { get; set; }

        private void DisableReferenceCascadeDelete(DbModelBuilder modelBuilder)
        {
            // Authorization.UserRole
            modelBuilder.Entity<Authorization>()
                .HasRequired(e => e.UserRole)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UnitConfiguration.Document
            modelBuilder.Entity<UnitConfiguration>()
                .HasRequired(e => e.Document)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UnitConfiguration.ProductType
            modelBuilder.Entity<UnitConfiguration>()
                .HasRequired(e => e.ProductType)
                .WithMany()
                .WillCascadeOnDelete(false);

            // MediaConfiguration.Document
            modelBuilder.Entity<MediaConfiguration>()
                .HasRequired(e => e.Document)
                .WithMany()
                .WillCascadeOnDelete(false);

            // Document.Tenant
            modelBuilder.Entity<Document>()
                .HasRequired(e => e.Tenant)
                .WithMany()
                .WillCascadeOnDelete(false);

            // DocumentVersion.Document
            modelBuilder.Entity<DocumentVersion>()
                .HasRequired(e => e.Document)
                .WithMany()
                .WillCascadeOnDelete(false);

            // User.OwnerTenant
            modelBuilder.Entity<User>()
                .HasRequired(e => e.OwnerTenant)
                .WithMany()
                .WillCascadeOnDelete(false);

            // AssociationTenantUserUserRole.User
            modelBuilder.Entity<AssociationTenantUserUserRole>()
                .HasRequired(e => e.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            // AssociationTenantUserUserRole.UserRole
            modelBuilder.Entity<AssociationTenantUserUserRole>()
                .HasRequired(e => e.UserRole)
                .WithMany()
                .WillCascadeOnDelete(false);

            // PackageVersion.Package
            modelBuilder.Entity<PackageVersion>()
                .HasRequired(e => e.Package)
                .WithMany()
                .WillCascadeOnDelete(false);

            // Unit.Tenant
            modelBuilder.Entity<Unit>()
                .HasRequired(e => e.Tenant)
                .WithMany()
                .WillCascadeOnDelete(false);

            // Unit.ProductType
            modelBuilder.Entity<Unit>()
                .HasRequired(e => e.ProductType)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UpdateGroup.Tenant
            modelBuilder.Entity<UpdateGroup>()
                .HasRequired(e => e.Tenant)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UpdatePart.UpdateGroup
            modelBuilder.Entity<UpdatePart>()
                .HasRequired(e => e.UpdateGroup)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UpdateCommand.Unit
            modelBuilder.Entity<UpdateCommand>()
                .HasRequired(e => e.Unit)
                .WithMany()
                .WillCascadeOnDelete(false);

            // UpdateFeedback.UpdateCommand
            modelBuilder.Entity<UpdateFeedback>()
                .HasRequired(e => e.UpdateCommand)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}