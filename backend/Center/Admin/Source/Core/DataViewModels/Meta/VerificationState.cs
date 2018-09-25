// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerificationState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VerificationState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Meta
{
    /// <summary>
    /// The verification state.
    /// </summary>
    public enum VerificationState
    {
        /// <summary>
        /// The object has not yet been verified.
        /// </summary>
        Unknown,

        /// <summary>
        /// The object is being verified.
        /// </summary>
        Verifying,

        /// <summary>
        /// The verification was successful.
        /// </summary>
        Ok,

        /// <summary>
        /// The verification was unsuccessful.
        /// </summary>
        Error
    }
}