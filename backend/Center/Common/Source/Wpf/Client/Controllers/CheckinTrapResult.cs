// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckinTrapResult.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The check in trap result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    /// <summary>
    /// The check in trap result.
    /// </summary>
    public class CheckinTrapResult
    {
        /// <summary>
        /// Gets or sets the decision.
        /// </summary>
        public CheckinUserDecision Decision { get; set; }

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Gets or sets the check in comment.
        /// </summary>
        public string CheckinComment { get; set; }
    }
}