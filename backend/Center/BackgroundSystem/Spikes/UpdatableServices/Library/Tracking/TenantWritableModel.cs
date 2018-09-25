namespace Library.Tracking
{
    using System;

    using Library.ServiceModel;

    public class TenantWritableModel : TrackingModelBase
    {
        private string name;

        private string description;

        private TenantDelta delta;

        public TenantWritableModel(
            int id,
            Changeset changeset,
            string name,
            string description,
            DateTime? lastModifiedOn)
        {
            this.Id = id;
            this.name = name;
            this.description = description;
            this.LastModifiedOn = lastModifiedOn;
            this.Delta = new TenantDelta(id, changeset);
        }

        private TenantDelta Delta
        {
            get
            {
                return this.delta;
            }
            set
            {
                this.delta = value;
                this.SetDelta(value);
            }
        }

        public DateTime? LastModifiedOn { get; set; }

        public int Id { get; private set; }

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
                this.Delta.Description = new PropertyChange<string>(value);
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.Delta.Name = new PropertyChange<string>(value);
            }
        }
    }

    public class WritableModelUpdatedEventArgs : EventArgs
    {
        public WritableModelUpdatedEventArgs(Delta delta)
        {
            this.Delta = delta;
        }

        public Delta Delta { get; private set; }
    }
}