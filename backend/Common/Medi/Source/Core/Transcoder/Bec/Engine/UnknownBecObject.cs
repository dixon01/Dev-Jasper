// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownBecObject.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownBecObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// <see cref="IUnknown"/> implementation for BEC.
    /// This is a placeholder for all unknown objects that get
    /// deserialized.
    /// </summary>
    internal class UnknownBecObject : IUnknown
    {
        private readonly Dictionary<string, object> members;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownBecObject"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema of the unknown type.
        /// </param>
        public UnknownBecObject(BecSchema schema)
        {
            this.Schema = schema;
            this.members = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the schema of the unknown type.
        /// </summary>
        public BecSchema Schema { get; private set; }

        /// <summary>
        /// Gets the type name of the unknown type.
        /// The <see cref="Core.TypeName.IsKnown"/> of the
        /// returned value will always be false, since it
        /// is an unknown type.
        /// </summary>
        public TypeName TypeName
        {
            get
            {
                return this.Schema.TypeName;
            }
        }

        /// <summary>
        /// Gets or sets a member value for a given name.
        /// </summary>
        /// <param name="name">
        /// The name of the member
        /// </param>
        /// <returns>
        /// The member's value.
        /// </returns>
        public object this[string name]
        {
            get
            {
                return this.members[name];
            }

            set
            {
                this.members[name] = value;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.GetType().Name, this.TypeName.FullName);
        }
    }
}