// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Constants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.QualityScreenProvider
{
    /// <summary>
    /// The constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// All common table names.
        /// </summary>
        public static class Tables
        {
            /// <summary>
            /// The SystemStatus table.
            /// </summary>
            public static readonly string SystemStatus = "SystemStatus";

            /// <summary>
            /// The AllBuilds table.
            /// </summary>
            public static readonly string AllBuilds = "AllBuilds";

            /// <summary>
            /// The FailedBuilds table.
            /// </summary>
            public static readonly string FailedBuilds = "FailedBuilds";
        }

        /// <summary>
        /// All common column names.
        /// </summary>
        public static class Columns
        {
            /// <summary>
            /// The BuildDefinitionName column.
            /// </summary>
            public static readonly string BuildDefinitionName = "BuildDefinitionName";

            /// <summary>
            /// The ErrorState column.
            /// </summary>
            public static readonly string ErrorState = "ErrorState";

            /// <summary>
            /// The UserName column.
            /// </summary>
            public static readonly string UserName = "UserName";

            /// <summary>
            /// The Timestamp column.
            /// </summary>
            public static readonly string Timestamp = "Timestamp";

            /// <summary>
            /// The ErrorDescription column.
            /// </summary>
            public static readonly string ErrorDescription = "ErrorDescription";
        }
    }
}
