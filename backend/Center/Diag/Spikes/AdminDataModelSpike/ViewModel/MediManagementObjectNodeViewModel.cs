namespace AdminDataModelSpike.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gorba.Common.Medi.Core.Management;

    public class MediManagementObjectNodeViewModel : MediManagementDataNodeViewModelBase
    {
        private readonly IManagementObjectProvider provider;

        public MediManagementObjectNodeViewModel(IManagementObjectProvider provider, MediManagementTreeViewModel owner)
            : base(provider, owner)
        {
            this.provider = provider;
            this.Properties = new ObservableCollection<ManagementProperty>();
        }

        public ObservableCollection<ManagementProperty> Properties { get; private set; }

        protected override Action LoadData()
        {
            var properties = this.provider.Properties.ToList();
            return () =>
                {
                    foreach (var property in properties)
                    {
                        this.Properties.Add(property);
                    }
                };
        }
    }
}