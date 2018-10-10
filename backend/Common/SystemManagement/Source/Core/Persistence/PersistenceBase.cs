// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistenceBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PersistenceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Persistence
{
    using System.Collections.Generic;

    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Base class for system and application persistence that keeps track of the last
    /// 20 launches (or boots) and exits (or shutdowns).
    /// </summary>
    public abstract class PersistenceBase
    {
        private const int MaxReasonCount = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceBase"/> class.
        /// </summary>
        protected PersistenceBase()
        {
            this.LaunchReasons = new List<ApplicationReasonInfo>();
            this.ExitReasons = new List<ApplicationReasonInfo>();
        }

        /// <summary>
        /// Gets or sets the last 20 reasons why the application has been launched.
        /// </summary>
        public List<ApplicationReasonInfo> LaunchReasons { get; set; }

        /// <summary>
        /// Gets or sets the last 20 reasons why the application has exited.
        /// </summary>
        public List<ApplicationReasonInfo> ExitReasons { get; set; }

        /// <summary>
        /// Adds a reason to <see cref="LaunchReasons"/>, keeping the limit of 20 reasons.
        /// This also updates the <see cref="ApplicationReasonInfo.TimestampUtc"/> to the current time.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void AddLaunchReason(ApplicationReasonInfo reason)
        {
            AddReason(this.LaunchReasons, reason);
        }

        /// <summary>
        /// Adds a reason to <see cref="ExitReasons"/>, keeping the limit of 20 reasons.
        /// This also updates the <see cref="ApplicationReasonInfo.TimestampUtc"/> to the current time.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public void AddExitReason(ApplicationReasonInfo reason)
        {
            AddReason(this.ExitReasons, reason);
        }

        private static void AddReason(IList<ApplicationReasonInfo> reasons, ApplicationReasonInfo reason)
        {
            reason.TimestampUtc = TimeProvider.Current.UtcNow;
            while (reasons.Count >= MaxReasonCount - 1)
            {
                reasons.RemoveAt(reasons.Count - 1);
            }

            reasons.Insert(0, reason);
        }
    }
}