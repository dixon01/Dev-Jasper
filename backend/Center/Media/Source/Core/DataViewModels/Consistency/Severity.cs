// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Severity.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Consistency
{
    /// <summary>
    /// The severity enumeration.
    /// </summary>
    public enum Severity
    {
         /// <summary>
        /// No error or warning.
        /// </summary>
        None,

        /// <summary>
        /// A warning. The exported project may not run as expected.
        /// </summary>
        Warning,

        /// <summary>
        /// An error. The exported project won't run.
        /// </summary>
        Error,

        /// <summary>
        /// A software version compatibility issue.
        /// </summary>
        CompatibilityIssue
    }
}
