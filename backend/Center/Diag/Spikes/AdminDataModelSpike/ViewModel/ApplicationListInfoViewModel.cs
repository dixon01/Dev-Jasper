namespace AdminDataModelSpike.ViewModel
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Common.SystemManagement.Client;

    public class ApplicationListInfoViewModel : InfoViewModelBase
    {
        public ApplicationListInfoViewModel()
        {
            this.Applications = new ObservableCollection<ApplicationStateInfoViewModel>();
            this.Name = "Applications";
        }

        public ObservableCollection<ApplicationStateInfoViewModel> Applications { get; private set; }

        public void Update(IList<ApplicationInfo> applications, RemoteSystemManagerClient systemManagementClient)
        {
            if (this.Applications.Count == 0)
            {
                foreach (var application in applications)
                {
                    this.Applications.Add(
                        new ApplicationStateInfoViewModel(
                            systemManagementClient.CreateApplicationStateObserver(application)));
                }
            }

            for (int i = 0; i < applications.Count && i < this.Applications.Count; i++)
            {
                this.Applications[i].Update(applications[i]);
            }
        }
    }
}