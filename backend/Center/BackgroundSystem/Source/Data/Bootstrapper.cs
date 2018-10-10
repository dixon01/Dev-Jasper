// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Bootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data
{
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;

    using Gorba.Center.BackgroundSystem.Data.Migrations;

    using NLog;

    /// <summary>
    /// Defines the bootstrapper to initialize the data environment.
    /// </summary>
    public class Bootstrapper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the data environment for an application.
        /// </summary>
        public void Bootstrap()
        {
            Logger.Debug("Bootstrapping the database");
            Database.SetInitializer(new NullDatabaseInitializer<CenterDataContext>());
            var targetDatabase = DataContextFactory.Current.GetDbConnectionInfo();
            DbMigrationsConfiguration configuration = new Configuration { TargetDatabase = targetDatabase };
            var migrator = new DbMigrator(configuration);
            try
            {
                migrator.Update();
            }
            catch (AutomaticMigrationsDisabledException exception)
            {
                throw new DataException("Exception while verifying the database (maybe it was not updated)", exception);
            }

            Logger.Debug("Migrations applied");
        }
    }
}