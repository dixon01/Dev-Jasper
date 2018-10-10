namespace Library.Tracking
{
    using System;
    using System.Runtime.Serialization;

    using Library.Model;
    using Library.ServiceModel;

    [DataContract]
    public class TenantReadableModel : ViewModelBase
    {
        public event EventHandler<WritableModelCreatedEventArgs> WritableModelCreated;

        public TenantReadableModel(Tenant tenant)
        {
            this.Id = tenant.Id;
            this.Changeset = tenant.Changeset;
            this.Name = tenant.Name;
            this.Description = tenant.Description;
            this.LastModifiedOn = tenant.LastModifiedOn;
        }

        protected virtual void OnWritableModelCreated(WritableModelCreatedEventArgs e)
        {
            var handler = this.WritableModelCreated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        protected virtual void OnWritableModelCreated(ITrackingModel model)
        {
            this.OnWritableModelCreated(new WritableModelCreatedEventArgs(model));
        }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public DateTime? LastModifiedOn { get; private set; }

        public TenantWritableModel ToWritable()
        {
            var model = new TenantWritableModel(
                this.Id,
                this.Changeset,
                this.Name,
                this.Description,
                this.LastModifiedOn);
            this.OnWritableModelCreated(model);
            return model;
        }

        [DataMember]
        public Changeset Changeset { get; set; }

        public void Apply(TenantDelta delta)
        {
            if (delta.Name != null)
            {
                this.Name = delta.Name.Value;
                this.OnPropertyChanged("Name");
            }

            if (delta.Description != null)
            {
                this.Description = delta.Description.Value;
                this.OnPropertyChanged("Description");
            }

            this.Changeset = delta.Changeset;
            this.OnPropertyChanged("Changeset");
        }
    }

    public class WritableModelCreatedEventArgs : EventArgs
    {
        public WritableModelCreatedEventArgs(ITrackingModel trackingModel)
        {
            this.TrackingModel = trackingModel;
        }

        public ITrackingModel TrackingModel { get; private set; }
    }
}