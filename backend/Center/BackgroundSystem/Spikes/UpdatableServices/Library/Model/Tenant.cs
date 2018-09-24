namespace Library.Model
{
    using System;

    using Library.ServiceModel;

    public class Tenant : ICloneable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Changeset Changeset { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }
    }
}