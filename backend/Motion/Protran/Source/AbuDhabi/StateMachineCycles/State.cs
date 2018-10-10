// --------------------------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.StateMachineCycles
{
    /// <summary>
    /// Base class for State
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Enumeration for the cycle state values
        /// </summary>
        public enum CycleStateValue
        {
            /// <summary>
            /// Main Cycle Value
            /// </summary>
            MainCycleValue = 0,

            /// <summary>
            /// Stop Cycle Value.
            /// </summary>
            StopRequestCycleValue,

            /// <summary>
            /// Stop Approaching Cycle Value.
            /// </summary>
            StopApproachingCycleValue,

            /// <summary>
            /// StopReq StopAppr Cycle Value.
            /// </summary>
            StopReqStopApprCycleValue,

            /// <summary>
            /// Empty State Value used as default value.
            /// </summary>
            EmptyStateValue
        }

        /// <summary>
        /// Stop request event
        /// </summary>
        /// <returns>
        /// The event stop request.
        /// </returns>
        public virtual CycleStateValue EventStopRequest()
        {
            return CycleStateValue.EmptyStateValue;
        }

        /// <summary>
        /// Removed Stop request event
        /// </summary>
        /// <returns>
        /// The event removed stop request.
        /// </returns>
        public virtual CycleStateValue EventRemovedStopRequest()
        {
            return CycleStateValue.EmptyStateValue;
        }

        /// <summary>
        /// Stop Approach event
        /// </summary>
        /// <returns>
        /// The event Stop Approach.
        /// </returns>
        public virtual CycleStateValue EventStopApproach()
        {
            return CycleStateValue.EmptyStateValue;
        }

        /// <summary>
        /// Removed Stop Approach event
        /// </summary>
        /// /// 
        /// <returns>
        /// The event removed Stop Approach.
        /// </returns>
        public virtual CycleStateValue EventRemovedStopApproach()
        {
            return CycleStateValue.EmptyStateValue;
        }

        /// <summary>
        /// Next stop event
        /// </summary>
        /// <returns>
        /// The event next stop.
        /// </returns>
        public virtual CycleStateValue EventNextStop()
        {
            return CycleStateValue.EmptyStateValue;
        }
    }
}
