// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodecIdentification.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CodecIdentification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Codec
{
    /// <summary>
    /// The identification of a codec with name and version number.
    /// </summary>
    internal class CodecIdentification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodecIdentification"/> class.
        /// </summary>
        /// <param name="name">
        /// The name which has to be a printable character.
        /// </param>
        /// <param name="version">
        /// The version number.
        /// </param>
        public CodecIdentification(char name, int version)
        {
            this.Name = name;
            this.Version = version;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public char Name { get; private set; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}{1}", this.Name, this.Version);
        }
    }
}
