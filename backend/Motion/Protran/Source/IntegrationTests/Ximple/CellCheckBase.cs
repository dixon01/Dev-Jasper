// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CellCheckBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Ximple
{
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// A verification of a single Ximple cell.
    /// </summary>
    public abstract class CellCheckBase
    {
        /// <summary>
        /// The verify.
        /// </summary>
        /// <param name="ximple">
        /// The ximple.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public abstract bool Verify(Ximple ximple);

        /// <summary>
        /// ToString just for debug purposes.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}
