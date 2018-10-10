namespace Library.ViewModel
{
    using System;

    using Library.Tracking;

    public class TenantDataViewModel : TrackingDataViewModelBase<TenantWritableModel>
    {
        public TenantDataViewModel(TenantWritableModel model)
            : base(model)
        {
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

            set
            {
                this.Model.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return this.Model.Description;
            }

            set
            {
                this.Model.Description = value;
            }
        }

        public DateTime? LastModifiedOn { get; set; }
    }
}