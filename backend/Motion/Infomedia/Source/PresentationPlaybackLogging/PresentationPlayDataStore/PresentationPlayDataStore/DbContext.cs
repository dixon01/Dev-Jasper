// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.DataStore
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.SqlServer;
    using System.IO;

    using Luminator.PresentationPlayLogging.Core;
    using Luminator.PresentationPlayLogging.DataStore.Interfaces;
    using Luminator.PresentationPlayLogging.DataStore.Models;

    using NLog;

    /// <summary>The presentation play logging db context.</summary>
    public class PresentationPlayLoggingDbContext : DbContext, IDbContext
    {

        // Must reference a type in EntityFramework.SqlServer.dll so that this dll will be
        // included in the output folder of referencing projects without requiring a direct 
        // dependency on Entity Framework. See http://stackoverflow.com/a/22315164/1141360.
        private static SqlProviderServices instance = SqlProviderServices.Instance;

        /// <summary>The default connection string.</summary>
        public const string DefaultConnectionString = Constants.DefaultConnectionString;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLoggingDbContext" /> class.</summary>
        /// <param name="nameOrConnectionString">The name or connection string.</param>
        public PresentationPlayLoggingDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.CommonConstruct(nameOrConnectionString);
        }

        /// <summary>Initializes a new instance of the <see cref="PresentationPlayLoggingDbContext" /> class.</summary>
        protected PresentationPlayLoggingDbContext()
            : this(DefaultConnectionString)
        {
        }

        public PresentationPlayLoggingDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            this.CommonConstruct(existingConnection.ConnectionString);
        }

        /// <summary>Gets the Sql connection string.</summary>
        public string ConnectionString { get; private set; }

        /// <summary>Gets or sets the presentation log items.</summary>
        public DbSet<PresentationPlayLoggingItem> PresentationLogItems { get; set; }

        /// <summary>
        ///     Create the database if missing
        /// </summary>
        public void CreateDatabase(string connectionString)
        {
            if (!Database.Exists(connectionString))
            {
                Logger.Info("Creating Database for Presentation Logging");
                this.Database.CreateIfNotExists();
                if (!Database.Exists(connectionString))
                {
                    throw new FileNotFoundException("PresentationLoggingDbContext Database not found!");
                }

                Logger.Info("Database {0} Created Successfully", this.Database.Connection.DataSource);
            }
            else
            {
                Logger.Info("Database {0} already exists", this.Database.Connection.DataSource);
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private void CommonConstruct(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.Configuration.LazyLoadingEnabled = false;
            
            Database.SetInitializer(new PresentationPlayLoggingCreateDatabaseIfNotExists());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<PresentationPlayLoggingDbContext>());
        }
    }
}