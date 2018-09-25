// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeName.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// This class is not supposed to be used outside this assembly.
    /// Holds the full name of a type and provides the <see cref="System.Type"/>
    /// if it can be found.
    /// </summary>
    public class TypeName : IBecSerializable, IXmlSerializable
    {
        private Type type;

        private bool? isKnown;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeName"/> class.
        /// This constructor is only for BEC and XML serialization support
        /// </summary>
        public TypeName()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeName"/> class
        /// with the full name of a type.
        /// </summary>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        public TypeName(string fullName)
        {
            this.FullName = fullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeName"/> class
        /// with a given type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public TypeName(Type type)
        {
            this.FullName = type.FullName;
            this.type = type;
            this.isKnown = true;
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets the type for the <see cref="FullName"/>.
        /// </summary>
        /// <exception cref="TypeLoadException">if the type couldn't be found on the local system.</exception>
        public Type Type
        {
            get
            {
                this.CreateType();
                if (!this.IsKnown)
                {
                    throw new TypeLoadException("Couldn't find " + this.FullName);
                }

                return this.type;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the type is known.
        /// </summary>
        public bool IsKnown
        {
            get
            {
                this.CreateType();
                return this.isKnown.HasValue && this.isKnown.Value;
            }
        }

        /// <summary>
        /// Get the type name for a given object.
        /// This method supports <see cref="IUnknown"/> and
        /// will return <see cref="IUnknown.TypeName"/> if the
        /// given object is an <see cref="IUnknown"/>.
        /// </summary>
        /// <param name="obj">
        /// The object for which you want to get the type name.
        /// </param>
        /// <returns>
        /// The type name for the given object.
        /// </returns>
        public static TypeName GetNameFor(object obj)
        {
            var unknown = obj as IUnknown;
            return unknown != null ? unknown.TypeName : new TypeName(obj.GetType());
        }

        /// <summary>
        /// Get the type name for a given generic type argument.
        /// This is a shortcut for <code>new TypeName(typeof(T))</code>.
        /// </summary>
        /// <typeparam name="T">
        /// The type for which you want to get the type name.
        /// </typeparam>
        /// <returns>
        /// The type name for the given type.
        /// </returns>
        public static TypeName Of<T>()
        {
            return new TypeName(typeof(T));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is
        /// equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal
        ///  to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with
        /// the current <see cref="T:System.Object"/>.</param>
        public override bool Equals(object obj)
        {
            var other = obj as TypeName;

            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.FullName == other.FullName;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.FullName.GetHashCode();
        }

        /// <summary>
        /// Returns the full name.
        /// </summary>
        /// <returns>The full name.</returns>
        public override string ToString()
        {
            return this.FullName;
        }

        BecSchema IBecSerializable.GetSchema()
        {
            return new BecSchema(Of<string>());
        }

        void IBecSerializable.WriteBec(BecWriter writer, BecSchema schema)
        {
            writer.WriteString(this.FullName);
        }

        void IBecSerializable.ReadBec(BecReader reader, BecSchema schema)
        {
            this.FullName = reader.ReadString();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.FullName = reader.ReadElementString();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteString(this.FullName);
        }

        private void CreateType()
        {
            if (this.isKnown != null)
            {
                return;
            }

            this.type = TypeFactory.Instance[this.FullName];
            this.isKnown = this.type != null;
        }
    }
}
