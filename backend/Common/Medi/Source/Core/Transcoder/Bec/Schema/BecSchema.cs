// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecSchema.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    using System.Collections.Generic;

    /// <summary>
    /// Schema defining how a type is serialized and deserialized in BEC.
    /// </summary>
    public class BecSchema : ITypeSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BecSchema"/> class. 
        /// This constructor is not meant for use outside this assembly.
        /// </summary>
        public BecSchema()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BecSchema"/> class
        /// for a given type. This constructor does not fill the <see cref="Members"/>
        /// list, that has to be done by the user of this object.
        /// </summary>
        /// <param name="typeName">
        /// The type name for the schema.
        /// </param>
        public BecSchema(TypeName typeName)
        {
            this.TypeName = typeName;
            this.Members = new List<SchemaMember>();
        }

        /// <summary>
        /// Gets or sets the type name.
        /// You should never set the type name manually, 
        /// this is only used for serialization.
        /// </summary>
        public TypeName TypeName { get; set; }

        /// <summary>
        /// Gets or sets a list of members of this schema.
        /// You should never set the members manually, 
        /// this is only used for serialization.
        /// </summary>
        public List<SchemaMember> Members { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("BecSchema[{0}]", this.TypeName.FullName);
        }
    }
}
