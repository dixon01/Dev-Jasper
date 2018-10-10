// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpServerDataController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FtpServerDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Common.Update.Ftp;

    /// <summary>
    /// The data controller for FTP servers from the <see cref="BackgroundSystemSettings"/>.
    /// </summary>
    public class FtpServerDataController : DataControllerBase
    {
        private readonly SystemConfigDataController systemConfigDataController;

        private readonly ObservableCollection<FtpServerReadOnlyDataViewModel> allFtpServers;

        private ReadOnlyEntityCollection<FtpServerReadOnlyDataViewModel> readOnlyList;

        private SystemConfigReadOnlyDataViewModel systemConfig;

        private TaskCompletionSource<bool> loadAllCompletion;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpServerDataController"/> class.
        /// </summary>
        /// <param name="dataController">
        /// The underlying system config data controller.
        /// </param>
        public FtpServerDataController(SystemConfigDataController dataController)
            : base(dataController.DataController)
        {
            this.systemConfigDataController = dataController;

            this.allFtpServers = new ObservableCollection<FtpServerReadOnlyDataViewModel>();
        }

        /// <summary>
        /// Gets all FTP server configurations in the system.
        /// </summary>
        public IReadOnlyEntityCollection<FtpServerReadOnlyDataViewModel> All
        {
            get
            {
                if (this.readOnlyList != null)
                {
                    return this.readOnlyList;
                }

                lock (this)
                {
                    if (this.readOnlyList != null)
                    {
                        return this.readOnlyList;
                    }

                    this.StartReloadAllData(true);
                }

                return this.readOnlyList;
            }
        }

        /// <summary>
        /// Asynchronous method to wait on this controller until it finished loading all data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override Task AwaitAllDataAsync()
        {
            // the following line is needed to kick off the loading (if not already done)
            // ReSharper disable once UnusedVariable
            var all = this.All;
            return this.loadAllCompletion.Task;
        }

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return dataViewModel is FtpServerReadOnlyDataViewModel;
        }

        /// <summary>
        /// Asynchronously gets the entity with the given id string.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the data view model with the given id.
        /// The returned view model can be null if no entity with the given id can be found.
        /// </returns>
        public async override Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString)
        {
            var index = int.Parse(idString) - 1;
            await this.AwaitAllDataAsync();
            return ((IReadOnlyList<FtpServerReadOnlyDataViewModel>)this.All)[index];
        }

        /// <summary>
        /// Creates a new <see cref="DataViewModelBase"/> object for the type of entity this controller handles.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        public override Task<DataViewModelBase> CreateEntityAsync()
        {
            return Task.FromResult(this.AddVerification(new FtpServerDataViewModel(null, this.Factory)));
        }

        /// <summary>
        /// Creates a <see cref="DataViewModelBase"/> for the given <paramref name="dataViewModel"/>.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="dataViewModel"/> is not of the type this controller handles.
        /// </exception>
        public override Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
        {
            var readOnly = (FtpServerReadOnlyDataViewModel)dataViewModel;
            return Task.FromResult(this.AddVerification(new FtpServerDataViewModel(readOnly, this.Factory)));
        }

        /// <summary>
        /// Creates a new <see cref="DataViewModelBase"/> with a copy of the given <paramref name="dataViewModel"/>.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="dataViewModel"/> is not of the type this controller handles.
        /// </exception>
        public override Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
        {
            var readOnly = (FtpServerReadOnlyDataViewModel)dataViewModel;
            var viewModel = new FtpServerDataViewModel(null, this.Factory);
            viewModel.Compression = readOnly.Compression;
            viewModel.Host = readOnly.Host;
            viewModel.Password = readOnly.Password;
            viewModel.PollInterval = readOnly.PollInterval;
            viewModel.Port = readOnly.Port;
            viewModel.RepositoryBasePath = readOnly.RepositoryBasePath;
            viewModel.Username = readOnly.Username;
            return Task.FromResult(this.AddVerification(viewModel));
        }

        /// <summary>
        /// Asynchronously saves the given entity to the background system.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to be saved.
        /// This is usually an object returned by <see cref="EditEntityAsync"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with the saved version of the view model to wait on.
        /// </returns>
        public override async Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dataViewModel)
        {
            var viewModel = (FtpServerDataViewModel)dataViewModel;

            var config =
                (SystemConfigDataViewModel)(await this.systemConfigDataController.EditEntityAsync(this.systemConfig));
            var settings = (BackgroundSystemSettings)config.Settings.XmlData.Deserialize();
            int id;
            if (viewModel.Id == 0)
            {
                settings.FtpUpdateProviders.Add(viewModel.CreateConfig());
                id = settings.FtpUpdateProviders.Count;
            }
            else
            {
                id = viewModel.Id;
                settings.FtpUpdateProviders[viewModel.Id - 1] = viewModel.CreateConfig();
            }

            config.Settings.XmlData = new XmlData(settings);
            await this.systemConfigDataController.SaveEntityAsync(config);
            return await this.GetEntityAsync(id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Asynchronously deletes an entity in the background system.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to delete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public override async Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel)
        {
            var viewModel = (FtpServerReadOnlyDataViewModel)dataViewModel;

            var config =
                (SystemConfigDataViewModel)(await this.systemConfigDataController.EditEntityAsync(this.systemConfig));
            var settings = (BackgroundSystemSettings)config.Settings.XmlData.Deserialize();
            settings.FtpUpdateProviders.RemoveAt(viewModel.Id - 1);
            config.Settings.XmlData = new XmlData(settings);
            await this.systemConfigDataController.SaveEntityAsync(config);
        }

        /// <summary>
        /// Starts reloading all data.
        /// </summary>
        /// <param name="force">
        /// A flag indicating if the data should be reloaded even if it wasn't loaded yet at all.
        /// </param>
        protected async override void StartReloadAllData(bool force)
        {
            if (this.readOnlyList == null)
            {
                if (!force)
                {
                    return;
                }

                this.readOnlyList = new ReadOnlyEntityCollection<FtpServerReadOnlyDataViewModel>(this.allFtpServers);
            }
            else
            {
                this.allFtpServers.Clear();
            }

            var completion = this.loadAllCompletion = new TaskCompletionSource<bool>();
            this.readOnlyList.IsLoading = true;
            try
            {
                try
                {
                    await this.systemConfigDataController.AwaitAllDataAsync();
                    this.systemConfig = this.systemConfigDataController.All.First();
                    this.systemConfig.PropertyChanged += this.SystemConfigOnPropertyChanged;

                    await this.LoadFtpServersAsync();
                }
                finally
                {
                    this.readOnlyList.IsLoading = false;
                }

                completion.TrySetResult(true);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Couldn't load all FTP servers");
                completion.TrySetException(ex);
            }
        }

        /// <summary>
        /// Implementation of the asynchronous load of the details of the given data view model.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await for completion.
        /// </returns>
        protected override Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
        {
            return Task.FromResult(0);
        }

        private async Task LoadFtpServersAsync()
        {
            await this.systemConfig.ReadableModel.LoadXmlPropertiesAsync();

            var settings = (BackgroundSystemSettings)this.systemConfig.ReadableModel.Settings.Deserialize();
            this.allFtpServers.Clear();
            for (var i = 0; i < settings.FtpUpdateProviders.Count; i++)
            {
                var config = settings.FtpUpdateProviders[i];
                this.allFtpServers.Add(new FtpServerReadOnlyDataViewModel(config, i + 1, this.Factory));
            }
        }

        private DataViewModelBase AddVerification(FtpServerDataViewModel dataViewModel)
        {
            dataViewModel.VerifyFtpServerCommand = new RelayCommand<FtpServerDataViewModel>(this.VerifyFtpServer);
            return dataViewModel;
        }

        private async void VerifyFtpServer(FtpServerDataViewModel dataViewModel)
        {
            try
            {
                dataViewModel.VerificationState = VerificationState.Verifying;

                // just so we actually see something if the verification is too fast
                await Task.Delay(TimeSpan.FromSeconds(1));

                var ftpHandler = new FtpHandler(dataViewModel.CreateConfig());
                await Task.Run(() => ftpHandler.GetEntries(dataViewModel.RepositoryBasePath));

                dataViewModel.VerificationState = VerificationState.Ok;
                dataViewModel.VerificationMessage = null;
            }
            catch (Exception ex)
            {
                dataViewModel.VerificationState = VerificationState.Error;
                dataViewModel.VerificationMessage = ex.Message;
                this.Logger.Info(ex , "FTP server verification was not successful");
            }
        }

        private async void SystemConfigOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Settings")
            {
                return;
            }

            try
            {
                await this.LoadFtpServersAsync();
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't update FTP servers");
            }
        }
    }
}