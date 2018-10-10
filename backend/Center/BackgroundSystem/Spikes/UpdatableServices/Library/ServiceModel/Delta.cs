namespace Library.ServiceModel
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(TenantDelta))]
    public abstract class Delta : IDisposable
    {
        private bool isDisposed;

        [DataMember]
        public Changeset Changeset { get; protected set; }

        public bool IsDirty { get; protected set; }

        public abstract void Clear();

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Clear();
            this.isDisposed = true;
        }

        protected void TrapDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("Delta");
            }
        }
    }
}