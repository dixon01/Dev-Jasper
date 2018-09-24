// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopApproachingCycleState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.StateMachineCycles
{
    /// <summary>
    /// Stop Approaching cycle class.
    /// </summary>
    public class StopApproachingCycleState : State
    {
        /// <summary>
        /// EventStopRequest state handling
        /// </summary>
        /// <returns>
        /// The event stop request.
        /// </returns>
        public override CycleStateValue EventStopRequest()
        {
            return CycleStateValue.StopReqStopApprCycleValue;
        }

        /// <summary>
        /// EventRemovedStopRequest state handling
        /// </summary>
        /// <returns>
        /// The event removed stop request.
        /// </returns>
        public override CycleStateValue EventRemovedStopRequest()
        {
            return CycleStateValue.MainCycleValue;
        }

        /// <summary>
        /// Event100m state handling
        /// </summary>
        /// <returns>
        /// The event 100 m.
        /// </returns>
        public override CycleStateValue EventStopApproach()
        {
            return CycleStateValue.StopApproachingCycleValue;
        }

        /// <summary>
        /// EventRemoved100m state handling
        /// </summary>
        /// <returns>
        /// The event removed 100 m.
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
