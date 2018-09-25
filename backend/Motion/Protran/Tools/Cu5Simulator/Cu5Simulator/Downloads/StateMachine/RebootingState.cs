// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RebootingState.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine
{
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Protocols.Ctu.Datagram;
    using Gorba.Motion.Protran.Tools.Cu5Simulator.Downloads.StateMachine.Utils;

    /// <summary>
    /// The Rebooting state
    /// </summary>
    public class RebootingState : State
    {
        private const int RebootTime = 10 * 1000;

        private Context contextStateMachine;

        /// <summary>
        /// Handles the things to do for this state.
        /// </summary>
        /// <param name="stateMachine">
        /// The state machine's stateMachine.
        /// </param>
        /// <param name="triplet">
        /// The triplet.
        /// </param>
        public override void Handle(Context stateMachine, Triplet triplet)
        {
            // in this state we don't have nothing to do.
            // just simulate the CU5 reboot.
            this.contextStateMachine = stateMachine;
            var nextStates = new List<string>();
            nextStates.AddRange(new[] { "WaitingForCtu" });
            var logEvent = new LogMessageEventArgs(this.GetType().Name, nextStates, "Rebooting...");
            stateMachine.RaiseLogMessageProduced(logEvent);

            var rebooter = new Thread(this.SimulateReboot) { Name = "Th_Rebooter" };
            rebooter.Start();
        }

        private void SimulateReboot()
        {
            Thread.Sleep(RebootTime);
            var nextStates = new List<string>();
            nextStates.AddRange(new[] { "WaitingForCtu" });
            var logEvent = new LogMessageEventArgs(this.GetType().Name, nextStates, "Reboot finished.");
            this.contextStateMachine.RaiseLogMessageProduced(logEvent);

            // now, after the "reboot", it's the time to go to the WaitingForCtuState
            this.contextStateMachine.SetNewState(new WaitingForCtuState());
        }
    }
}
