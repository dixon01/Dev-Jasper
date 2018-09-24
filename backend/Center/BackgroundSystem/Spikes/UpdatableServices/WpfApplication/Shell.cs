namespace WpfApplication
{
    using System;
    using System.Collections.ObjectModel;

    using Library.ViewModel;

    using WpfApplication.Controllers;

    using TenantDataViewModel = Library.ViewModel.TenantDataViewModel;
    using ViewModelBase = WpfApplication.ViewModels.ViewModelBase;

    public class Shell : ViewModelBase
    {
        private static readonly Lazy<Shell> LazyInstance = new Lazy<Shell>(() => new Shell());

        private readonly Lazy<string> lazySessionId = new Lazy<string>(
            () => TenantsController.Instance.SessionId);

        private TenantDataViewModel editingTenant;

        private ReadOnlyTenantDataViewModel selectedTenant;

        private string sessionId;

        private Shell()
        {
            this.Tenants = new ObservableCollection<ReadOnlyTenantDataViewModel>();
        }

        public static Shell Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        public ObservableCollection<ReadOnlyTenantDataViewModel> Tenants { get; private set; }

        public TenantDataViewModel EditingTenant
        {
            get { return this.editingTenant; }
            set
            {
                if (object.Equals(value, this.editingTenant))
                {
                    return;
                }

                this.editingTenant = value;
                this.OnPropertyChanged();
            }
        }

        public ReadOnlyTenantDataViewModel SelectedTenant
        {
            get
            {
                return this.selectedTenant;
            }
            set
            {
                if (value == this.selectedTenant)
                {
                    return;
                }

                this.selectedTenant = value;
                this.OnPropertyChanged();
            }
        }

        public string SessionId
        {
            get
            {
                return this.lazySessionId.Value;
            }
        }
    }
}
