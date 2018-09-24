// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ChangeTrackingObserver.TestEditorClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.Client.ChangeTracking.Configurations;
    using Gorba.Center.Common.Client.ChangeTracking.Documents;
    using Gorba.Center.Common.Client.ChangeTracking.Membership;
    using Gorba.Center.Common.Client.ChangeTracking.Units;
    using Gorba.Center.Common.Client.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceBus;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Center.Common.ServiceModel.Units;

    using NLog;

    using ChangeTrackingSetup = Gorba.Center.Common.Client.ChangeTracking.Membership.ChangeTrackingSetup;
    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        private const string HashedPassword = "19812e3fbf242b2b166cc2f285f43871";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly AutoResetEvent OperationCompleted = new AutoResetEvent(false);

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal static void Main(string[] args)
        {
            Console.WriteLine("Type enter to start the Editor");
            Console.ReadLine();

            ServiceBusNotificationManagerUtility.ConfigureServiceBusNotificationManager();

            var configuration = BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            ChannelScopeFactoryUtility<IUnitDataService>.ConfigureAsDataService(configuration.DataServices, "Unit");
            ChannelScopeFactoryUtility<ITenantDataService>.ConfigureAsDataService(configuration.DataServices, "Tenant");
            var delay = Task.Delay(TimeSpan.FromSeconds(10));
            var userCredentials = new UserCredentials("admin", HashedPassword);

            var tasks = CreateChangeTrackingManagers(configuration, userCredentials);
            var managers = Task.WhenAll(tasks);
            Task.WhenAny(managers, delay);

            var tenantChangeTrackingManager =
                (TenantChangeTrackingManager)DependencyResolver.Current.Get<ITenantChangeTrackingManager>();

            tenantChangeTrackingManager.OperationCompleted += (sender, eventArgs) => OperationCompleted.Set();
            var tenants = tenantChangeTrackingManager.QueryAsync().Result;

            var firstTenant = tenants.First();
            Logger.Info("First tenant: '{0}' ({1})", firstTenant.Name, firstTenant.Id);
            firstTenant.PropertyChanged += FirstTenantOnPropertyChanged;
            for (var i = 0; i < 2; i++)
            {
                EditRun(firstTenant);
                OperationCompleted.WaitOne();
            }

            firstTenant.LoadNavigationPropertiesAsync().Wait();

            Logger.Info("Users count: {0}", firstTenant.Users.Count);
            firstTenant.Users.CollectionChanged += (sender, eventArgs) =>
                {
                    switch (eventArgs.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            Logger.Info("User added. New count: {0}", firstTenant.Users.Count);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            Logger.Info("User deleted. New count: {0}", firstTenant.Users.Count);
                        break;
                        case NotifyCollectionChangedAction.Replace:
                        case NotifyCollectionChangedAction.Move:
                        case NotifyCollectionChangedAction.Reset:
                            Logger.Warn("Unexpected action '{0}'", eventArgs.Action);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };
            var userChangeTrackingManager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
            var user = userChangeTrackingManager.Create();
            user.OwnerTenant = firstTenant;
            user.Username = "user " + DateTime.Now;
            user.Commit();

            var filter = UserFilter.Create().WithUsername("admin", StringComparison.Different);
            var query = userChangeTrackingManager.QueryAsync(filter).Result.ToList();
            Logger.Info("Found {0} user(s)", query.Count);
            var firstUser = query.First();
            firstUser.OwnerTenant.LoadNavigationPropertiesAsync().Wait();
            Logger.Info(
                "First user belongs to tenant '{0}' ({1}) with {2} user(s)",
                firstUser.OwnerTenant.Name,
                firstUser.OwnerTenant.Id,
                firstUser.OwnerTenant.Users.Count);
            Logger.Info("Deleting user '{0}' ({1})", firstUser.Username, firstUser.Id);
            userChangeTrackingManager.DeleteAsync(firstUser).Wait();
            Logger.Info("User deleted");

            TestUnits();

            Console.WriteLine();
            Console.WriteLine("Enter to exit");
            Console.ReadLine();
        }

        private static void TestUnits()
        {
            var unitChangeTrackingManager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
            var units = unitChangeTrackingManager.QueryAsync().Result.ToList();
            Logger.Info("Found {0} unit(s)", units.Count);
            var unit = units.First();
            var unitWritableModel = unit.ToChangeTrackingModel();
            unitWritableModel.Name = "Changed unit name " + DateTime.Now;
            Logger.Info("Changing unit");
            unitChangeTrackingManager.CommitAndVerifyAsync(unitWritableModel).Wait();

            unit = units.Last();
            unitWritableModel = unit.ToChangeTrackingModel();
            unitWritableModel.Name = "Changed unit name " + DateTime.Now;
            unitWritableModel.UserDefinedProperties["test"] = "new value " + DateTime.Now;
            Logger.Info("Changing unit");
            unitChangeTrackingManager.CommitAndVerifyAsync(unitWritableModel).Wait();

            unitWritableModel = unitChangeTrackingManager.Create();
            unitWritableModel.Name = "New unit " + DateTime.Now;
            unitWritableModel.Tenant = unit.Tenant;
            unitWritableModel.ProductType = unit.ProductType;
            unitWritableModel.UserDefinedProperties["test"] = "new value on new unit " + DateTime.Now;
            unitChangeTrackingManager.CommitAndVerifyAsync(unitWritableModel).Wait();

            var newUnitFromService = new Unit
                                         {
                                             Name = "New unit from service " + DateTime.Now,
                                             ProductType = new ProductType { Id = unit.ProductType.Id },
                                             Tenant = new Tenant { Id = unit.Tenant.Id }
                                         };
            newUnitFromService.UserDefinedProperties["test"] = "new unit from service " + DateTime.Now;
            using (
                var channelScope =
                    ChannelScopeFactory<IUnitDataService>.Current.Create(new UserCredentials("admin", HashedPassword)))
            {
                channelScope.Channel.AddAsync(newUnitFromService).Wait();
            }

            unit = units.Skip(1).First();
            using (
                var channelScope =
                    ChannelScopeFactory<IUnitDataService>.Current.Create(new UserCredentials("admin", HashedPassword)))
            {
                var unitToUpdate = channelScope.Channel.GetAsync(unit.Id).Result;
                unitToUpdate.UserDefinedProperties["test"] = "value updated from service " + DateTime.Now;
                channelScope.Channel.UpdateAsync(unitToUpdate).Wait();
            }

            Logger.Info("Unit changed");
        }

        private static void FirstTenantOnPropertyChanged(
            object sender,
            PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var tenant = (TenantReadableModel)sender;
            Logger.Debug("-------------------");
            Logger.Debug("Property '{0}' changed for tenant", propertyChangedEventArgs.PropertyName);
            Logger.Debug("Tenant: {0}, named '{1}'", tenant.Id, tenant.Name);
            Logger.Debug("-------------------");
        }

        private static void EditRun(TenantReadableModel tenant)
        {
            EditName(tenant);
        }

        private static void EditName(TenantReadableModel tenant)
        {
            var writableTenant = tenant.ToChangeTrackingModel();
            Logger.Info(
                "Editing tenant {0} named '{1}' with current version {2}",
                tenant.Id,
                tenant.Name,
                tenant.Version.Value);
            writableTenant.Name = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            writableTenant.Commit();
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
            yield return CreateUnitChangeTrackingManager(configuration, userCredentials);
        }

        private static Task CreateTenantChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var tenantChangeTrackingManager =
                (TenantChangeTrackingManager)
                ChangeTrackingSetup.CreateTenantChangeTrackingManager(configuration, userCredentials);
            return tenantChangeTrackingManager.RunAsync();
        }

        private static Task CreateUserChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var userChangeTrackingManager =
                (UserChangeTrackingManager)
                ChangeTrackingSetup.CreateUserChangeTrackingManager(configuration, userCredentials);
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
            return unitConfigurationChangeTrackingManager.RunAsync();
        }

        private static Task CreateUnitChangeTrackingManager(
            BackgroundSystemConfiguration configuration,
            UserCredentials userCredentials)
        {
            var unitChangeTrackingManager =
                (UnitChangeTrackingManager)
                Gorba.Center.Common.Client.ChangeTracking.Units.ChangeTrackingSetup
                    .CreateUnitChangeTrackingManager(configuration, userCredentials);
            return unitChangeTrackingManager.RunAsync();
        }
    }
}
