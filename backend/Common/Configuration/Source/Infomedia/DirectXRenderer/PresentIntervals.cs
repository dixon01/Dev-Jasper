// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentIntervals.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentIntervals type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    /// <summary>
    /// The presentation intervals supported by DirectX.
    /// </summary>
    public enum PresentIntervals
    {
        /// <summary>
        /// The immediate.
        /// </summary>
        Immediate = -2147483648,

        /// <summary>
        /// The four.
        /// </summary>
        Four = 8,

        /// <summary>
        /// The three.
        /// </summary>
        Three = 4,

        /// <summary>
        /// The two.
        /// </summary>
        Two = 2,

        /// <summary>
        /// The one.
        /// </summary>
        One = 1,

        /// <summary>
        /// The default.
        /// </summary>
        Default = 0
    }
}