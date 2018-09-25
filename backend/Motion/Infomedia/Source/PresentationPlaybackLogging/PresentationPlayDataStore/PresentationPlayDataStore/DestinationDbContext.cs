// PresentationPlayLogging
// PresentationPlayLogging.DataStore
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationPlayLogging.DataStore
{
    using System.Data.Common;
    using System.Data.Entity;

    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.DataStore.Interfaces;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    using NLog;

    /// <summary>
    /// The destinations EF DB context
    /// </summary>
    public class DestinationDbContext : DbContext, IDbContext
    {
        /// <summary>The default connection string.</summary>
        public const string DefaultConnectionString = Constants.DefaultDestinationsConnectionString;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="DestinationDbContext"/> class.</summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public DestinationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            CommonConstruct(nameOrConnectionString);
        }

        public DestinationDbContext()
            : this(DefaultConnectionString)
        {
        }

        public DestinationDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            this.CommonConstruct(existingConnection.ConnectionString);
        }

        public string ConnectionString { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        /// <summary>Gets or sets the units.</summary>
        public DbSet<Unit> Units { get; set; }

        private void CommonConstruct(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new CreateDatabaseIfNotExists<DestinationDbContext>());
        }
    }
}