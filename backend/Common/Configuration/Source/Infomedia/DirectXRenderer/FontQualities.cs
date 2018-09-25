// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontQualities.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontQualities type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    /// <summary>
    /// The font qualities supported by DirectX.
    /// </summary>
    public enum FontQualities
    {
        /// <summary>
        /// The clear type natural.
        /// </summary>
        ClearTypeNatural = 6,

        /// <summary>
        /// The clear type.
        /// </summary>
        ClearType = 5,

        /// <summary>
        /// The anti aliased.
        /// </summary>
        AntiAliased = 4,

        /// <summary>
        /// The non anti aliased.
        /// </summary>
        NonAntiAliased = 3,

        /// <summary>
        /// The proof.
        /// </summary>
        Proof = 2,

        /// <summary>
        /// The draft.
        /// </summary>
        Draft = 1,

        /// <summary>
        /// The default.
        /// </summary>
        Default = 0
    }
}