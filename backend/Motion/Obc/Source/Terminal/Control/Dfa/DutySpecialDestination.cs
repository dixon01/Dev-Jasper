// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DutySpecialDestination.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DutySpecialDestination type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    /// The duty special destination state.
    /// </summary>
    internal class DutySpecialDestination : State
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DutySpecialDestination"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal DutySpecialDestination(IContext context)
            : base(context)
        {
        }

        /// <summary>
        ///   This method will be called by entering this state.
        ///   Register here to events you want to receive!
        /// </summary>
        internal override void EnterState()
        {
            this.Context.StatusHandler.SaveStatus();
        }

        /// <summary>
        ///   This method will be called by leaving this state.
        ///   UNREGISTER ALL EVENTS you have registered in the EnterState() method!!!
        /// </summary>
        internal override void LeaveState()
        {
            this.Context.StatusHandler.DeleteSavedStatus();
        }

        /// <summary>
        ///   Gets the Root screen of the state
        /// </summary>
        /// <returns>
        /// The root screen.
        /// </returns>
        internal override MainFieldKey GetRootScreen()
        {
            return MainFieldKey.SpecialDestinationDrive;
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

        /*   internal override Gorba.Motion.Obc.Terminal.Control.Data.IListItem GetMenu()
        {
            return Data.DataCollection.GetDriveMenu();
        }*/

        ////internal override ShortKeyConfig GetShortKeyConfig() { return Data.DataCollection.GetDriveShortKey(); }
    }
}