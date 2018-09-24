namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Logging;
    using Gorba.Common.SystemManagement.Client;

    using LogLevel = NLog.LogLevel;

    public class ApplicationViewModel : TabableViewModelBase
    {
        private readonly IRootMessageDispatcher messageDispatcher;

        private string name;

        private MediAddress address;

        private ApplicationStateInfoViewModel applicationStateInfo;

        private string executableName;

        private MediManagementTreeViewModel mediManagementTree;

        private MediLogViewModel logViewModel;

        public ApplicationViewModel(IRootMessageDispatcher messageDispatcher)
        {
            this.messageDispatcher = messageDispatcher;
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        public MediAddress Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.SetProperty(ref this.address, value, () => this.Address);
            }
        }

        public bool HasApplicationName(string applicationName)
        {
            if (this.Address == null)
            {
                return applicationName.Equals(this.executableName, StringComparison.InvariantCultureIgnoreCase);
            }

            return applicationName.Equals(this.Address.Application, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Update(ApplicationInfo applicationInfo, RemoteSystemManagerClient systemManagementClient)
        {
            this.Name = applicationInfo.Name;
            this.executableName = Path.GetFileNameWithoutExtension(applicationInfo.Path);

            if (this.applicationStateInfo == null)
            {
                this.applicationStateInfo = new ApplicationStateInfoViewModel(
                    systemManagementClient.CreateApplicationStateObserver(applicationInfo));
                this.Tabs.Insert(0, this.applicationStateInfo);
            }

            this.applicationStateInfo.Update(applicationInfo);
        }

        public void Update(MediAddress mediAddress)
        {
            this.Address = mediAddress;
            if (this.Name == null)
            {
                this.Name = mediAddress.Application;
            }

            if (this.mediManagementTree == null)
            {
                this.mediManagementTree =
                    new MediManagementTreeViewModel(
                        this.messageDispatcher.ManagementProviderFactory.CreateRemoteProvider(mediAddress));
                this.Tabs.Add(this.mediManagementTree);
            }

            if (this.logViewModel == null)
            {
                this.logViewModel =
                    new MediLogViewModel(this.messageDispatcher.LogObserverFactory.CreateRemoteObserver(mediAddress));
                this.Tabs.Add(this.logViewModel);
            }
        }
    }

    public class MediLogViewModel : InfoViewModelBase
    {
        private readonly ILogObserver logObserver;

        private bool isEnabled;

        private LogEventArgs selectedLog;

        public MediLogViewModel(ILogObserver logObserver)
        {
            this.logObserver = logObserver;

            this.Name = "Logging";

            this.Logs = new ObservableCollection<LogEventArgs>();
            this.LogLevels = new List<LogLevel>
                                 {
                                     LogLevel.Fatal,
                                     LogLevel.Error,
                                     LogLevel.Warn,
                                     LogLevel.Info,
                                     LogLevel.Debug,
                                     LogLevel.Trace
                                 };
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (!this.SetProperty(ref this.isEnabled, value, () => this.IsEnabled))
                {
                    return;
                }

                if (this.isEnabled)
                {
                    this.logObserver.MessageLogged += this.LogObserverOnMessageLogged;
                }
                else
                {
                    this.logObserver.MessageLogged -= this.LogObserverOnMessageLogged;
                }
            }
        }

        public IList<LogLevel> LogLevels { get; private set; }

        public LogLevel MinLevel
        {
            get
            {
                return this.logObserver.MinLevel;
            }
            set
            {
                if (this.logObserver.MinLevel.Equals(value))
                {
                    return;
                }

                this.logObserver.MinLevel = value;
                this.RaisePropertyChanged(() => this.MinLevel);
            }
        }

        public ObservableCollection<LogEventArgs> Logs { get; private set; }

        public LogEventArgs SelectedLog
        {
            get
            {
                return this.selectedLog;
            }

            set
            {
                this.SetProperty(ref this.selectedLog, value, () => this.SelectedLog);
            }
        }

        private void LogObserverOnMessageLogged(object sender, LogEventArgs e)
        {
            this.TaskFactory.StartNew(
                () =>
                    {
                        this.Logs.Add(e);
                        this.SelectedLog = e;
                    });
        }
    }
}