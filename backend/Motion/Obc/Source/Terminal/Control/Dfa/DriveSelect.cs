// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriveSelect.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriveSelect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    /// The drive selection state.
    /// </summary>
    internal class DriveSelect : State
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriveSelect"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal DriveSelect(IContext context)
            : base(context)
        {
        }

        /// <summary>
        ///   This method will be called by entering this state.
        ///   Register here to events you want to receive!
        /// </summary>
        internal override void EnterState()
        {
            this.Context.MessageHandler.SetDestinationText(null);
        }

        /// <summary>
        ///   This method will be called by leaving this state.
        ///   UNREGISTER ALL EVENTS you have registered in the EnterState() method!!!
        /// </summary>
        internal override void LeaveState()
        {
        }

        /// <summary>
        ///   Gets the Root screen of the state
        /// </summary>
        /// <returns>
        /// The root screen.
        /// </returns>
        internal override MainFieldKey GetRootScreen()
        {
            if (this.Context.ConfigHandler.GetConfig().ThirdPartyPS)
            {
                return MainFieldKey.BlockDriveWait; // Billetique ERG
            }

            return this.Context.StatusHandler.DriveInfo.DriverNumber != 0
                       ? MainFieldKey.DriveSelect
                       : MainFieldKey.DriverLogin;
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
            switch (action)
            {
                case InputAlphabet.SpecialDestSet:
                    return new DutySpecialDestination(this.Context);
                case InputAlphabet.BlockSet:
                    return new DutyBlock(this.Context);
                default:
                    return this;
            }
        }
    }
}