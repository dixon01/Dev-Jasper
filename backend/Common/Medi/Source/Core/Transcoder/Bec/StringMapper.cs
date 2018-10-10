// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringMapper.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringMapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    /// <summary>
    /// Mapper implementation for strings with two predefined IDs:
    /// - ID 0 is null.
    /// - ID 1 is an empty string.
    /// </summary>
    internal class StringMapper : Mapper<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMapper"/> class.
        /// </summary>
        public StringMapper()
        {
            this.Add(this.GetNextId(), string.Empty);
        }
    }
}
