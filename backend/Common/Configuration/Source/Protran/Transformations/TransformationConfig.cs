// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Transformations
{
    using System;

    /// <summary>
    /// Container of all the transformation that Protran
    /// has to do on the IBIS telegrams.
    /// </summary>
    [Serializable]
    public abstract class TransformationConfig
    {
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
