// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainCycleState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.StateMachineCycles
{
    /// <summary>
    /// Main cycle class
    /// </summary>
    public class MainCycleState : State
    {
        /// <summary>
        /// EventStopRequest state handling
        /// </summary>
        /// <returns>
        /// Stop Request state
        /// </returns>
        public override CycleStateValue EventStopRequest()
        {
            return CycleStateValue.StopRequestCycleValue;
        }

        /// <summary>
        /// EventRemovedStopRequest state handling
        /// </summary>
        /// <returns>
        /// new state
        /// </returns>
        public override CycleStateValue EventRemovedStopRequest()
        {
            return CycleStateValue.MainCycleValue;
        }

        /// <summary>
        /// EventStopApproach state handling
        /// </summary>
        /// <returns>
        /// The event StopApproach.
        /// </returns>
        public override CycleStateValue EventStopApproach()
        {
            return CycleStateValue.StopApproachingCycleValue;
        }

        /// <summary>
        /// EventRemovedStopApproach state handling
        /// </summary>
        /// <returns>
        /// The event removed StopApproach.
        /// </returns>
        public override CycleStateValue EventRemovedStopApproach()
        {
            return CycleStateValue.MainCycleValue;
        }

        /// <summary>
        /// EventNextStop state handling
        /// </summary>
        /// <returns>
        /// The event next stop.
        /// </returns>
        public override CycleStateValue EventNextStop()
        {
            return CycleStateValue.MainCycleValue;
        }
    }
}
