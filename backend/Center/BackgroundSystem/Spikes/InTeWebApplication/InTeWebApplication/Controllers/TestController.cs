// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Controllers
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication.Models;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Common.ServiceModel.Filters.AccessControl;
    using Gorba.Center.Common.ServiceModel.Filters.Membership;
    using Gorba.Center.Common.ServiceModel.Filters.Units;
    using Gorba.Center.Common.ServiceModel.Membership;
    using Gorba.Center.Common.ServiceModel.Security;

    using DependencyResolver = Gorba.Center.Common.ServiceModel.DependencyResolver;
    using StringComparison = Gorba.Center.Common.ServiceModel.Filters.StringComparison;

    /// <summary>
    /// The test controller.
    /// </summary>
    public class TestController : Controller
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The index.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// The tenants query async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> TenantsQueryAsync()
        {
            var testResult = new TestResult("TenantsQueryAsync", Guid.NewGuid());
            await RunTest(TestTenantsQueryAsyncInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The units query async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> UnitsQueryAsync()
        {
            var testResult = new TestResult("UnitsQueryAsync", Guid.NewGuid());
            await RunTest(TestUnitsQueryAsyncInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The tenant with users query async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> TenantWithUsersQueryAsync()
        {
            var testResult = new TestResult("TenantWithUsersQueryAsync", Guid.NewGuid());
            await RunTest(TestTenantWithUsersQueryInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The unit with update commands query async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> UnitWithUpdateCommandsQueryAsync()
        {
            var testResult = new TestResult("UnitWithUpdateCommandsQueryAsync", Guid.NewGuid());
            await RunTest(TestUnitWithUpdateCommandsQueryInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The change tracking manager async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> ChangeTrackingManagerAsync()
        {
            var testResult = new TestResult("ChangeTrackingManagerAsync", Guid.NewGuid());
            await RunTest(TestChangeTrackingManagerInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The association user role filter async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> AssociationUserRoleFilterAsync()
        {
            var testResult = new TestResult("AssociationUserRoleFilterAsync", Guid.NewGuid());
            await RunTest(TestAssociationUserRoleFilterInternal, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The authorization data scope filter async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> AuthorizationDataScopeFilterAsync()
        {
            var testResult = new TestResult("AuthorizationDataScopeFilterAsync", Guid.NewGuid());
            await RunTest(TestAuthorizationDataScopeFilterInternal, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The enum set on add async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> EnumSetOnAddAsync()
        {
            var testResult = new TestResult("EnumSetOnAddAsync", Guid.NewGuid());
            await RunTest(TestEnumSetOnAddInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// Tests the creation of an update group.
        /// </summary>
        /// <returns>
        /// The result of the action.
        /// </returns>
        /// <remarks>
        /// An update group represents a specific test case because it defines a collection property (UpdateGroups)
        /// without an inverse property.
        /// </remarks>
        public async Task<ActionResult> AddUpdateGroupAsync()
        {
            var testResult = new TestResult("AddUpdateGroupAsync", Guid.NewGuid());
            await RunTest(TestAddUpdateGroupInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The parent child async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> ParentChildAsync()
        {
            var testResult = new TestResult("ParentChildAsync", Guid.NewGuid());
            await RunTest(TestParentChildInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        /// <summary>
        /// The exception on commit async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ActionResult> ExceptionOnCommitAsync()
        {
            var testResult = new TestResult("ExceptionOnCommitAsync", Guid.NewGuid());
            await RunTest(TestExceptionOnCommitInternalAsync, testResult);
            return this.View("TestResult", testResult);
        }

        private static async Task RunTest(Func<TestResult, Task> test, TestResult testResult)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await test(testResult);
            }
            catch (Exception exception)
            {
                testResult.Exception = exception;
                testResult.ResultType = TestResultType.Error;
            }

            stopwatch.Stop();
            testResult.ElapsedTime = stopwatch.Elapsed;
        }

        private static async Task TestTenantsQueryAsyncInternalAsync(TestResult testResult)
        {
            using (var channelScope = CreateChannelScope<ITenantDataService>())
            {
                var tenants = (await channelScope.Channel.QueryAsync()).ToArray();
                testResult.ResultType = tenants.Length > 0 ? TestResultType.Success : TestResultType.Warning;
                testResult.Description = string.Format("The system returned {0} tenant(s)", tenants.Length);
            }
        }

        private static async Task TestUnitsQueryAsyncInternalAsync(TestResult testResult)
        {
            using (var channelScope = CreateChannelScope<IUnitDataService>())
            {
                var units = (await channelScope.Channel.QueryAsync()).ToArray();
                testResult.ResultType = units.Length > 0 ? TestResultType.Success : TestResultType.Warning;
                testResult.Description = string.Format("The system returned {0} unit(s)", units.Length);
            }
        }

        private static async Task TestTenantWithUsersQueryInternalAsync(TestResult testResult)
        {
            using (var channelScope = CreateChannelScope<ITenantDataService>())
            {
                var filter = TenantFilter.Create().IncludeUsers();
                var tenants = (await channelScope.Channel.QueryAsync(filter)).ToArray();
                var tenant = tenants.Last();
                testResult.ResultType = tenant.Users.Count > 0 ? TestResultType.Success : TestResultType.Warning;
                testResult.Description = string.Format("The system returned a tenant with {0} unit(s)", tenants.Length);
            }
        }

        private static async Task TestAddUpdateGroupInternalAsync(TestResult testResult)
        {
            var tenantChangeTrackingManager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
            var tenant = (await tenantChangeTrackingManager.QueryAsync()).First();
            var updateGroupChangeTrackingManager = DependencyResolver.Current.Get<IUpdateGroupChangeTrackingManager>();
            var updateGroup = updateGroupChangeTrackingManager.Create();
            updateGroup.Name = "My update group " + DateTime.Now;
            updateGroup.Tenant = tenant;
            try
            {
                await updateGroupChangeTrackingManager.CommitAndVerifyAsync(updateGroup);
            }
            catch (Exception)
            {
                testResult.Description = "Exception while committing the update group";
                testResult.ResultType = TestResultType.Error;
                return;
            }

            testResult.Description = "Update group successfully created";
            testResult.ResultType = TestResultType.Success;
        }

        private static async Task TestUnitWithUpdateCommandsQueryInternalAsync(TestResult testResult)
        {
            using (var channelScope = CreateChannelScope<IUnitDataService>())
            {
                var filter = UnitFilter.Create().IncludeUpdateCommands();
                var units = (await channelScope.Channel.QueryAsync(filter)).ToArray();
                var totalCommands = units.Sum(unit => unit.UpdateCommands.Count);
                testResult.ResultType = totalCommands == 8000 ? TestResultType.Success : TestResultType.Warning;
                testResult.Description =
                    string.Format(
                        "The system loaded {0} unit(s) with a total of {1} update command(s)",
                        units.Length,
                        totalCommands);
            }
        }

        private static async Task TestChangeTrackingManagerInternalAsync(TestResult testResult)
        {
            var tenantChangeTrackingManager = DependencyResolver.Current.Get<ITenantChangeTrackingManager>();
            var userChangeTrackingManager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
            var tenantsTask = tenantChangeTrackingManager.QueryAsync();
            var filter = UserFilter.Create().WithUsername("admin", StringComparison.Different);
            var usersTask = userChangeTrackingManager.QueryAsync(filter);
            var tenants = (await tenantsTask).ToArray();
            var users = (await usersTask).ToArray();
            var user = users.First();
            var waitPropertyChanged = new TaskCompletionSource<bool>();
            var newName = "FirstName " + DateTime.Now;
            PropertyChangedEventHandler propertyChangedEventHandler = (sender, args) =>
                {
                    var userModel = sender as UserReadableModel;
                    if (userModel == null || userModel.Id != user.Id)
                    {
                        return;
                    }

                    if (args.PropertyName != "FirstName")
                    {
                        return;
                    }

                    waitPropertyChanged.SetResult(user.FirstName == newName);
                };
            user.PropertyChanged += propertyChangedEventHandler;
            var waitOtherTenant = new TaskCompletionSource<bool>();
            var ownerTenant = tenants.SingleOrDefault(model => model.Id == user.OwnerTenant.Id);
            var otherTenant = tenants.First(model => model.Id != user.OwnerTenant.Id);
            await otherTenant.LoadNavigationPropertiesAsync();
            NotifyCollectionChangedEventHandler usersOnCollectionChanged = (o, args) =>
                {
                    Console.WriteLine("Users collection changed");
                    waitOtherTenant.SetResult(
                        args.Action == NotifyCollectionChangedAction.Add
                        && ((UserReadableModel)args.NewItems[0]).Id == user.Id);
                };
            otherTenant.Users.CollectionChanged += usersOnCollectionChanged;
            var userWritableModel = user.ToChangeTrackingModel();
            userWritableModel.OwnerTenant = otherTenant;
            userWritableModel.FirstName = newName;
            var task = userChangeTrackingManager.CommitAndVerifyAsync(userWritableModel);
            var delay = Task.Delay(Timeout);
            await Task.WhenAny(delay, task);
            if (delay.IsCompleted)
            {
                testResult.ResultType = TestResultType.Warning;
                testResult.Description = "Timeout while waiting the commit";
                otherTenant.Users.CollectionChanged -= usersOnCollectionChanged;
                user.PropertyChanged -= propertyChangedEventHandler;
                return;
            }

            delay = Task.Delay(Timeout);
            await Task.WhenAny(delay, waitOtherTenant.Task, waitPropertyChanged.Task);
            if (delay.IsCompleted)
            {
                testResult.ResultType = TestResultType.Warning;
                testResult.Description = "Timeout while waiting the collection change";
                otherTenant.Users.CollectionChanged -= usersOnCollectionChanged;
                user.PropertyChanged -= propertyChangedEventHandler;
                return;
            }

            var collectionChangedResult = await waitOtherTenant.Task;
            var propertyChangedResult = await waitPropertyChanged.Task;
            testResult.ResultType = collectionChangedResult && propertyChangedResult
                                        ? TestResultType.Success
                                        : TestResultType.Warning;
            testResult.Description =
                string.Format(
                    "Change tracking test completed. Collection changed: {0}, property changed: {1}",
                    collectionChangedResult,
                    propertyChangedResult);
            otherTenant.Users.CollectionChanged -= usersOnCollectionChanged;
            user.PropertyChanged -= propertyChangedEventHandler;
        }

        private static async Task TestAssociationUserRoleFilterInternal(TestResult testResult)
        {
            User user;
            using (var scope = CreateChannelScope<IUserDataService>())
            {
                user = (await scope.Channel.QueryAsync()).Last();
            }

            using (var scope = CreateChannelScope<IAssociationTenantUserUserRoleDataService>())
            {
                var filter =
                    AssociationTenantUserUserRoleFilter.Create().IncludeTenant().IncludeUserRole().WithUser(user);
                var values = (await scope.Channel.QueryAsync(filter)).ToArray();
                testResult.ResultType = values.Length > 0 ? TestResultType.Success : TestResultType.Warning;
                testResult.Description = string.Format("Found {0} association(s) for the given user", values.Length);
            }
        }

        private static async Task TestAuthorizationDataScopeFilterInternal(TestResult testResult)
        {
            using (var scope = CreateChannelScope<IAuthorizationDataService>())
            {
                var filter = AuthorizationFilter.Create().IncludeUserRole().WithDataScope(DataScope.MediaConfiguration);
                var valuesTask = scope.Channel.QueryAsync(filter);
                filter =
                    AuthorizationFilter.Create()
                        .IncludeUserRole()
                        .WithDataScope(DataScope.MediaConfiguration)
                        .WithPermission(Permission.Read);
                var values2Task = scope.Channel.QueryAsync(filter);
                var timeout = Task.Delay(Timeout);
                var wait = Task.WaitAny(timeout, Task.WhenAll(valuesTask, values2Task));
                if (timeout.IsCompleted)
                {
                    testResult.ResultType = TestResultType.Error;
                    testResult.Description = "Timeout for the requests";
                    return;
                }

                var values = (await valuesTask).ToArray();
                var values2 = (await values2Task).ToArray();
                testResult.ResultType = values.Length == 2 && values2.Length == 1
                                            ? TestResultType.Success
                                            : TestResultType.Warning;
                testResult.Description = string.Format(
                    "Found {0} and {1} authorization(s) for the given data scope",
                    values.Length,
                    values2.Length);
            }
        }

        private static async Task TestEnumSetOnAddInternalAsync(TestResult testResult)
        {
            var userRoleChangeTrackingManager = DependencyResolver.Current.Get<IUserRoleChangeTrackingManager>();
            var userRole = (await userRoleChangeTrackingManager.QueryAsync()).First();
            var authorizationChangeManager = DependencyResolver.Current.Get<IAuthorizationChangeTrackingManager>();
            var newAuthorization = authorizationChangeManager.Create();
            newAuthorization.DataScope = DataScope.Resource;
            newAuthorization.Permission = Permission.Interact;
            newAuthorization.UserRole = userRole;
            var createdAuthorization = await authorizationChangeManager.CommitAndVerifyAsync(newAuthorization);
            var addedAuthorization = await authorizationChangeManager.GetAsync(createdAuthorization.Id);
            testResult.ResultType = addedAuthorization.DataScope == DataScope.Resource
                                    && addedAuthorization.Permission == Permission.Interact
                                        ? TestResultType.Success
                                        : TestResultType.Warning;
            testResult.Description = string.Format("Added authorization {0}", addedAuthorization.Id);
        }

        private static async Task TestParentChildInternalAsync(TestResult testResult)
        {
            var associationChangeTrackingManager =
                DependencyResolver.Current.Get<IAssociationTenantUserUserRoleChangeTrackingManager>();
            var associations = await associationChangeTrackingManager.QueryAsync();
            foreach (var association in associations)
            {
                await association.User.LoadNavigationPropertiesAsync();
                var count = association.User.AssociationTenantUserUserRoles.Count;
            }

            // var userChangeTrackingManager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
        }

        private static async Task TestExceptionOnCommitInternalAsync(TestResult testResult)
        {
            await Task.FromResult(0);
            var userChangeTrackingManager = DependencyResolver.Current.Get<IUserChangeTrackingManager>();
            var user = userChangeTrackingManager.Create();
            try
            {
                await userChangeTrackingManager.CommitAndVerifyAsync(user);
            }
            catch (ChangeTrackingException)
            {
                testResult.ResultType = TestResultType.Success;
                testResult.Description = "CommitAndVerifyAsync completed";
                return;
            }

            testResult.ResultType = TestResultType.Warning;
            testResult.Description = "Exception not thrown by CommitAndVerifyAsync";
        }

        private static ChannelScope<T> CreateChannelScope<T>() where T : class
        {
            return ChannelScopeFactory<T>.Current.Create(
                new UserCredentials("admin", BackgroundSystemConfig.HashedPassword));
        }
    }
}