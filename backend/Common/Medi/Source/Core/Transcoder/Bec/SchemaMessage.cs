// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaMessage.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This class is not meant for use outside this assembly.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// This class is not meant for use outside this assembly.
    /// Internal message between two instances of BEC to transmit
    /// a schema and its id.
    /// </summary>
    public class SchemaMessage
    {
        /// <summary>
        /// Gets or sets the id of the schema.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public ITypeSchema Schema { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("SchemaMessage[Id={0},Schema={1}]", this.Id, this.Schema);
        }
    }
}
