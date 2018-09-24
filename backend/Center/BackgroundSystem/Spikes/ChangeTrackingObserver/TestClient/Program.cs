// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ChangeTrackingObserver.TestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking.Configurations;
    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;

    using NLog;

    using ChangeTrackingSetup = Gorba.Center.Common.Client.ChangeTracking.Membership.ChangeTrackingSetup;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private const string HashedPassword = "19812e3fbf242b2b166cc2f285f43871";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Type enter to start the Observer");
            Console.ReadLine();

            ServiceBusNotificationManagerUtility.ConfigureServiceBusNotificationManager();

            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            ChannelScopeFactoryUtility<ITenantDataService>.ConfigureAsDataService(configuration.DataServices, "Tenant");
            ChannelScopeFactoryUtility<IUserDataService>.ConfigureAsDataService(configuration.DataServices, "User");
            ChannelScopeFactoryUtility<IUpdateGroupDataService>.ConfigureAsDataService(
                configuration.DataServices,
                "UpdateGroup");
            ChannelScopeFactoryUtility<IDocumentDataService>.ConfigureAsDataService(
                configuration.DataServices,
                "Document");
            ChannelScopeFactoryUtility<IUnitConfigurationDataService>.ConfigureAsDataService(
                configuration.DataServices,
                "UnitConfiguration");
            ChannelScopeFactoryUtility<IProductTypeDataService>.ConfigureAsDataService(
                configuration.DataServices,
                "ProductType");
            var delay = Task.Delay(TimeSpan.FromSeconds(10));
            var userCredentials = new UserCredentials("admin", HashedPassword);

            var tasks = CreateChangeTrackingManagers(configuration, userCredentials);
            var managers = Task.WhenAll(tasks);
            Task.WhenAny(managers, delay);

            var tenantChangeTrackingManager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
            ((TenantChangeTrackingManager)tenantChangeTrackingManager).OperationCompleted +=
                (sender, eventArgs) =>
                {
                    Logger.Trace("-------------------");
                    Logger.Debug("Operation completed for tenant. Succeeded: {0}", eventArgs.Succeeded);
                    Logger.Trace("-------------------");
                };
            var query = tenantChangeTrackingManager.QueryAsync().Result;

            var unitConfigurationChangeTrackingManager =
                DependencyResolver.Current.Get<IUnitConfigurationChangeTrackingManager>();
            ((UnitConfigurationChangeTrackingManager)unitConfigurationChangeTrackingManager).OperationCompleted +=
                OnOperationCompleted;
            var unitConfiguration = unitConfigurationChangeTrackingManager.QueryAsync().Result.First();
            unitConfiguration.Document.LoadReferencePropertiesAsync().Wait();
            var tenant = unitConfiguration.Document.Tenant;
            var counter = 0;
            tenant.PropertyChanged += (sender, eventArgs) =>
                {
                    Logger.Trace("-------------------");
                    Logger.Debug("Property '{0}' changed for tenant ({1})", eventArgs.PropertyName, ++counter);
                    Logger.Debug("Tenant: {0}, named '{1}'", tenant.Id, tenant.Name);
                    Logger.Trace("-------------------");
                };
            Console.WriteLine("Tenant for document of configuration: '{0}'", tenant.Name);

            foreach (var tenantReadableModel in query)
            {
                var areSame = tenant.Id != tenantReadableModel.Id
                        || object.ReferenceEquals(tenant, tenantReadableModel);
                WriteTenantVerification(areSame, 1);
                tenantReadableModel.LoadNavigationPropertiesAsync().Wait();
                foreach (var userReadableModel in tenantReadableModel.Users)
                {
                    Logger.Info(
                        "Found user '{0}' for tenant '{1}'",
                        userReadableModel.Username,
                        userReadableModel.OwnerTenant.Name);
                    areSame = tenant.Id != userReadableModel.OwnerTenant.Id
                        || object.ReferenceEquals(tenant, userReadableModel.OwnerTenant);
                    WriteTenantVerification(areSame, 2);
                    areSame = tenantReadableModel.Id != userReadableModel.OwnerTenant.Id
                        || object.ReferenceEquals(tenantReadableModel, userReadableModel.OwnerTenant);
                    WriteTenantVerification(areSame, 3);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Enter to exit");
            Console.ReadLine();
        }

        private static void TestAssociations()
        {
            var c = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            var userCredentials = new UserCredentials("admin", HashedPassword);

            ChannelScopeFactoryUtility<IUserDataService>.ConfigureAsDataService(
                c.DataServices,
                "User");
            User user;
            using (var scope = ChannelScopeFactory<IUserDataService>.Current.Create(userCredentials))
            {
                user = scope.Channel.QueryAsync().Result.Last();
            }

            ChannelScopeFactoryUtility<IAssociationTenantUserUserRoleDataService>.ConfigureAsDataService(
                c.DataServices,
                "AssociationTenantUserUserRole");
            using (
                var scope =
                    ChannelScopeFactory<IAssociationTenantUserUserRoleDataService>.Current.Create(userCredentials))
            {
                var filter =
                    AssociationTenantUserUserRoleFilter.Create().IncludeTenant().IncludeUserRole().WithUser(user);
                var values = scope.Channel.QueryAsync(filter).Result.ToList();
                foreach (var associationTenantUserUserRole in values)
                {
                    Logger.Info("Returned association {0}", associationTenantUserUserRole.Id);
                }
            }
        }

        private static void OnOperationCompleted(
            object sender,
            UnitConfigurationChangeTrackingManager.OperationCompletedEventArgs operationCompletedEventArgs)
        {
            Logger.Trace("-------------------");
            Logger.Debug(
                "Operation completed for unit configuration. Succeeded: {0}",
                operationCompletedEventArgs.Succeeded);
            Logger.Trace("-------------------");
        }

        private static void WriteTenantVerification(bool areSame, int index)
        {
            if (areSame)
            {
                Logger.Debug("Instance reference check ok ({0})", index);
                return;
            }

            Logger.Warn("Instance reference check failed ({0})!", index);
        }

        private static IEnumerable<Task> CreateChangeTrackingManagers(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            yield return CreateTenantChangeTrackingManager(configuration, userCredentials);
            yield return CreateUserChangeTrackingManager(configuration, userCredentials);
            yield return CreateUnitGroupChangeTrackingManager(configuration, userCredentials);
            yield return CreateDocumentChangeTrackingManager(configuration, userCredentials);
            yield return CreateProductTypeChangeTrackingManager(configuration, userCredentials);
            yield return CreateUnitConfigurationChangeTrackingManager(configuration, userCredentials);
        }

        private static Task CreateTenantChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var tenantChangeTrackingManager =
                (TenantChangeTrackingManager)
                ChangeTrackingSetup.CreateTenantChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register((ITenantChangeTrackingManager)tenantChangeTrackingManager);
            return tenantChangeTrackingManager.RunAsync();
        }

        private static Task CreateUserChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var userChangeTrackingManager =
                (UserChangeTrackingManager)
                ChangeTrackingSetup.CreateUserChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register((IUserChangeTrackingManager)userChangeTrackingManager);
            var usersTask = userChangeTrackingManager.RunAsync();
            return usersTask;
        }

        private static Task CreateUnitGroupChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var updateGroupChangeTrackingManager =
                (UpdateGroupChangeTrackingManager)
                Gorba.Center.Common.Client.ChangeTracking.Update.ChangeTrackingSetup
                    .CreateUpdateGroupChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register((IUpdateGroupChangeTrackingManager)updateGroupChangeTrackingManager);
            return updateGroupChangeTrackingManager.RunAsync();
        }

        private static Task CreateDocumentChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var documentChangeTrackingManager =
                (DocumentChangeTrackingManager)
                Gorba.Center.Common.Client.ChangeTracking.Documents.ChangeTrackingSetup
                    .CreateDocumentChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register((IDocumentChangeTrackingManager)documentChangeTrackingManager);
            return documentChangeTrackingManager.RunAsync();
        }

        private static Task CreateProductTypeChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var productTypeChangeTrackingManager =
                (ProductTypeChangeTrackingManager)
                Gorba.Center.Common.Client.ChangeTracking.Units.ChangeTrackingSetup
                    .CreateProductTypeChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register((IProductTypeChangeTrackingManager)productTypeChangeTrackingManager);
            return productTypeChangeTrackingManager.RunAsync();
        }

        private static Task CreateUnitConfigurationChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var unitConfigurationChangeTrackingManager =
                (UnitConfigurationChangeTrackingManager)
                Gorba.Center.Common.Client.ChangeTracking.Configurations.ChangeTrackingSetup
                    .CreateUnitConfigurationChangeTrackingManager(configuration, userCredentials);
            DependencyResolver.Current.Register(
                (IUnitConfigurationChangeTrackingManager)unitConfigurationChangeTrackingManager);
            return unitConfigurationChangeTrackingManager.RunAsync();
        }
    }
}
