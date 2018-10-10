// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownBecList.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownBecList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// A list (or array) of unknown objects.
    /// This class is used if an <see cref="UnknownBecObject"/> has a
    /// property with either an <see cref="IList{T}"/> or an array type
    /// for which the type of the elements is unknown.
    /// </summary>
    internal class UnknownBecList : IUnknown
    {
        private readonly ListTypeSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownBecList"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        public UnknownBecList(ListTypeSchema schema)
        {
            this.schema = schema;
            this.Items = new List<object>();
        }

        /// <summary>
        /// Gets the items of this list.
        /// </summary>
        public IList<object> Items { get; private set; }

        /// <summary>
        /// Gets the type name of the object represented by this object.
        /// </summary>
        public TypeName TypeName
        {
            get
            {
                return this.schema.TypeName;
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