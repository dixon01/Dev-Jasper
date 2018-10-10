// PresentationPlayLogging
// PresentationLogging.UnitTest
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.PresentationLogging.UnitTest
{
    using System.Data.Entity.SqlServer;
    using System.Linq;
    using System.ServiceProcess;

    using Luminator.PresentationPlayLogging.DataStore;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PresentationPlayLoggingDatabaseUnitTest
    {
        const string DefaultConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Luminator.UnitTestPresentationPlayLogging;Integrated Security=true";

        const string DestinationsConnectionString = @"Data Source =.\SQLEXPRESS;Initial Catalog=Luminator.Destinations;Integrated Security = true";

        private static bool enableTest;

        // Must reference a type in EntityFramework.SqlServer.dll so that this dll will be
        // included in the output folder of referencing projects without requiring a direct 
        // dependency on Entity Framework. See http://stackoverflow.com/a/22315164/1141360.
        private static SqlProviderServices instance = SqlProviderServices.Instance;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var s = ServiceController.GetServices().FirstOrDefault(m => m.ServiceName == "MSSQL$SQLEXPRESS");
            enableTest = s != null && s.Status == ServiceControllerStatus.Running;
        }

#if __UseDestinations
        [TestMethod]
        public void CreateLocalDestinationsDatabase()
        {
            AssertIfDisabled();
       
            using (var context = new DestinationDbContext(DestinationsConnectionString))
            {
                context.Database.CreateIfNotExists();
            }

            Assert.IsTrue(new PresentationPlayLoggingDbContext(DefaultConnectionString).Database.Exists());
        }
#endif

        [TestMethod]
        public void CreateLocalUnitTestDatabase()
        {
            AssertIfDisabled();

            using (var context = new PresentationPlayLoggingDbContext(DefaultConnectionString))
            {
                context.Database.CreateIfNotExists();
            }

            Assert.IsTrue(new PresentationPlayLoggingDbContext(DefaultConnectionString).Database.Exists());
        }

        [TestMethod]
        public void CreatePresentationPlayLoggingDbContext()
        {
            AssertIfDisabled();
            using (var dbContext = new PresentationPlayLoggingDbContext(DefaultConnectionString))
            {
                Assert.IsFalse(string.IsNullOrEmpty(dbContext.ConnectionString));
            }
        }

        [TestMethod]
        [Ignore] // Comment out to execute
        public void CreateSqlGorbaDatabase()
        {
            // Used only to create sql table for testing on unique sql instance
            const string DefaultReleaseConnectionString =
                @"Data Source=SWDEVSQL\GORBA;Initial Catalog=Luminator.Reporting.PresentationPlayLogging;User Id=lumuser;Password=Lum2014;";

            AssertIfDisabled();
            using (var dbContext = new PresentationPlayLoggingDbContext(DefaultReleaseConnectionString))
            {
                Assert.IsFalse(string.IsNullOrEmpty(dbContext.ConnectionString));
                if (dbContext.Database.Exists() == false)
                {
                    dbContext.CreateDatabase(dbContext.ConnectionString);
                    Assert.IsTrue(dbContext.Database.Exists());
                }
                else
                {
                    Assert.Inconclusive("Database already exists");
                }
            }
        }

        [TestMethod]
        public void DropAndCreateDatbase()
        {
            AssertIfDisabled();
            const string DefaultConnectionString =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=UnitTest_DropAndCreateDatbase PresentationPlayLogging;Integrated Security=true;";

            using (var dbContext = new PresentationPlayLoggingDbContext(DefaultConnectionString))
            {
                if (dbContext.Database.Exists())
                {
                    dbContext.Database.Delete();
                }

                var dbExists = dbContext.Database.Exists();
                Assert.IsFalse(dbExists);
                dbContext.CreateDatabase(DefaultConnectionString);
                dbExists = dbContext.Database.Exists();
                Assert.IsTrue(dbExists);
            }

            using (var dbContext = new PresentationPlayLoggingDbContext(DefaultConnectionString))
            {
                dbContext.Database.Delete();
            }
        }

        static void AssertIfDisabled()
        {
            if (!enableTest)
            {
                Assert.Inconclusive("Database unit test skipped");
            }
        }
    }
}