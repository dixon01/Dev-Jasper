namespace Library.ServiceModel
{
    using System.Runtime.Serialization;

    [DataContract]
    public class TenantDelta : Delta
    {
        private PropertyChange<string> name;

        private PropertyChange<string> description;

        public TenantDelta(int id, Changeset changeset)
        {
            this.Id = id;
            this.Changeset = changeset;
        }

        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public PropertyChange<string> Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.TrapDisposed();
                this.name = value;
            }
        }

        [DataMember]
        public PropertyChange<string> Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.TrapDisposed();
                this.description = value;
            }
        }

        public override void Clear()
        {
            this.Description = null;
            this.Name = null;
        }

        public void IncrementChangeset()
        {
            this.Changeset = new Changeset(this.Changeset.Value + 1);
        }
    }

    [DataContract]
    public class PropertyChange<T>
    {
        public PropertyChange(T value)
        {
            this.Value = value;
        }

        public PropertyChange()
        {
        }

        [DataMember]
        public T Value { get; set; }
    }
}