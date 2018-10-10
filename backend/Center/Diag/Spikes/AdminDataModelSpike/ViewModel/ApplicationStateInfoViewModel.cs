namespace AdminDataModelSpike.ViewModel
{
    using System;

    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.ServiceModel;

    public class ApplicationStateInfoViewModel : InfoViewModelBase
    {
        private readonly IApplicationStateObserver applicationStateObserver;

        private string version;

        private string path;

        private ApplicationState state;

        private string cpuUsage;

        private string ramUsage;

        private string applicationName;

        public ApplicationStateInfoViewModel(IApplicationStateObserver applicationStateObserver)
        {
            this.applicationStateObserver = applicationStateObserver;
            this.applicationStateObserver.StateChanged += this.ApplicationStateObserverOnStateChanged;

            this.Name = "Application State";
            this.State = this.applicationStateObserver.State;
        }

        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                this.SetProperty(ref this.applicationName, value, () => this.ApplicationName);
            }
        }

        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                this.SetProperty(ref this.version, value, () => this.Version);
            }
        }

        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.SetProperty(ref this.path, value, () => this.Path);
            }
        }

        public ApplicationState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.SetProperty(ref this.state, value, () => this.State);
            }
        }

        public string CpuUsage
        {
            get
            {
                return this.cpuUsage;
            }

            set
            {
                this.SetProperty(ref this.cpuUsage, value, () => this.CpuUsage);
            }
        }

        public string RamUsage
        {
            get
            {
                return this.ramUsage;
            }

            set
            {
                this.SetProperty(ref this.ramUsage, value, () => this.RamUsage);
            }
        }

        public void Update(ApplicationInfo applicationInfo)
        {
            this.ApplicationName = applicationInfo.Name;
            this.Version = applicationInfo.Version;
            this.Path = applicationInfo.Path;
            this.State = applicationInfo.State;

            this.CpuUsage = string.Format("{0:0}%", applicationInfo.CpuUsage * 100);
            this.RamUsage = string.Format("{0:#,##0} KB", applicationInfo.RamBytes / 1024);
        }

        private void UpdateState()
        {
            this.State = this.applicationStateObserver.State;
        }

        private void ApplicationStateObserverOnStateChanged(object sender, EventArgs eventArgs)
        {
            this.TaskFactory.StartNew(this.UpdateState);
        }
    }
}