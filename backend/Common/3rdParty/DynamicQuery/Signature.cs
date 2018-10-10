namespace DynamicQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Signature : IEquatable<Signature>
    {
        public DynamicProperty[] properties;

        public int hashCode;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            this.properties = properties.ToArray();
            this.hashCode = 0;
            foreach (DynamicProperty p in properties)
            {
                this.hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is Signature ? this.Equals((Signature)obj) : false;
        }

        public bool Equals(Signature other)
        {
            if (this.properties.Length != other.properties.Length) return false;
            for (int i = 0; i < this.properties.Length; i++)
            {
                if (this.properties[i].Name != other.properties[i].Name || this.properties[i].Type != other.properties[i].Type) return false;
            }
            return true;
        }
    }
}