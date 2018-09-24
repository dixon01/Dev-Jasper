// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutyBlock.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DutyBlock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Screens;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;

    /// <summary>
    /// The duty block state.
    /// </summary>
    internal class DutyBlock : State
    {
        private readonly DriveInfo driveInfo;
        private MainFieldKey rootScreen = MainFieldKey.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="DutyBlock"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal DutyBlock(IContext context)
            : base(context)
        {
            this.rootScreen = MainFieldKey.BlockDriveWait;
            this.driveInfo = context.StatusHandler.DriveInfo;
        }

        /// <summary>
        ///   Gets the Root screen of the state
        /// </summary>
        /// <returns>
        /// The root screen.
        /// </returns>
        internal override MainFieldKey GetRootScreen()
        {
            return this.rootScreen;
        }

        /// <summary>
        ///   This method will be called by entering this state.
        ///   Register here to events you want to receive!
        /// </summary>
        internal override void EnterState()
        {
            MessageDispatcher.Instance.Subscribe<evBUSStopReached>(this.EvBusStopReachedEvent);
            MessageDispatcher.Instance.Subscribe<evBUSStopLeft>(this.EvBusStopLeftEvent);
            MessageDispatcher.Instance.Subscribe<evServiceEnded>(this.EvServiceEndedEvent);
            MessageDispatcher.Instance.Subscribe<evTripEnded>(this.EvTripEndedEvent);

            this.Context.StatusHandler.SaveStatus();
        }

        /// <summary>
        ///   This method will be called by leaving this state.
        ///   UNREGISTER ALL EVENTS you have registered in the EnterState() method!!!
        /// </summary>
        internal override void LeaveState()
        {
            this.Context.StatusHandler.DeleteSavedStatus();

            MessageDispatcher.Instance.Unsubscribe<evBUSStopReached>(this.EvBusStopReachedEvent);
            MessageDispatcher.Instance.Unsubscribe<evBUSStopLeft>(this.EvBusStopLeftEvent);
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.EvServiceEndedEvent);
            MessageDispatcher.Instance.Unsubscribe<evTripEnded>(this.EvTripEndedEvent);
        }

        /// <summary>
        /// Processes an input.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The new <see cref="State"/> after processing the given input.
        /// </returns>
        protected override State DoProcess(InputAlphabet action)
        {
            return this;
        }

        private void EvTripEndedEvent(object sender, MessageEventArgs<evTripEnded> messageEventArgs)
        {
            this.SetScreen(MainFieldKey.BlockDriveWait);
            this.driveInfo.BlockDrive.IsDriving = false;
        }

        private void EvServiceEndedEvent(object sender, MessageEventArgs<evServiceEnded> messageEventArgs)
        {
            MessageDispatcher.Instance.Unsubscribe<evServiceEnded>(this.EvServiceEndedEvent);

            ////SetScreen(Gorba.Motion.Obc.Terminal.Control.Screens.MFKey.BlockDriveWait);
            if (this.driveInfo.DriveType == DriveType.Block)
            {
                // Context.Process maybe called from
                // actionhandler. so don't call it again....
                this.driveInfo.BlockDrive.IsDriving = false;
                this.Context.Process(InputAlphabet.LogOffDrive);
            }
        }

        private void EvBusStopLeftEvent(object sender, MessageEventArgs<evBUSStopLeft> messageEventArgs)
        {
            this.SetScreen(MainFieldKey.BlockDriving);
        }

        private void EvBusStopReachedEvent(object sender, MessageEventArgs<evBUSStopReached> messageEventArgs)
        {
            ////SetScreen(Gorba.Motion.Obc.Terminal.Control.Screens.MFKey.BlockDriving);
            ////blockDrive.IsDriving = true;
            if (this.driveInfo.BlockDrive.GetNextStop(0) == null)
            {
                this.SetScreen(MainFieldKey.BlockDriveWait);
            }
        }

        private void SetScreen(MainFieldKey screen)
        {
            if (this.rootScreen != screen)
            {
                this.rootScreen = screen;
                this.Context.ShowRootScreen();
            }
        }
    }
}