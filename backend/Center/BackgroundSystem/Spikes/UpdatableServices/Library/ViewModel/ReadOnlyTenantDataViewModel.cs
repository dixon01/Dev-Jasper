namespace Library.ViewModel
{
    using System;
    using System.ComponentModel;

    using Library.ServiceModel;
    using Library.Tracking;

    public class ReadOnlyTenantDataViewModel : ViewModelBase
    {
        public ReadOnlyTenantDataViewModel(TenantReadableModel model)
        {
            this.Model = model;
            model.PropertyChanged += ModelOnPropertyChanged;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.OnPropertyChanged(propertyChangedEventArgs.PropertyName);
        }

        public string Description
        {
            get
            {
                return this.Model.Description;
            }
        }

        public int Id
        {
            get
            {
                return this.Model.Id;
            }
        }

        public string Name
        {
            get
            {
                return this.Model.Name;
            }
        }

        public DateTime? LastModifiedOn
        {
            get
            {
                return this.Model.LastModifiedOn;
            }
        }

        public TenantReadableModel Model { get; private set; }

        public Changeset Changeset
        {
            get
            {
                return this.Model.Changeset;
            }
        }
    }
}