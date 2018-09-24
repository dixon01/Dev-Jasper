namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;

    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    using Gorba.Center.Admin.Core.DataViewModels;

    public partial class DataController
    {
        private void CreateControllers()
        {
            this.UserRole = new AccessControl.UserRoleDataController(this);
            this.Authorization = new AccessControl.AuthorizationDataController(this);
            this.Tenant = new Membership.TenantDataController(this);
            this.User = new Membership.UserDataController(this);
            this.AssociationTenantUserUserRole = new Membership.AssociationTenantUserUserRoleDataController(this);
            this.ProductType = new Units.ProductTypeDataController(this);
            this.Unit = new Units.UnitDataController(this);
            this.Resource = new Resources.ResourceDataController(this);
            this.UpdateGroup = new Update.UpdateGroupDataController(this);
            this.UpdatePart = new Update.UpdatePartDataController(this);
            this.UpdateCommand = new Update.UpdateCommandDataController(this);
            this.UpdateFeedback = new Update.UpdateFeedbackDataController(this);
            this.Document = new Documents.DocumentDataController(this);
            this.DocumentVersion = new Documents.DocumentVersionDataController(this);
            this.Package = new Software.PackageDataController(this);
            this.PackageVersion = new Software.PackageVersionDataController(this);
            this.UnitConfiguration = new Configurations.UnitConfigurationDataController(this);
            this.MediaConfiguration = new Configurations.MediaConfigurationDataController(this);
            this.UserDefinedProperty = new Meta.UserDefinedPropertyDataController(this);
            this.SystemConfig = new Meta.SystemConfigDataController(this);
        }

        private void InitializeControllers()
        {
            this.UserRole.Initialize(this.ConnectionController);
            this.Authorization.Initialize(this.ConnectionController);
            this.Tenant.Initialize(this.ConnectionController);
            this.User.Initialize(this.ConnectionController);
            this.AssociationTenantUserUserRole.Initialize(this.ConnectionController);
            this.ProductType.Initialize(this.ConnectionController);
            this.Unit.Initialize(this.ConnectionController);
            this.Resource.Initialize(this.ConnectionController);
            this.UpdateGroup.Initialize(this.ConnectionController);
            this.UpdatePart.Initialize(this.ConnectionController);
            this.UpdateCommand.Initialize(this.ConnectionController);
            this.UpdateFeedback.Initialize(this.ConnectionController);
            this.Document.Initialize(this.ConnectionController);
            this.DocumentVersion.Initialize(this.ConnectionController);
            this.Package.Initialize(this.ConnectionController);
            this.PackageVersion.Initialize(this.ConnectionController);
            this.UnitConfiguration.Initialize(this.ConnectionController);
            this.MediaConfiguration.Initialize(this.ConnectionController);
            this.UserDefinedProperty.Initialize(this.ConnectionController);
            this.SystemConfig.Initialize(this.ConnectionController);
        }

        public AccessControl.UserRoleDataController UserRole { get; private set; }

        public AccessControl.AuthorizationDataController Authorization { get; private set; }

        public Membership.TenantDataController Tenant { get; private set; }

        public Membership.UserDataController User { get; private set; }

        public Membership.AssociationTenantUserUserRoleDataController AssociationTenantUserUserRole { get; private set; }

        public Units.ProductTypeDataController ProductType { get; private set; }

        public Units.UnitDataController Unit { get; private set; }

        public Resources.ResourceDataController Resource { get; private set; }

        public Update.UpdateGroupDataController UpdateGroup { get; private set; }

        public Update.UpdatePartDataController UpdatePart { get; private set; }

        public Update.UpdateCommandDataController UpdateCommand { get; private set; }

        public Update.UpdateFeedbackDataController UpdateFeedback { get; private set; }

        public Documents.DocumentDataController Document { get; private set; }

        public Documents.DocumentVersionDataController DocumentVersion { get; private set; }

        public Software.PackageDataController Package { get; private set; }

        public Software.PackageVersionDataController PackageVersion { get; private set; }

        public Configurations.UnitConfigurationDataController UnitConfiguration { get; private set; }

        public Configurations.MediaConfigurationDataController MediaConfiguration { get; private set; }

        public Meta.UserDefinedPropertyDataController UserDefinedProperty { get; private set; }

        public Meta.SystemConfigDataController SystemConfig { get; private set; }
    }


    namespace AccessControl
    {
        using Gorba.Center.Admin.Core.DataViewModels.AccessControl;
        using Gorba.Center.Admin.Core.ViewModels.Stages.AccessControl;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.AccessControl;
        using Gorba.Center.Common.ServiceModel.Filters.AccessControl;


        public partial class UserRoleDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UserRoleReadOnlyDataViewModel> allUserRoles =
                new ObservableCollection<UserRoleReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel> readOnlyUserRoles;

            private TaskCompletionSource<bool> loadAllCompletion;

            private TaskCompletionSource<IUserRoleUdpContext> udpContextCompletion;

            public UserRoleDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUserRoles != null)
                    {
                        return this.readOnlyUserRoles;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUserRoles != null)
                        {
                            return this.readOnlyUserRoles;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUserRoles;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUserRoleChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UserRoleChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UserRoleChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserRoleReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UserRoleChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UserRoleChangeTrackingManager.Create();
                var udpContext = await this.GetUdpContextAsync();
                var dvm = this.Factory.Create(model, udpContext);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UserRoleReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UserRoleReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UserRoleChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UserRoleDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UserRoleDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UserRoleChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UserRoleReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UserRoleChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UserRoleReadOnlyDataViewModel");
                }

                readOnly.Authorizations.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Authorizations)
                    {
                        readOnly.Authorizations.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Authorizations.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Authorizations.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (AuthorizationReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Authorizations.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Authorizations.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Authorizations.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Authorizations.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UserRoleReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UserRoleReadableModel>, Task<IEnumerable<UserRoleReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UserRoleDataViewModel dataViewModel);

            partial void PostCopyEntity(UserRoleDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UserRoleDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UserRoleDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UserRoleDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UserRoleDataViewModel, UserRoleReadableModel, Task<UserRoleReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UserRoleQuery query);

            private void RunFilter(ref Func<UserRoleReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UserRoleReadableModel>, Task<IEnumerable<UserRoleReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UserRoleDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UserRoleDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UserRoleDataViewModel, UserRoleReadableModel, Task<UserRoleReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                this.udpContextCompletion = null;
                if (this.readOnlyUserRoles == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUserRoles =
                        new ReadOnlyEntityCollection<UserRoleReadOnlyDataViewModel>(this.allUserRoles);
                }
                else
                {
                    this.allUserRoles.Clear();
                }

                this.readOnlyUserRoles.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UserRoleQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UserRoleChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUserRoles.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUserRoles.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UserRoleDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UserRoleValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UserRoleDataViewModel dataViewModel)
            {
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UserRoleReadOnlyDataViewModel> CreateReadOnlyAsync(UserRoleReadableModel model)
            {
                var udpContext = await this.GetUdpContextAsync();
                return this.Factory.CreateReadOnly(model, udpContext);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UserRoleReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUserRoles == null || this.readOnlyUserRoles.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUserRoles.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UserRole", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UserRoleReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUserRoles == null || this.readOnlyUserRoles.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUserRoles.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUserRoles.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UserRole", ex);
                }
            }

            private async Task<IUserRoleUdpContext> GetUdpContextAsync()
            {
                if (this.udpContextCompletion != null)
                {
                    return await this.udpContextCompletion.Task;
                }

                var completion = this.udpContextCompletion = new TaskCompletionSource<IUserRoleUdpContext>();

                await this.DataController.UserDefinedProperty.AwaitAllDataAsync();
                var context =
                    new UserRoleUdpContext(
                        this.DataController.UserDefinedProperty.All.Where(
                            u =>
                            u.OwnerEntity == Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity.UserRole
                            && (u.Tenant == null || u.Tenant.Id == this.ApplicationState.CurrentTenant.Id))
                            .Select(u => u.ReadableModel.Name));

                completion.TrySetResult(context);
                return context;
            }

            private class UserRoleUdpContext : IUserRoleUdpContext
            {
                private List<string> udpNames;

                public UserRoleUdpContext(IEnumerable<string> udpNames)
                {
                    this.udpNames = udpNames.ToList();
                }

                public IEnumerable<string> GetAdditionalUserRoleProperties()
                {
                    return this.udpNames;
                }
            }
        }


        public partial class AuthorizationDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<AuthorizationReadOnlyDataViewModel> allAuthorizations =
                new ObservableCollection<AuthorizationReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<AuthorizationReadOnlyDataViewModel> readOnlyAuthorizations;

            private TaskCompletionSource<bool> loadAllCompletion;

            public AuthorizationDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<AuthorizationReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyAuthorizations != null)
                    {
                        return this.readOnlyAuthorizations;
                    }

                    lock (this)
                    {
                        if (this.readOnlyAuthorizations != null)
                        {
                            return this.readOnlyAuthorizations;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyAuthorizations;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IAuthorizationChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.AuthorizationChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.AuthorizationChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is AuthorizationReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.AuthorizationChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.AuthorizationChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AuthorizationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a AuthorizationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AuthorizationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a AuthorizationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.AuthorizationChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as AuthorizationDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a AuthorizationDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.AuthorizationChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AuthorizationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a AuthorizationReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.AuthorizationChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AuthorizationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a AuthorizationReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<AuthorizationReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<AuthorizationReadableModel>, Task<IEnumerable<AuthorizationReadableModel>>> asyncMethod);

            partial void PostCreateEntity(AuthorizationDataViewModel dataViewModel);

            partial void PostCopyEntity(AuthorizationDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<AuthorizationDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(AuthorizationDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<AuthorizationDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<AuthorizationDataViewModel, AuthorizationReadableModel, Task<AuthorizationReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref AuthorizationQuery query);

            private void RunFilter(ref Func<AuthorizationReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<AuthorizationReadableModel>, Task<IEnumerable<AuthorizationReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<AuthorizationDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<AuthorizationDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<AuthorizationDataViewModel, AuthorizationReadableModel, Task<AuthorizationReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<AuthorizationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyAuthorizations == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyAuthorizations =
                        new ReadOnlyEntityCollection<AuthorizationReadOnlyDataViewModel>(this.allAuthorizations);
                }
                else
                {
                    this.allAuthorizations.Clear();
                }

                this.readOnlyAuthorizations.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            AuthorizationQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.AuthorizationChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allAuthorizations.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyAuthorizations.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(AuthorizationDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new AuthorizationValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(AuthorizationDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.UserRole.Entities = this.DataController.UserRole.All;
                dataViewModel.UserRole.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.UserRole.Entities = null;
                await this.DataController.UserRole.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<AuthorizationReadOnlyDataViewModel> CreateReadOnlyAsync(AuthorizationReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<AuthorizationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyAuthorizations == null || this.readOnlyAuthorizations.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allAuthorizations.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Authorization", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<AuthorizationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyAuthorizations == null || this.readOnlyAuthorizations.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allAuthorizations.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allAuthorizations.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Authorization", ex);
                }
            }
        }

    }

    namespace Membership
    {
        using Gorba.Center.Admin.Core.DataViewModels.Membership;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Membership;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
        using Gorba.Center.Common.ServiceModel.Filters.Membership;


        public partial class TenantDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<TenantReadOnlyDataViewModel> allTenants =
                new ObservableCollection<TenantReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<TenantReadOnlyDataViewModel> readOnlyTenants;

            private TaskCompletionSource<bool> loadAllCompletion;

            private TaskCompletionSource<ITenantUdpContext> udpContextCompletion;

            public TenantDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<TenantReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyTenants != null)
                    {
                        return this.readOnlyTenants;
                    }

                    lock (this)
                    {
                        if (this.readOnlyTenants != null)
                        {
                            return this.readOnlyTenants;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyTenants;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                ITenantChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.TenantChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.TenantChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is TenantReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.TenantChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.TenantChangeTrackingManager.Create();
                var udpContext = await this.GetUdpContextAsync();
                var dvm = this.Factory.Create(model, udpContext);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as TenantReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a TenantReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as TenantReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a TenantReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.TenantChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as TenantDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a TenantDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.TenantChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as TenantReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a TenantReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.TenantChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as TenantReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a TenantReadOnlyDataViewModel");
                }

                readOnly.Users.IsLoading = true;
                readOnly.UpdateGroups.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Users)
                    {
                        readOnly.Users.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Users.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Users.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UserReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Users.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Users.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Users.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Users.IsLoading = false;
                }

                try
                {
                    foreach (var reference in readOnly.ReadableModel.UpdateGroups)
                    {
                        readOnly.UpdateGroups.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.UpdateGroups.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.UpdateGroups.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateGroupReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.UpdateGroups.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.UpdateGroups.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.UpdateGroups.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.UpdateGroups.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<TenantReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<TenantReadableModel>, Task<IEnumerable<TenantReadableModel>>> asyncMethod);

            partial void PostCreateEntity(TenantDataViewModel dataViewModel);

            partial void PostCopyEntity(TenantDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<TenantDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(TenantDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<TenantDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<TenantDataViewModel, TenantReadableModel, Task<TenantReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref TenantQuery query);

            private void RunFilter(ref Func<TenantReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<TenantReadableModel>, Task<IEnumerable<TenantReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<TenantDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<TenantDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<TenantDataViewModel, TenantReadableModel, Task<TenantReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<TenantReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                this.udpContextCompletion = null;
                if (this.readOnlyTenants == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyTenants =
                        new ReadOnlyEntityCollection<TenantReadOnlyDataViewModel>(this.allTenants);
                }
                else
                {
                    this.allTenants.Clear();
                }

                this.readOnlyTenants.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            TenantQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.TenantChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allTenants.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyTenants.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(TenantDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new TenantValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(TenantDataViewModel dataViewModel)
            {
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<TenantReadOnlyDataViewModel> CreateReadOnlyAsync(TenantReadableModel model)
            {
                var udpContext = await this.GetUdpContextAsync();
                return this.Factory.CreateReadOnly(model, udpContext);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<TenantReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyTenants == null || this.readOnlyTenants.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allTenants.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Tenant", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<TenantReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyTenants == null || this.readOnlyTenants.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allTenants.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allTenants.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Tenant", ex);
                }
            }

            private async Task<ITenantUdpContext> GetUdpContextAsync()
            {
                if (this.udpContextCompletion != null)
                {
                    return await this.udpContextCompletion.Task;
                }

                var completion = this.udpContextCompletion = new TaskCompletionSource<ITenantUdpContext>();

                await this.DataController.UserDefinedProperty.AwaitAllDataAsync();
                var context =
                    new TenantUdpContext(
                        this.DataController.UserDefinedProperty.All.Where(
                            u =>
                            u.OwnerEntity == Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity.Tenant
                            && (u.Tenant == null || u.Tenant.Id == this.ApplicationState.CurrentTenant.Id))
                            .Select(u => u.ReadableModel.Name));

                completion.TrySetResult(context);
                return context;
            }

            private class TenantUdpContext : ITenantUdpContext
            {
                private List<string> udpNames;

                public TenantUdpContext(IEnumerable<string> udpNames)
                {
                    this.udpNames = udpNames.ToList();
                }

                public IEnumerable<string> GetAdditionalTenantProperties()
                {
                    return this.udpNames;
                }
            }
        }


        public partial class UserDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UserReadOnlyDataViewModel> allUsers =
                new ObservableCollection<UserReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UserReadOnlyDataViewModel> readOnlyUsers;

            private TaskCompletionSource<bool> loadAllCompletion;

            private TaskCompletionSource<IUserUdpContext> udpContextCompletion;

            public UserDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UserReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUsers != null)
                    {
                        return this.readOnlyUsers;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUsers != null)
                        {
                            return this.readOnlyUsers;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUsers;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUserChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UserChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UserChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UserChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UserChangeTrackingManager.Create();
                var udpContext = await this.GetUdpContextAsync();
                var dvm = this.Factory.Create(model, udpContext);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UserReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UserReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UserChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UserDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UserDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UserChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UserReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UserChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UserReadOnlyDataViewModel");
                }

                readOnly.AssociationTenantUserUserRoles.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.AssociationTenantUserUserRoles)
                    {
                        readOnly.AssociationTenantUserUserRoles.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.AssociationTenantUserUserRoles.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.AssociationTenantUserUserRoles.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (AssociationTenantUserUserRoleReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.AssociationTenantUserUserRoles.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.AssociationTenantUserUserRoles.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.AssociationTenantUserUserRoles.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.AssociationTenantUserUserRoles.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UserReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UserReadableModel>, Task<IEnumerable<UserReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UserDataViewModel dataViewModel);

            partial void PostCopyEntity(UserDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UserDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UserDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UserDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UserDataViewModel, UserReadableModel, Task<UserReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UserQuery query);

            private void RunFilter(ref Func<UserReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UserReadableModel>, Task<IEnumerable<UserReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UserDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UserDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UserDataViewModel, UserReadableModel, Task<UserReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UserReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                this.udpContextCompletion = null;
                if (this.readOnlyUsers == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUsers =
                        new ReadOnlyEntityCollection<UserReadOnlyDataViewModel>(this.allUsers);
                }
                else
                {
                    this.allUsers.Clear();
                }

                this.readOnlyUsers.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UserQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UserChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUsers.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUsers.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UserDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UserValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UserDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.OwnerTenant.Entities = this.DataController.Tenant.All;
                dataViewModel.OwnerTenant.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.OwnerTenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UserReadOnlyDataViewModel> CreateReadOnlyAsync(UserReadableModel model)
            {
                var udpContext = await this.GetUdpContextAsync();
                return this.Factory.CreateReadOnly(model, udpContext);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UserReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUsers == null || this.readOnlyUsers.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUsers.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added User", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UserReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUsers == null || this.readOnlyUsers.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUsers.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUsers.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove User", ex);
                }
            }

            private async Task<IUserUdpContext> GetUdpContextAsync()
            {
                if (this.udpContextCompletion != null)
                {
                    return await this.udpContextCompletion.Task;
                }

                var completion = this.udpContextCompletion = new TaskCompletionSource<IUserUdpContext>();

                await this.DataController.UserDefinedProperty.AwaitAllDataAsync();
                var context =
                    new UserUdpContext(
                        this.DataController.UserDefinedProperty.All.Where(
                            u =>
                            u.OwnerEntity == Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity.User
                            && (u.Tenant == null || u.Tenant.Id == this.ApplicationState.CurrentTenant.Id))
                            .Select(u => u.ReadableModel.Name));

                completion.TrySetResult(context);
                return context;
            }

            private class UserUdpContext : IUserUdpContext
            {
                private List<string> udpNames;

                public UserUdpContext(IEnumerable<string> udpNames)
                {
                    this.udpNames = udpNames.ToList();
                }

                public IEnumerable<string> GetAdditionalUserProperties()
                {
                    return this.udpNames;
                }
            }
        }


        public partial class AssociationTenantUserUserRoleDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> allAssociationTenantUserUserRoles =
                new ObservableCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> readOnlyAssociationTenantUserUserRoles;

            private TaskCompletionSource<bool> loadAllCompletion;

            public AssociationTenantUserUserRoleDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyAssociationTenantUserUserRoles != null)
                    {
                        return this.readOnlyAssociationTenantUserUserRoles;
                    }

                    lock (this)
                    {
                        if (this.readOnlyAssociationTenantUserUserRoles != null)
                        {
                            return this.readOnlyAssociationTenantUserUserRoles;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyAssociationTenantUserUserRoles;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IAssociationTenantUserUserRoleChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is AssociationTenantUserUserRoleReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AssociationTenantUserUserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a AssociationTenantUserUserRoleReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AssociationTenantUserUserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a AssociationTenantUserUserRoleReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as AssociationTenantUserUserRoleDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a AssociationTenantUserUserRoleDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AssociationTenantUserUserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a AssociationTenantUserUserRoleReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as AssociationTenantUserUserRoleReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a AssociationTenantUserUserRoleReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<AssociationTenantUserUserRoleReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<AssociationTenantUserUserRoleReadableModel>, Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>>> asyncMethod);

            partial void PostCreateEntity(AssociationTenantUserUserRoleDataViewModel dataViewModel);

            partial void PostCopyEntity(AssociationTenantUserUserRoleDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<AssociationTenantUserUserRoleDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(AssociationTenantUserUserRoleDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<AssociationTenantUserUserRoleDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<AssociationTenantUserUserRoleDataViewModel, AssociationTenantUserUserRoleReadableModel, Task<AssociationTenantUserUserRoleReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref AssociationTenantUserUserRoleQuery query);

            private void RunFilter(ref Func<AssociationTenantUserUserRoleReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<AssociationTenantUserUserRoleReadableModel>, Task<IEnumerable<AssociationTenantUserUserRoleReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<AssociationTenantUserUserRoleDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<AssociationTenantUserUserRoleDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<AssociationTenantUserUserRoleDataViewModel, AssociationTenantUserUserRoleReadableModel, Task<AssociationTenantUserUserRoleReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<AssociationTenantUserUserRoleReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyAssociationTenantUserUserRoles == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyAssociationTenantUserUserRoles =
                        new ReadOnlyEntityCollection<AssociationTenantUserUserRoleReadOnlyDataViewModel>(this.allAssociationTenantUserUserRoles);
                }
                else
                {
                    this.allAssociationTenantUserUserRoles.Clear();
                }

                this.readOnlyAssociationTenantUserUserRoles.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            AssociationTenantUserUserRoleQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.AssociationTenantUserUserRoleChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allAssociationTenantUserUserRoles.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyAssociationTenantUserUserRoles.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(AssociationTenantUserUserRoleDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new AssociationTenantUserUserRoleValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(AssociationTenantUserUserRoleDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Tenant.Entities = this.DataController.Tenant.All;
                dataViewModel.Tenant.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.Tenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();

                dataViewModel.User.Entities = this.DataController.User.All;
                dataViewModel.User.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.User.Entities = null;
                await this.DataController.User.AwaitAllDataAsync();

                dataViewModel.UserRole.Entities = this.DataController.UserRole.All;
                dataViewModel.UserRole.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.UserRole.Entities = null;
                await this.DataController.UserRole.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<AssociationTenantUserUserRoleReadOnlyDataViewModel> CreateReadOnlyAsync(AssociationTenantUserUserRoleReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyAssociationTenantUserUserRoles == null || this.readOnlyAssociationTenantUserUserRoles.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allAssociationTenantUserUserRoles.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added AssociationTenantUserUserRole", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<AssociationTenantUserUserRoleReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyAssociationTenantUserUserRoles == null || this.readOnlyAssociationTenantUserUserRoles.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allAssociationTenantUserUserRoles.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allAssociationTenantUserUserRoles.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove AssociationTenantUserUserRole", ex);
                }
            }
        }

    }

    namespace Units
    {
        using Gorba.Center.Admin.Core.DataViewModels.Units;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Units;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
        using Gorba.Center.Common.ServiceModel.Filters.Units;


        public partial class ProductTypeDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<ProductTypeReadOnlyDataViewModel> allProductTypes =
                new ObservableCollection<ProductTypeReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<ProductTypeReadOnlyDataViewModel> readOnlyProductTypes;

            private TaskCompletionSource<bool> loadAllCompletion;

            public ProductTypeDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<ProductTypeReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyProductTypes != null)
                    {
                        return this.readOnlyProductTypes;
                    }

                    lock (this)
                    {
                        if (this.readOnlyProductTypes != null)
                        {
                            return this.readOnlyProductTypes;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyProductTypes;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IProductTypeChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.ProductTypeChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.ProductTypeChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is ProductTypeReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.ProductTypeChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.ProductTypeChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ProductTypeReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a ProductTypeReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ProductTypeReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a ProductTypeReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.ProductTypeChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as ProductTypeDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a ProductTypeDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.ProductTypeChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ProductTypeReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a ProductTypeReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.ProductTypeChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ProductTypeReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a ProductTypeReadOnlyDataViewModel");
                }

                readOnly.Units.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Units)
                    {
                        readOnly.Units.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Units.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Units.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UnitReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Units.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Units.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Units.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Units.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<ProductTypeReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<ProductTypeReadableModel>, Task<IEnumerable<ProductTypeReadableModel>>> asyncMethod);

            partial void PostCreateEntity(ProductTypeDataViewModel dataViewModel);

            partial void PostCopyEntity(ProductTypeDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<ProductTypeDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(ProductTypeDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<ProductTypeDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<ProductTypeDataViewModel, ProductTypeReadableModel, Task<ProductTypeReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref ProductTypeQuery query);

            private void RunFilter(ref Func<ProductTypeReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<ProductTypeReadableModel>, Task<IEnumerable<ProductTypeReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<ProductTypeDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<ProductTypeDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<ProductTypeDataViewModel, ProductTypeReadableModel, Task<ProductTypeReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<ProductTypeReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyProductTypes == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyProductTypes =
                        new ReadOnlyEntityCollection<ProductTypeReadOnlyDataViewModel>(this.allProductTypes);
                }
                else
                {
                    this.allProductTypes.Clear();
                }

                this.readOnlyProductTypes.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            ProductTypeQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.ProductTypeChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allProductTypes.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyProductTypes.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(ProductTypeDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new ProductTypeValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(ProductTypeDataViewModel dataViewModel)
            {
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<ProductTypeReadOnlyDataViewModel> CreateReadOnlyAsync(ProductTypeReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<ProductTypeReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyProductTypes == null || this.readOnlyProductTypes.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allProductTypes.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added ProductType", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<ProductTypeReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyProductTypes == null || this.readOnlyProductTypes.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allProductTypes.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allProductTypes.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove ProductType", ex);
                }
            }
        }


        public partial class UnitDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UnitReadOnlyDataViewModel> allUnits =
                new ObservableCollection<UnitReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UnitReadOnlyDataViewModel> readOnlyUnits;

            private TaskCompletionSource<bool> loadAllCompletion;

            private TaskCompletionSource<IUnitUdpContext> udpContextCompletion;

            public UnitDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UnitReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUnits != null)
                    {
                        return this.readOnlyUnits;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUnits != null)
                        {
                            return this.readOnlyUnits;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUnits;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUnitChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UnitChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UnitChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UnitChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UnitChangeTrackingManager.Create();
                var udpContext = await this.GetUdpContextAsync();
                var dvm = this.Factory.Create(model, udpContext);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UnitReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UnitReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UnitChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UnitDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UnitDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UnitChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UnitReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UnitChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UnitReadOnlyDataViewModel");
                }

                readOnly.UpdateCommands.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.UpdateCommands)
                    {
                        readOnly.UpdateCommands.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.UpdateCommands.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.UpdateCommands.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateCommandReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.UpdateCommands.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.UpdateCommands.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.UpdateCommands.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.UpdateCommands.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UnitReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UnitReadableModel>, Task<IEnumerable<UnitReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UnitDataViewModel dataViewModel);

            partial void PostCopyEntity(UnitDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UnitDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UnitDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UnitDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UnitDataViewModel, UnitReadableModel, Task<UnitReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UnitQuery query);

            private void RunFilter(ref Func<UnitReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UnitReadableModel>, Task<IEnumerable<UnitReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UnitDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UnitDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UnitDataViewModel, UnitReadableModel, Task<UnitReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UnitReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                this.udpContextCompletion = null;
                if (this.readOnlyUnits == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUnits =
                        new ReadOnlyEntityCollection<UnitReadOnlyDataViewModel>(this.allUnits);
                }
                else
                {
                    this.allUnits.Clear();
                }

                this.readOnlyUnits.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UnitQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UnitChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUnits.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUnits.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UnitDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UnitValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UnitDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Tenant.Entities = this.DataController.Tenant.All;
                dataViewModel.Tenant.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Tenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();

                dataViewModel.ProductType.Entities = this.DataController.ProductType.All;
                dataViewModel.ProductType.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.ProductType.Entities = null;
                await this.DataController.ProductType.AwaitAllDataAsync();

                dataViewModel.UpdateGroup.Entities = this.DataController.UpdateGroup.All;
                dataViewModel.UpdateGroup.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.UpdateGroup.Entities = null;
                await this.DataController.UpdateGroup.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UnitReadOnlyDataViewModel> CreateReadOnlyAsync(UnitReadableModel model)
            {
                var udpContext = await this.GetUdpContextAsync();
                return this.Factory.CreateReadOnly(model, udpContext);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UnitReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUnits == null || this.readOnlyUnits.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUnits.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Unit", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UnitReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUnits == null || this.readOnlyUnits.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUnits.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUnits.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Unit", ex);
                }
            }

            private async Task<IUnitUdpContext> GetUdpContextAsync()
            {
                if (this.udpContextCompletion != null)
                {
                    return await this.udpContextCompletion.Task;
                }

                var completion = this.udpContextCompletion = new TaskCompletionSource<IUnitUdpContext>();

                await this.DataController.UserDefinedProperty.AwaitAllDataAsync();
                var context =
                    new UnitUdpContext(
                        this.DataController.UserDefinedProperty.All.Where(
                            u =>
                            u.OwnerEntity == Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity.Unit
                            && (u.Tenant == null || u.Tenant.Id == this.ApplicationState.CurrentTenant.Id))
                            .Select(u => u.ReadableModel.Name));

                completion.TrySetResult(context);
                return context;
            }

            private class UnitUdpContext : IUnitUdpContext
            {
                private List<string> udpNames;

                public UnitUdpContext(IEnumerable<string> udpNames)
                {
                    this.udpNames = udpNames.ToList();
                }

                public IEnumerable<string> GetAdditionalUnitProperties()
                {
                    return this.udpNames;
                }
            }
        }

    }

    namespace Resources
    {
        using Gorba.Center.Admin.Core.DataViewModels.Resources;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Resources;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Resources;
        using Gorba.Center.Common.ServiceModel.Filters.Resources;


        public partial class ResourceDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<ResourceReadOnlyDataViewModel> allResources =
                new ObservableCollection<ResourceReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<ResourceReadOnlyDataViewModel> readOnlyResources;

            private TaskCompletionSource<bool> loadAllCompletion;

            public ResourceDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<ResourceReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyResources != null)
                    {
                        return this.readOnlyResources;
                    }

                    lock (this)
                    {
                        if (this.readOnlyResources != null)
                        {
                            return this.readOnlyResources;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyResources;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IResourceChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.ResourceChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.ResourceChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is ResourceReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.ResourceChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.ResourceChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ResourceReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a ResourceReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ResourceReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a ResourceReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.ResourceChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as ResourceDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a ResourceDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.ResourceChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ResourceReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a ResourceReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.ResourceChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as ResourceReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a ResourceReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<ResourceReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<ResourceReadableModel>, Task<IEnumerable<ResourceReadableModel>>> asyncMethod);

            partial void PostCreateEntity(ResourceDataViewModel dataViewModel);

            partial void PostCopyEntity(ResourceDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<ResourceDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(ResourceDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<ResourceDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<ResourceDataViewModel, ResourceReadableModel, Task<ResourceReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref ResourceQuery query);

            private void RunFilter(ref Func<ResourceReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<ResourceReadableModel>, Task<IEnumerable<ResourceReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<ResourceDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<ResourceDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<ResourceDataViewModel, ResourceReadableModel, Task<ResourceReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<ResourceReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyResources == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyResources =
                        new ReadOnlyEntityCollection<ResourceReadOnlyDataViewModel>(this.allResources);
                }
                else
                {
                    this.allResources.Clear();
                }

                this.readOnlyResources.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            ResourceQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.ResourceChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allResources.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyResources.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(ResourceDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new ResourceValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(ResourceDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.UploadingUser.Entities = this.DataController.User.All;
                dataViewModel.UploadingUser.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.UploadingUser.Entities = null;
                await this.DataController.User.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<ResourceReadOnlyDataViewModel> CreateReadOnlyAsync(ResourceReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<ResourceReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyResources == null || this.readOnlyResources.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allResources.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Resource", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<ResourceReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyResources == null || this.readOnlyResources.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allResources.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allResources.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Resource", ex);
                }
            }
        }

    }

    namespace Update
    {
        using Gorba.Center.Admin.Core.DataViewModels.Update;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Update;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
        using Gorba.Center.Common.ServiceModel.Filters.Update;


        public partial class UpdateGroupDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UpdateGroupReadOnlyDataViewModel> allUpdateGroups =
                new ObservableCollection<UpdateGroupReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UpdateGroupReadOnlyDataViewModel> readOnlyUpdateGroups;

            private TaskCompletionSource<bool> loadAllCompletion;

            private TaskCompletionSource<IUpdateGroupUdpContext> udpContextCompletion;

            public UpdateGroupDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UpdateGroupReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUpdateGroups != null)
                    {
                        return this.readOnlyUpdateGroups;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUpdateGroups != null)
                        {
                            return this.readOnlyUpdateGroups;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUpdateGroups;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUpdateGroupChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UpdateGroupChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UpdateGroupChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateGroupReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UpdateGroupChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UpdateGroupChangeTrackingManager.Create();
                var udpContext = await this.GetUdpContextAsync();
                var dvm = this.Factory.Create(model, udpContext);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateGroupReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UpdateGroupReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateGroupReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UpdateGroupReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UpdateGroupChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UpdateGroupDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UpdateGroupDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UpdateGroupChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateGroupReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UpdateGroupReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UpdateGroupChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateGroupReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UpdateGroupReadOnlyDataViewModel");
                }

                readOnly.Units.IsLoading = true;
                readOnly.UpdateParts.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Units)
                    {
                        readOnly.Units.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Units.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Units.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (Gorba.Center.Common.ServiceModel.ChangeTracking.Units.UnitReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Units.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Units.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Units.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Units.IsLoading = false;
                }

                try
                {
                    foreach (var reference in readOnly.ReadableModel.UpdateParts)
                    {
                        readOnly.UpdateParts.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.UpdateParts.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.UpdateParts.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UpdatePartReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.UpdateParts.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.UpdateParts.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.UpdateParts.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.UpdateParts.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UpdateGroupReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UpdateGroupReadableModel>, Task<IEnumerable<UpdateGroupReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UpdateGroupDataViewModel dataViewModel);

            partial void PostCopyEntity(UpdateGroupDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UpdateGroupDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UpdateGroupDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UpdateGroupDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UpdateGroupDataViewModel, UpdateGroupReadableModel, Task<UpdateGroupReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UpdateGroupQuery query);

            private void RunFilter(ref Func<UpdateGroupReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UpdateGroupReadableModel>, Task<IEnumerable<UpdateGroupReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UpdateGroupDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UpdateGroupDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UpdateGroupDataViewModel, UpdateGroupReadableModel, Task<UpdateGroupReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UpdateGroupReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                this.udpContextCompletion = null;
                if (this.readOnlyUpdateGroups == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUpdateGroups =
                        new ReadOnlyEntityCollection<UpdateGroupReadOnlyDataViewModel>(this.allUpdateGroups);
                }
                else
                {
                    this.allUpdateGroups.Clear();
                }

                this.readOnlyUpdateGroups.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UpdateGroupQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UpdateGroupChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUpdateGroups.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUpdateGroups.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UpdateGroupDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UpdateGroupValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UpdateGroupDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Tenant.Entities = this.DataController.Tenant.All;
                dataViewModel.Tenant.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Tenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();

                dataViewModel.UnitConfiguration.Entities = this.DataController.UnitConfiguration.All;
                dataViewModel.UnitConfiguration.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.UnitConfiguration.Entities = null;
                await this.DataController.UnitConfiguration.AwaitAllDataAsync();

                dataViewModel.MediaConfiguration.Entities = this.DataController.MediaConfiguration.All;
                dataViewModel.MediaConfiguration.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.MediaConfiguration.Entities = null;
                await this.DataController.MediaConfiguration.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UpdateGroupReadOnlyDataViewModel> CreateReadOnlyAsync(UpdateGroupReadableModel model)
            {
                var udpContext = await this.GetUdpContextAsync();
                return this.Factory.CreateReadOnly(model, udpContext);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateGroups == null || this.readOnlyUpdateGroups.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUpdateGroups.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UpdateGroup", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UpdateGroupReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateGroups == null || this.readOnlyUpdateGroups.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUpdateGroups.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUpdateGroups.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UpdateGroup", ex);
                }
            }

            private async Task<IUpdateGroupUdpContext> GetUdpContextAsync()
            {
                if (this.udpContextCompletion != null)
                {
                    return await this.udpContextCompletion.Task;
                }

                var completion = this.udpContextCompletion = new TaskCompletionSource<IUpdateGroupUdpContext>();

                await this.DataController.UserDefinedProperty.AwaitAllDataAsync();
                var context =
                    new UpdateGroupUdpContext(
                        this.DataController.UserDefinedProperty.All.Where(
                            u =>
                            u.OwnerEntity == Gorba.Center.Common.ServiceModel.Meta.UserDefinedPropertyEnabledEntity.UpdateGroup
                            && (u.Tenant == null || u.Tenant.Id == this.ApplicationState.CurrentTenant.Id))
                            .Select(u => u.ReadableModel.Name));

                completion.TrySetResult(context);
                return context;
            }

            private class UpdateGroupUdpContext : IUpdateGroupUdpContext
            {
                private List<string> udpNames;

                public UpdateGroupUdpContext(IEnumerable<string> udpNames)
                {
                    this.udpNames = udpNames.ToList();
                }

                public IEnumerable<string> GetAdditionalUpdateGroupProperties()
                {
                    return this.udpNames;
                }
            }
        }


        public partial class UpdatePartDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UpdatePartReadOnlyDataViewModel> allUpdateParts =
                new ObservableCollection<UpdatePartReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UpdatePartReadOnlyDataViewModel> readOnlyUpdateParts;

            private TaskCompletionSource<bool> loadAllCompletion;

            public UpdatePartDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UpdatePartReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUpdateParts != null)
                    {
                        return this.readOnlyUpdateParts;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUpdateParts != null)
                        {
                            return this.readOnlyUpdateParts;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUpdateParts;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUpdatePartChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UpdatePartChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UpdatePartChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdatePartReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UpdatePartChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UpdatePartChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdatePartReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UpdatePartReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdatePartReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UpdatePartReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UpdatePartChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UpdatePartDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UpdatePartDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UpdatePartChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdatePartReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UpdatePartReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UpdatePartChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdatePartReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UpdatePartReadOnlyDataViewModel");
                }

                readOnly.RelatedCommands.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.RelatedCommands)
                    {
                        readOnly.RelatedCommands.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.RelatedCommands.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.RelatedCommands.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UpdateCommandReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.RelatedCommands.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.RelatedCommands.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.RelatedCommands.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.RelatedCommands.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UpdatePartReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UpdatePartReadableModel>, Task<IEnumerable<UpdatePartReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UpdatePartDataViewModel dataViewModel);

            partial void PostCopyEntity(UpdatePartDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UpdatePartDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UpdatePartDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UpdatePartDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UpdatePartDataViewModel, UpdatePartReadableModel, Task<UpdatePartReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UpdatePartQuery query);

            private void RunFilter(ref Func<UpdatePartReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UpdatePartReadableModel>, Task<IEnumerable<UpdatePartReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UpdatePartDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UpdatePartDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UpdatePartDataViewModel, UpdatePartReadableModel, Task<UpdatePartReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UpdatePartReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyUpdateParts == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUpdateParts =
                        new ReadOnlyEntityCollection<UpdatePartReadOnlyDataViewModel>(this.allUpdateParts);
                }
                else
                {
                    this.allUpdateParts.Clear();
                }

                this.readOnlyUpdateParts.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UpdatePartQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UpdatePartChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUpdateParts.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUpdateParts.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UpdatePartDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UpdatePartValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UpdatePartDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.UpdateGroup.Entities = this.DataController.UpdateGroup.All;
                dataViewModel.UpdateGroup.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.UpdateGroup.Entities = null;
                await this.DataController.UpdateGroup.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UpdatePartReadOnlyDataViewModel> CreateReadOnlyAsync(UpdatePartReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UpdatePartReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateParts == null || this.readOnlyUpdateParts.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUpdateParts.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UpdatePart", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UpdatePartReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateParts == null || this.readOnlyUpdateParts.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUpdateParts.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUpdateParts.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UpdatePart", ex);
                }
            }
        }


        public partial class UpdateCommandDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UpdateCommandReadOnlyDataViewModel> allUpdateCommands =
                new ObservableCollection<UpdateCommandReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UpdateCommandReadOnlyDataViewModel> readOnlyUpdateCommands;

            private TaskCompletionSource<bool> loadAllCompletion;

            public UpdateCommandDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UpdateCommandReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUpdateCommands != null)
                    {
                        return this.readOnlyUpdateCommands;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUpdateCommands != null)
                        {
                            return this.readOnlyUpdateCommands;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUpdateCommands;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUpdateCommandChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UpdateCommandChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UpdateCommandChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateCommandReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UpdateCommandChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UpdateCommandChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateCommandReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UpdateCommandReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateCommandReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UpdateCommandReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UpdateCommandChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UpdateCommandDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UpdateCommandDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UpdateCommandChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateCommandReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UpdateCommandReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UpdateCommandChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateCommandReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UpdateCommandReadOnlyDataViewModel");
                }

                readOnly.IncludedParts.IsLoading = true;
                readOnly.Feedbacks.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.IncludedParts)
                    {
                        readOnly.IncludedParts.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.IncludedParts.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.IncludedParts.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UpdatePartReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.IncludedParts.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.IncludedParts.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.IncludedParts.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.IncludedParts.IsLoading = false;
                }

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Feedbacks)
                    {
                        readOnly.Feedbacks.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Feedbacks.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Feedbacks.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (UpdateFeedbackReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Feedbacks.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Feedbacks.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Feedbacks.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Feedbacks.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UpdateCommandReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UpdateCommandReadableModel>, Task<IEnumerable<UpdateCommandReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UpdateCommandDataViewModel dataViewModel);

            partial void PostCopyEntity(UpdateCommandDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UpdateCommandDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UpdateCommandDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UpdateCommandDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UpdateCommandDataViewModel, UpdateCommandReadableModel, Task<UpdateCommandReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UpdateCommandQuery query);

            private void RunFilter(ref Func<UpdateCommandReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UpdateCommandReadableModel>, Task<IEnumerable<UpdateCommandReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UpdateCommandDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UpdateCommandDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UpdateCommandDataViewModel, UpdateCommandReadableModel, Task<UpdateCommandReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UpdateCommandReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyUpdateCommands == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUpdateCommands =
                        new ReadOnlyEntityCollection<UpdateCommandReadOnlyDataViewModel>(this.allUpdateCommands);
                }
                else
                {
                    this.allUpdateCommands.Clear();
                }

                this.readOnlyUpdateCommands.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UpdateCommandQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UpdateCommandChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUpdateCommands.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUpdateCommands.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UpdateCommandDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UpdateCommandValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UpdateCommandDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Unit.Entities = this.DataController.Unit.All;
                dataViewModel.Unit.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Unit.Entities = null;
                await this.DataController.Unit.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UpdateCommandReadOnlyDataViewModel> CreateReadOnlyAsync(UpdateCommandReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UpdateCommandReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateCommands == null || this.readOnlyUpdateCommands.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUpdateCommands.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UpdateCommand", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UpdateCommandReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateCommands == null || this.readOnlyUpdateCommands.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUpdateCommands.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUpdateCommands.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UpdateCommand", ex);
                }
            }
        }


        public partial class UpdateFeedbackDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UpdateFeedbackReadOnlyDataViewModel> allUpdateFeedbacks =
                new ObservableCollection<UpdateFeedbackReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UpdateFeedbackReadOnlyDataViewModel> readOnlyUpdateFeedbacks;

            private TaskCompletionSource<bool> loadAllCompletion;

            public UpdateFeedbackDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UpdateFeedbackReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUpdateFeedbacks != null)
                    {
                        return this.readOnlyUpdateFeedbacks;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUpdateFeedbacks != null)
                        {
                            return this.readOnlyUpdateFeedbacks;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUpdateFeedbacks;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUpdateFeedbackChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UpdateFeedbackChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UpdateFeedbackChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UpdateFeedbackReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UpdateFeedbackChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UpdateFeedbackChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateFeedbackReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UpdateFeedbackReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateFeedbackReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UpdateFeedbackReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UpdateFeedbackChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UpdateFeedbackDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UpdateFeedbackDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UpdateFeedbackChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateFeedbackReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UpdateFeedbackReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UpdateFeedbackChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UpdateFeedbackReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UpdateFeedbackReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<UpdateFeedbackReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UpdateFeedbackReadableModel>, Task<IEnumerable<UpdateFeedbackReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UpdateFeedbackDataViewModel dataViewModel);

            partial void PostCopyEntity(UpdateFeedbackDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UpdateFeedbackDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UpdateFeedbackDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UpdateFeedbackDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UpdateFeedbackDataViewModel, UpdateFeedbackReadableModel, Task<UpdateFeedbackReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UpdateFeedbackQuery query);

            private void RunFilter(ref Func<UpdateFeedbackReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UpdateFeedbackReadableModel>, Task<IEnumerable<UpdateFeedbackReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UpdateFeedbackDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UpdateFeedbackDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UpdateFeedbackDataViewModel, UpdateFeedbackReadableModel, Task<UpdateFeedbackReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UpdateFeedbackReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyUpdateFeedbacks == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUpdateFeedbacks =
                        new ReadOnlyEntityCollection<UpdateFeedbackReadOnlyDataViewModel>(this.allUpdateFeedbacks);
                }
                else
                {
                    this.allUpdateFeedbacks.Clear();
                }

                this.readOnlyUpdateFeedbacks.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UpdateFeedbackQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UpdateFeedbackChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUpdateFeedbacks.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUpdateFeedbacks.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UpdateFeedbackDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UpdateFeedbackValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UpdateFeedbackDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.UpdateCommand.Entities = this.DataController.UpdateCommand.All;
                dataViewModel.UpdateCommand.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.UpdateCommand.Entities = null;
                await this.DataController.UpdateCommand.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UpdateFeedbackReadOnlyDataViewModel> CreateReadOnlyAsync(UpdateFeedbackReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateFeedbacks == null || this.readOnlyUpdateFeedbacks.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUpdateFeedbacks.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UpdateFeedback", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UpdateFeedbackReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUpdateFeedbacks == null || this.readOnlyUpdateFeedbacks.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUpdateFeedbacks.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUpdateFeedbacks.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UpdateFeedback", ex);
                }
            }
        }

    }

    namespace Documents
    {
        using Gorba.Center.Admin.Core.DataViewModels.Documents;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Documents;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
        using Gorba.Center.Common.ServiceModel.Filters.Documents;


        public partial class DocumentDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<DocumentReadOnlyDataViewModel> allDocuments =
                new ObservableCollection<DocumentReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<DocumentReadOnlyDataViewModel> readOnlyDocuments;

            private TaskCompletionSource<bool> loadAllCompletion;

            public DocumentDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<DocumentReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyDocuments != null)
                    {
                        return this.readOnlyDocuments;
                    }

                    lock (this)
                    {
                        if (this.readOnlyDocuments != null)
                        {
                            return this.readOnlyDocuments;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyDocuments;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IDocumentChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.DocumentChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.DocumentChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.DocumentChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.DocumentChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a DocumentReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a DocumentReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.DocumentChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as DocumentDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a DocumentDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.DocumentChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a DocumentReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.DocumentChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a DocumentReadOnlyDataViewModel");
                }

                readOnly.Versions.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Versions)
                    {
                        readOnly.Versions.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Versions.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Versions.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (DocumentVersionReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Versions.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Versions.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Versions.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Versions.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<DocumentReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<DocumentReadableModel>, Task<IEnumerable<DocumentReadableModel>>> asyncMethod);

            partial void PostCreateEntity(DocumentDataViewModel dataViewModel);

            partial void PostCopyEntity(DocumentDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<DocumentDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(DocumentDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<DocumentDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<DocumentDataViewModel, DocumentReadableModel, Task<DocumentReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref DocumentQuery query);

            private void RunFilter(ref Func<DocumentReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<DocumentReadableModel>, Task<IEnumerable<DocumentReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<DocumentDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<DocumentDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<DocumentDataViewModel, DocumentReadableModel, Task<DocumentReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<DocumentReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyDocuments == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyDocuments =
                        new ReadOnlyEntityCollection<DocumentReadOnlyDataViewModel>(this.allDocuments);
                }
                else
                {
                    this.allDocuments.Clear();
                }

                this.readOnlyDocuments.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            DocumentQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.DocumentChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allDocuments.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyDocuments.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(DocumentDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new DocumentValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(DocumentDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Tenant.Entities = this.DataController.Tenant.All;
                dataViewModel.Tenant.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Tenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<DocumentReadOnlyDataViewModel> CreateReadOnlyAsync(DocumentReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<DocumentReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyDocuments == null || this.readOnlyDocuments.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allDocuments.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Document", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<DocumentReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyDocuments == null || this.readOnlyDocuments.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allDocuments.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allDocuments.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Document", ex);
                }
            }
        }


        public partial class DocumentVersionDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<DocumentVersionReadOnlyDataViewModel> allDocumentVersions =
                new ObservableCollection<DocumentVersionReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<DocumentVersionReadOnlyDataViewModel> readOnlyDocumentVersions;

            private TaskCompletionSource<bool> loadAllCompletion;

            public DocumentVersionDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<DocumentVersionReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyDocumentVersions != null)
                    {
                        return this.readOnlyDocumentVersions;
                    }

                    lock (this)
                    {
                        if (this.readOnlyDocumentVersions != null)
                        {
                            return this.readOnlyDocumentVersions;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyDocumentVersions;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IDocumentVersionChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.DocumentVersionChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.DocumentVersionChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is DocumentVersionReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.DocumentVersionChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.DocumentVersionChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a DocumentVersionReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a DocumentVersionReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.DocumentVersionChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as DocumentVersionDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a DocumentVersionDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.DocumentVersionChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a DocumentVersionReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.DocumentVersionChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as DocumentVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a DocumentVersionReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<DocumentVersionReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<DocumentVersionReadableModel>, Task<IEnumerable<DocumentVersionReadableModel>>> asyncMethod);

            partial void PostCreateEntity(DocumentVersionDataViewModel dataViewModel);

            partial void PostCopyEntity(DocumentVersionDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<DocumentVersionDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(DocumentVersionDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<DocumentVersionDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<DocumentVersionDataViewModel, DocumentVersionReadableModel, Task<DocumentVersionReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref DocumentVersionQuery query);

            private void RunFilter(ref Func<DocumentVersionReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<DocumentVersionReadableModel>, Task<IEnumerable<DocumentVersionReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<DocumentVersionDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<DocumentVersionDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<DocumentVersionDataViewModel, DocumentVersionReadableModel, Task<DocumentVersionReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<DocumentVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyDocumentVersions == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyDocumentVersions =
                        new ReadOnlyEntityCollection<DocumentVersionReadOnlyDataViewModel>(this.allDocumentVersions);
                }
                else
                {
                    this.allDocumentVersions.Clear();
                }

                this.readOnlyDocumentVersions.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            DocumentVersionQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.DocumentVersionChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allDocumentVersions.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyDocumentVersions.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(DocumentVersionDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new DocumentVersionValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(DocumentVersionDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Document.Entities = this.DataController.Document.All;
                dataViewModel.Document.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Document.Entities = null;
                await this.DataController.Document.AwaitAllDataAsync();

                dataViewModel.CreatingUser.Entities = this.DataController.User.All;
                dataViewModel.CreatingUser.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.CreatingUser.Entities = null;
                await this.DataController.User.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<DocumentVersionReadOnlyDataViewModel> CreateReadOnlyAsync(DocumentVersionReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<DocumentVersionReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyDocumentVersions == null || this.readOnlyDocumentVersions.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allDocumentVersions.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added DocumentVersion", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<DocumentVersionReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyDocumentVersions == null || this.readOnlyDocumentVersions.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allDocumentVersions.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allDocumentVersions.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove DocumentVersion", ex);
                }
            }
        }

    }

    namespace Software
    {
        using Gorba.Center.Admin.Core.DataViewModels.Software;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Software;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
        using Gorba.Center.Common.ServiceModel.Filters.Software;


        public partial class PackageDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<PackageReadOnlyDataViewModel> allPackages =
                new ObservableCollection<PackageReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<PackageReadOnlyDataViewModel> readOnlyPackages;

            private TaskCompletionSource<bool> loadAllCompletion;

            public PackageDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<PackageReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyPackages != null)
                    {
                        return this.readOnlyPackages;
                    }

                    lock (this)
                    {
                        if (this.readOnlyPackages != null)
                        {
                            return this.readOnlyPackages;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyPackages;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IPackageChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.PackageChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.PackageChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.PackageChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.PackageChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a PackageReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a PackageReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.PackageChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as PackageDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a PackageDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.PackageChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a PackageReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.PackageChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a PackageReadOnlyDataViewModel");
                }

                readOnly.Versions.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.Versions)
                    {
                        readOnly.Versions.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.Versions.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.Versions.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (PackageVersionReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.Versions.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.Versions.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.Versions.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.Versions.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<PackageReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<PackageReadableModel>, Task<IEnumerable<PackageReadableModel>>> asyncMethod);

            partial void PostCreateEntity(PackageDataViewModel dataViewModel);

            partial void PostCopyEntity(PackageDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<PackageDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(PackageDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<PackageDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<PackageDataViewModel, PackageReadableModel, Task<PackageReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref PackageQuery query);

            private void RunFilter(ref Func<PackageReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<PackageReadableModel>, Task<IEnumerable<PackageReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<PackageDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<PackageDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<PackageDataViewModel, PackageReadableModel, Task<PackageReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<PackageReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyPackages == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyPackages =
                        new ReadOnlyEntityCollection<PackageReadOnlyDataViewModel>(this.allPackages);
                }
                else
                {
                    this.allPackages.Clear();
                }

                this.readOnlyPackages.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            PackageQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.PackageChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allPackages.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyPackages.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(PackageDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new PackageValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(PackageDataViewModel dataViewModel)
            {
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<PackageReadOnlyDataViewModel> CreateReadOnlyAsync(PackageReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<PackageReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyPackages == null || this.readOnlyPackages.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allPackages.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added Package", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<PackageReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyPackages == null || this.readOnlyPackages.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allPackages.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allPackages.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove Package", ex);
                }
            }
        }


        public partial class PackageVersionDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<PackageVersionReadOnlyDataViewModel> allPackageVersions =
                new ObservableCollection<PackageVersionReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<PackageVersionReadOnlyDataViewModel> readOnlyPackageVersions;

            private TaskCompletionSource<bool> loadAllCompletion;

            public PackageVersionDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<PackageVersionReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyPackageVersions != null)
                    {
                        return this.readOnlyPackageVersions;
                    }

                    lock (this)
                    {
                        if (this.readOnlyPackageVersions != null)
                        {
                            return this.readOnlyPackageVersions;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyPackageVersions;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IPackageVersionChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.PackageVersionChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.PackageVersionChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is PackageVersionReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.PackageVersionChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.PackageVersionChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a PackageVersionReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a PackageVersionReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.PackageVersionChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as PackageVersionDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a PackageVersionDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.PackageVersionChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a PackageVersionReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.PackageVersionChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as PackageVersionReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a PackageVersionReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<PackageVersionReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<PackageVersionReadableModel>, Task<IEnumerable<PackageVersionReadableModel>>> asyncMethod);

            partial void PostCreateEntity(PackageVersionDataViewModel dataViewModel);

            partial void PostCopyEntity(PackageVersionDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<PackageVersionDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(PackageVersionDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<PackageVersionDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<PackageVersionDataViewModel, PackageVersionReadableModel, Task<PackageVersionReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref PackageVersionQuery query);

            private void RunFilter(ref Func<PackageVersionReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<PackageVersionReadableModel>, Task<IEnumerable<PackageVersionReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<PackageVersionDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<PackageVersionDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<PackageVersionDataViewModel, PackageVersionReadableModel, Task<PackageVersionReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<PackageVersionReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyPackageVersions == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyPackageVersions =
                        new ReadOnlyEntityCollection<PackageVersionReadOnlyDataViewModel>(this.allPackageVersions);
                }
                else
                {
                    this.allPackageVersions.Clear();
                }

                this.readOnlyPackageVersions.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            PackageVersionQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.PackageVersionChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allPackageVersions.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyPackageVersions.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(PackageVersionDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new PackageVersionValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(PackageVersionDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Package.Entities = this.DataController.Package.All;
                dataViewModel.Package.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Package.Entities = null;
                await this.DataController.Package.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<PackageVersionReadOnlyDataViewModel> CreateReadOnlyAsync(PackageVersionReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<PackageVersionReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyPackageVersions == null || this.readOnlyPackageVersions.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allPackageVersions.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added PackageVersion", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<PackageVersionReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyPackageVersions == null || this.readOnlyPackageVersions.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allPackageVersions.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allPackageVersions.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove PackageVersion", ex);
                }
            }
        }

    }

    namespace Configurations
    {
        using Gorba.Center.Admin.Core.DataViewModels.Configurations;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Configurations;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Configurations;
        using Gorba.Center.Common.ServiceModel.Filters.Configurations;


        public partial class UnitConfigurationDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UnitConfigurationReadOnlyDataViewModel> allUnitConfigurations =
                new ObservableCollection<UnitConfigurationReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UnitConfigurationReadOnlyDataViewModel> readOnlyUnitConfigurations;

            private TaskCompletionSource<bool> loadAllCompletion;

            public UnitConfigurationDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UnitConfigurationReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUnitConfigurations != null)
                    {
                        return this.readOnlyUnitConfigurations;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUnitConfigurations != null)
                        {
                            return this.readOnlyUnitConfigurations;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUnitConfigurations;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUnitConfigurationChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UnitConfigurationChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UnitConfigurationChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UnitConfigurationReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UnitConfigurationChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UnitConfigurationChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UnitConfigurationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UnitConfigurationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UnitConfigurationChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UnitConfigurationDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UnitConfigurationDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UnitConfigurationChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UnitConfigurationReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UnitConfigurationChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UnitConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UnitConfigurationReadOnlyDataViewModel");
                }

                readOnly.UpdateGroups.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.UpdateGroups)
                    {
                        readOnly.UpdateGroups.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.UpdateGroups.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.UpdateGroups.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateGroupReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.UpdateGroups.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.UpdateGroups.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.UpdateGroups.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.UpdateGroups.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<UnitConfigurationReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UnitConfigurationReadableModel>, Task<IEnumerable<UnitConfigurationReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UnitConfigurationDataViewModel dataViewModel);

            partial void PostCopyEntity(UnitConfigurationDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UnitConfigurationDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UnitConfigurationDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UnitConfigurationDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UnitConfigurationDataViewModel, UnitConfigurationReadableModel, Task<UnitConfigurationReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UnitConfigurationQuery query);

            private void RunFilter(ref Func<UnitConfigurationReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UnitConfigurationReadableModel>, Task<IEnumerable<UnitConfigurationReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UnitConfigurationDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UnitConfigurationDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UnitConfigurationDataViewModel, UnitConfigurationReadableModel, Task<UnitConfigurationReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UnitConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyUnitConfigurations == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUnitConfigurations =
                        new ReadOnlyEntityCollection<UnitConfigurationReadOnlyDataViewModel>(this.allUnitConfigurations);
                }
                else
                {
                    this.allUnitConfigurations.Clear();
                }

                this.readOnlyUnitConfigurations.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UnitConfigurationQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UnitConfigurationChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUnitConfigurations.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUnitConfigurations.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UnitConfigurationDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UnitConfigurationValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UnitConfigurationDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Document.Entities = this.DataController.Document.All;
                dataViewModel.Document.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Document.Entities = null;
                await this.DataController.Document.AwaitAllDataAsync();

                dataViewModel.ProductType.Entities = this.DataController.ProductType.All;
                dataViewModel.ProductType.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.ProductType.Entities = null;
                await this.DataController.ProductType.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UnitConfigurationReadOnlyDataViewModel> CreateReadOnlyAsync(UnitConfigurationReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UnitConfigurationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUnitConfigurations == null || this.readOnlyUnitConfigurations.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUnitConfigurations.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UnitConfiguration", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UnitConfigurationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUnitConfigurations == null || this.readOnlyUnitConfigurations.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUnitConfigurations.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUnitConfigurations.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UnitConfiguration", ex);
                }
            }
        }


        public partial class MediaConfigurationDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<MediaConfigurationReadOnlyDataViewModel> allMediaConfigurations =
                new ObservableCollection<MediaConfigurationReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<MediaConfigurationReadOnlyDataViewModel> readOnlyMediaConfigurations;

            private TaskCompletionSource<bool> loadAllCompletion;

            public MediaConfigurationDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<MediaConfigurationReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyMediaConfigurations != null)
                    {
                        return this.readOnlyMediaConfigurations;
                    }

                    lock (this)
                    {
                        if (this.readOnlyMediaConfigurations != null)
                        {
                            return this.readOnlyMediaConfigurations;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyMediaConfigurations;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IMediaConfigurationChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.MediaConfigurationChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.MediaConfigurationChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is MediaConfigurationReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.MediaConfigurationChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.MediaConfigurationChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as MediaConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a MediaConfigurationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as MediaConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a MediaConfigurationReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.MediaConfigurationChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as MediaConfigurationDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a MediaConfigurationDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.MediaConfigurationChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as MediaConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a MediaConfigurationReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.MediaConfigurationChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as MediaConfigurationReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a MediaConfigurationReadOnlyDataViewModel");
                }

                readOnly.UpdateGroups.IsLoading = true;

                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);

                try
                {
                    foreach (var reference in readOnly.ReadableModel.UpdateGroups)
                    {
                        readOnly.UpdateGroups.Items.Add(this.Factory.CreateReadOnly(reference));
                    }
                }
                finally
                {
                    readOnly.ReadableModel.UpdateGroups.CollectionChanged += (s, e) =>
                    {
                        this.StartNew(
                            () =>
                            {
                                switch (e.Action)
                                {
                                    case NotifyCollectionChangedAction.Add:
                                        readOnly.UpdateGroups.Items.Add(
                                                this.Factory.CreateReadOnly(
                                                    (Gorba.Center.Common.ServiceModel.ChangeTracking.Update.UpdateGroupReadableModel)e.NewItems[0]));
                                        break;
                                    case NotifyCollectionChangedAction.Remove:
                                        var remove =
                                                readOnly.UpdateGroups.Items.FirstOrDefault(
                                                    i => i.ReadableModel.Equals(e.OldItems[0]));
                                        readOnly.UpdateGroups.Items.Remove(remove);
                                        break;
                                    case NotifyCollectionChangedAction.Reset:
                                        readOnly.UpdateGroups.Items.Clear();
                                        break;
                                }
                            });
                    };
                    readOnly.UpdateGroups.IsLoading = false;
                }
            }

            partial void Initialize();

            partial void Filter(ref Func<MediaConfigurationReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<MediaConfigurationReadableModel>, Task<IEnumerable<MediaConfigurationReadableModel>>> asyncMethod);

            partial void PostCreateEntity(MediaConfigurationDataViewModel dataViewModel);

            partial void PostCopyEntity(MediaConfigurationDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<MediaConfigurationDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(MediaConfigurationDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<MediaConfigurationDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<MediaConfigurationDataViewModel, MediaConfigurationReadableModel, Task<MediaConfigurationReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref MediaConfigurationQuery query);

            private void RunFilter(ref Func<MediaConfigurationReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<MediaConfigurationReadableModel>, Task<IEnumerable<MediaConfigurationReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<MediaConfigurationDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<MediaConfigurationDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<MediaConfigurationDataViewModel, MediaConfigurationReadableModel, Task<MediaConfigurationReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<MediaConfigurationReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyMediaConfigurations == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyMediaConfigurations =
                        new ReadOnlyEntityCollection<MediaConfigurationReadOnlyDataViewModel>(this.allMediaConfigurations);
                }
                else
                {
                    this.allMediaConfigurations.Clear();
                }

                this.readOnlyMediaConfigurations.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            MediaConfigurationQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.MediaConfigurationChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allMediaConfigurations.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyMediaConfigurations.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(MediaConfigurationDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new MediaConfigurationValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(MediaConfigurationDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Document.Entities = this.DataController.Document.All;
                dataViewModel.Document.IsRequired = true;
                dataViewModel.Disposing += (s, e) => dataViewModel.Document.Entities = null;
                await this.DataController.Document.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<MediaConfigurationReadOnlyDataViewModel> CreateReadOnlyAsync(MediaConfigurationReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<MediaConfigurationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyMediaConfigurations == null || this.readOnlyMediaConfigurations.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allMediaConfigurations.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added MediaConfiguration", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<MediaConfigurationReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyMediaConfigurations == null || this.readOnlyMediaConfigurations.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allMediaConfigurations.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allMediaConfigurations.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove MediaConfiguration", ex);
                }
            }
        }

    }

    namespace Log
    {
        using Gorba.Center.Admin.Core.DataViewModels.Log;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Log;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Log;
        using Gorba.Center.Common.ServiceModel.Filters.Log;

    }

    namespace Meta
    {
        using Gorba.Center.Admin.Core.DataViewModels.Meta;
        using Gorba.Center.Admin.Core.ViewModels.Stages.Meta;
        using Gorba.Center.Common.ServiceModel.ChangeTracking.Meta;
        using Gorba.Center.Common.ServiceModel.Filters.Meta;


        public partial class UserDefinedPropertyDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<UserDefinedPropertyReadOnlyDataViewModel> allUserDefinedProperties =
                new ObservableCollection<UserDefinedPropertyReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<UserDefinedPropertyReadOnlyDataViewModel> readOnlyUserDefinedProperties;

            private TaskCompletionSource<bool> loadAllCompletion;

            public UserDefinedPropertyDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<UserDefinedPropertyReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlyUserDefinedProperties != null)
                    {
                        return this.readOnlyUserDefinedProperties;
                    }

                    lock (this)
                    {
                        if (this.readOnlyUserDefinedProperties != null)
                        {
                            return this.readOnlyUserDefinedProperties;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlyUserDefinedProperties;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                IUserDefinedPropertyChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.UserDefinedPropertyChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.UserDefinedPropertyChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is UserDefinedPropertyReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.UserDefinedPropertyChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.UserDefinedPropertyChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserDefinedPropertyReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a UserDefinedPropertyReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserDefinedPropertyReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a UserDefinedPropertyReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.UserDefinedPropertyChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as UserDefinedPropertyDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a UserDefinedPropertyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.UserDefinedPropertyChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserDefinedPropertyReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a UserDefinedPropertyReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.UserDefinedPropertyChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as UserDefinedPropertyReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a UserDefinedPropertyReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<UserDefinedPropertyReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<UserDefinedPropertyReadableModel>, Task<IEnumerable<UserDefinedPropertyReadableModel>>> asyncMethod);

            partial void PostCreateEntity(UserDefinedPropertyDataViewModel dataViewModel);

            partial void PostCopyEntity(UserDefinedPropertyDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<UserDefinedPropertyDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(UserDefinedPropertyDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<UserDefinedPropertyDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<UserDefinedPropertyDataViewModel, UserDefinedPropertyReadableModel, Task<UserDefinedPropertyReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref UserDefinedPropertyQuery query);

            private void RunFilter(ref Func<UserDefinedPropertyReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<UserDefinedPropertyReadableModel>, Task<IEnumerable<UserDefinedPropertyReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<UserDefinedPropertyDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<UserDefinedPropertyDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<UserDefinedPropertyDataViewModel, UserDefinedPropertyReadableModel, Task<UserDefinedPropertyReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<UserDefinedPropertyReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlyUserDefinedProperties == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlyUserDefinedProperties =
                        new ReadOnlyEntityCollection<UserDefinedPropertyReadOnlyDataViewModel>(this.allUserDefinedProperties);
                }
                else
                {
                    this.allUserDefinedProperties.Clear();
                }

                this.readOnlyUserDefinedProperties.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            UserDefinedPropertyQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.UserDefinedPropertyChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allUserDefinedProperties.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlyUserDefinedProperties.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(UserDefinedPropertyDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new UserDefinedPropertyValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(UserDefinedPropertyDataViewModel dataViewModel)
            {
                dataViewModel.IsLoading = true;

                dataViewModel.Tenant.Entities = this.DataController.Tenant.All;
                dataViewModel.Tenant.IsRequired = false;
                dataViewModel.Disposing += (s, e) => dataViewModel.Tenant.Entities = null;
                await this.DataController.Tenant.AwaitAllDataAsync();
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<UserDefinedPropertyReadOnlyDataViewModel> CreateReadOnlyAsync(UserDefinedPropertyReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUserDefinedProperties == null || this.readOnlyUserDefinedProperties.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allUserDefinedProperties.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added UserDefinedProperty", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<UserDefinedPropertyReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlyUserDefinedProperties == null || this.readOnlyUserDefinedProperties.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allUserDefinedProperties.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allUserDefinedProperties.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove UserDefinedProperty", ex);
                }
            }
        }


        public partial class SystemConfigDataController : DataControllerBase
        {
            private readonly AsyncLock asyncLocker = new AsyncLock();

            private readonly ObservableCollection<SystemConfigReadOnlyDataViewModel> allSystemConfigs =
                new ObservableCollection<SystemConfigReadOnlyDataViewModel>();
            private volatile ReadOnlyEntityCollection<SystemConfigReadOnlyDataViewModel> readOnlySystemConfigs;

            private TaskCompletionSource<bool> loadAllCompletion;

            public SystemConfigDataController(DataController dataController)
                : base(dataController)
            {
            }

            public IReadOnlyEntityCollection<SystemConfigReadOnlyDataViewModel> All
            {
                get
                {
                    if (this.readOnlySystemConfigs != null)
                    {
                        return this.readOnlySystemConfigs;
                    }

                    lock (this)
                    {
                        if (this.readOnlySystemConfigs != null)
                        {
                            return this.readOnlySystemConfigs;
                        }

                        this.StartReloadAllData(true);
                    }


                    return this.readOnlySystemConfigs;
                }
            }

            public override void Initialize(IConnectionController controller)
            {
                ISystemConfigChangeTrackingManager changeTrackingManager;
                if (this.ConnectionController != null)
                {
                    changeTrackingManager = this.ConnectionController.SystemConfigChangeTrackingManager;
                    changeTrackingManager.Added -= this.ChangeTrackingManagerOnAdded;
                    changeTrackingManager.Removed -= this.ChangeTrackingManagerOnRemoved;
                }

                base.Initialize(controller);

                changeTrackingManager = this.ConnectionController.SystemConfigChangeTrackingManager;
                changeTrackingManager.Added += this.ChangeTrackingManagerOnAdded;
                changeTrackingManager.Removed += this.ChangeTrackingManagerOnRemoved;

                this.Initialize();
            }

            public override Task AwaitAllDataAsync()
            {
                // the following line is needed to kick off the loading (if not already done)
                var all = this.All;
                return this.loadAllCompletion.Task;
            }

            public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
            {
                return dataViewModel is SystemConfigReadOnlyDataViewModel;
            }

            public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
            {
                var id = int.Parse(idString);
                var model = await this.ConnectionController.SystemConfigChangeTrackingManager.GetAsync(id);
                if (model == null)
                {
                    return null;
                }

                if (!(await PartialAsync.RunAsync(this.RunFilter, model, true)))
                {
                    return null;
                }

                return await this.CreateReadOnlyAsync(model);
            }

#pragma warning disable 1998
            public async override Task<DataViewModelBase> CreateEntityAsync()
            {
                var model = this.ConnectionController.SystemConfigChangeTrackingManager.Create();
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCreateEntity(dvm);
                return dvm;
            }
#pragma warning restore 1998

            public override async Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as SystemConfigReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("EditEntity() requires a SystemConfigReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var dvm = this.Factory.Create(readOnly);
                await this.SetupDataViewModelAsync(dvm);
                this.PostStartEditEntity(dvm);
                return dvm;
            }

            public override async Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as SystemConfigReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("CopyEntity() requires a SystemConfigReadOnlyDataViewModel");
                }

                await readOnly.ReadableModel.LoadXmlPropertiesAsync();
                var model = this.ConnectionController.SystemConfigChangeTrackingManager.CreateCopy(
                    readOnly.ReadableModel);
                var dvm = this.Factory.Create(model);
                await this.SetupDataViewModelAsync(dvm);
                this.PostCopyEntity(dvm);
                return dvm;
            }

            public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dvm)
            {
                var dataViewModel = dvm as SystemConfigDataViewModel;
                if (dataViewModel == null)
                {
                    throw new ArgumentException("SaveEntity() requires a SystemConfigDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreSaveEntity, dataViewModel);

                var readable = await this.ConnectionController.SystemConfigChangeTrackingManager.CommitAndVerifyAsync(
                    dataViewModel.Model);
                readable = await PartialAsync.RunAsync(this.RunPostSaveEntity, dataViewModel, readable, readable);
                await readable.LoadReferencePropertiesAsync();
                return await this.CreateReadOnlyAsync(readable);
            }

            public async override Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as SystemConfigReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DeleteEntity() requires a SystemConfigReadOnlyDataViewModel");
                }

                await PartialAsync.RunAsync(this.RunPreDeleteEntity, readOnly);
                await this.ConnectionController.SystemConfigChangeTrackingManager.DeleteAsync(readOnly.ReadableModel);
                await PartialAsync.RunAsync(this.RunPostDeleteEntity, readOnly);
            }

            protected async override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
            {
                var readOnly = dataViewModel as SystemConfigReadOnlyDataViewModel;
                if (readOnly == null)
                {
                    throw new ArgumentException("DoLoadEntityDetailsAsync() requires a SystemConfigReadOnlyDataViewModel");
                }


                await readOnly.ReadableModel.LoadNavigationPropertiesAsync();
                await PartialAsync.RunAsync(this.RunPrePopulateEntityDetails, readOnly);
            }

            partial void Initialize();

            partial void Filter(ref Func<SystemConfigReadableModel, Task<bool>> asyncMethod);

            partial void FilterResults(
                ref Func<IEnumerable<SystemConfigReadableModel>, Task<IEnumerable<SystemConfigReadableModel>>> asyncMethod);

            partial void PostCreateEntity(SystemConfigDataViewModel dataViewModel);

            partial void PostCopyEntity(SystemConfigDataViewModel dataViewModel);

            partial void PostSetupReferenceProperties(ref Func<SystemConfigDataViewModel, Task> asyncMethod);

            partial void PostStartEditEntity(SystemConfigDataViewModel dataViewModel);

            partial void PreSaveEntity(ref Func<SystemConfigDataViewModel, Task> asyncMethod);

            partial void PostSaveEntity(
                ref Func<SystemConfigDataViewModel, SystemConfigReadableModel, Task<SystemConfigReadableModel>> asyncMethod);

            partial void PreDeleteEntity(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod);

            partial void PostDeleteEntity(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod);

            partial void PrePopulateEntityDetails(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod);

            partial void CreateFilter(ref SystemConfigQuery query);

            private void RunFilter(ref Func<SystemConfigReadableModel, Task<bool>> asyncMethod)
            {
                this.Filter(ref asyncMethod);
            }

            private void RunFilterResults(
                ref Func<IEnumerable<SystemConfigReadableModel>, Task<IEnumerable<SystemConfigReadableModel>>> asyncMethod)
            {
                this.FilterResults(ref asyncMethod);
            }

            private void RunPostSetupReferenceProperties(ref Func<SystemConfigDataViewModel, Task> asyncMethod)
            {
                this.PostSetupReferenceProperties(ref asyncMethod);
            }

            private void RunPreSaveEntity(ref Func<SystemConfigDataViewModel, Task> asyncMethod)
            {
                this.PreSaveEntity(ref asyncMethod);
            }

            private void RunPostSaveEntity(
                ref Func<SystemConfigDataViewModel, SystemConfigReadableModel, Task<SystemConfigReadableModel>> asyncMethod)
            {
                this.PostSaveEntity(ref asyncMethod);
            }

            private void RunPreDeleteEntity(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PreDeleteEntity(ref asyncMethod);
            }

            private void RunPostDeleteEntity(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PostDeleteEntity(ref asyncMethod);
            }

            private void RunPrePopulateEntityDetails(ref Func<SystemConfigReadOnlyDataViewModel, Task> asyncMethod)
            {
                this.PrePopulateEntityDetails(ref asyncMethod);
            }

            protected async override void StartReloadAllData(bool force)
            {
                if (this.readOnlySystemConfigs == null)
                {
                    if (!force)
                    {
                        return;
                    }

                    this.readOnlySystemConfigs =
                        new ReadOnlyEntityCollection<SystemConfigReadOnlyDataViewModel>(this.allSystemConfigs);
                }
                else
                {
                    this.allSystemConfigs.Clear();
                }

                this.readOnlySystemConfigs.IsLoading = true;
                var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        try
                        {
                            SystemConfigQuery query = null;
                            this.CreateFilter(ref query);
                            var models = await this.ConnectionController.SystemConfigChangeTrackingManager.QueryAsync(query);
                            var filteredModels = await PartialAsync.RunAsync(this.RunFilterResults, models, models);
                            foreach (var model in filteredModels)
                            {
                                this.allSystemConfigs.Add(await this.CreateReadOnlyAsync(model));
                            }
                        }
                        finally
                        {
                            this.readOnlySystemConfigs.IsLoading = false;
                        }
                    }

                    completion.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorException("Couldn't load all entities", ex);
                    completion.TrySetException(ex);
                }
            }

            private async Task SetupDataViewModelAsync(SystemConfigDataViewModel dataViewModel)
            {
                await this.SetupReferencePropertiesAsync(dataViewModel);

                var validator = new SystemConfigValidator(dataViewModel, this.DataController);
                validator.Start();

                dataViewModel.Disposing += (s, e) => validator.Stop();
                dataViewModel.IsLoading = false;
            }

            private async Task SetupReferencePropertiesAsync(SystemConfigDataViewModel dataViewModel)
            {
                await PartialAsync.RunAsync(this.RunPostSetupReferenceProperties, dataViewModel);
            }

#pragma warning disable 1998
            private async Task<SystemConfigReadOnlyDataViewModel> CreateReadOnlyAsync(SystemConfigReadableModel model)
            {
                return this.Factory.CreateReadOnly(model);
            }
#pragma warning restore 1998

            private async void ChangeTrackingManagerOnAdded(
                object sender, ReadableModelEventArgs<SystemConfigReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlySystemConfigs == null || this.readOnlySystemConfigs.IsLoading)
                        {
                            return;
                        }
                    }

                    await e.Model.LoadReferencePropertiesAsync();
                    if (await PartialAsync.RunAsync(this.RunFilter, e.Model, true))
                    {
                        this.StartNew(async () => this.allSystemConfigs.Add(await this.CreateReadOnlyAsync(e.Model)));
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't load added SystemConfig", ex);
                }
            }

            private async void ChangeTrackingManagerOnRemoved(
                object sender, ReadableModelEventArgs<SystemConfigReadableModel> e)
            {
                try
                {
                    using (await this.asyncLocker.LockAsync())
                    {
                        if (this.readOnlySystemConfigs == null || this.readOnlySystemConfigs.IsLoading)
                        {
                            return;
                        }
                    }

                    this.StartNew(() =>
                    {
                        var dvm = this.allSystemConfigs.FirstOrDefault(m => m.Id.Equals(e.Model.Id));
                        if (dvm != null)
                        {
                            this.allSystemConfigs.Remove(dvm);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Couldn't remove SystemConfig", ex);
                }
            }
        }

    }
}