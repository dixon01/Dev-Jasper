// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Status.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    /// <summary>
    /// Supported Protran status about received telegrams.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Whenever everything is running well.
        /// </summary>
        Ok,

        /// <summary>
        /// Whenever Protran hasn't received any data from the IBIS master.
        /// </summary>
        NoData,

        /// <summary>
        /// Whenever Protran is missing data from the IBIS master.
        /// </summary>
        MissingData,

        /// <summary>
        /// Whenever wrong values are received within IBIS telegrams.
        /// </summary>
        IncorrectRecord
    }
}
