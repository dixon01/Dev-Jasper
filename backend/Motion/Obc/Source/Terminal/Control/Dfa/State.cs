// --------------------------------------------------------------------------------------------------------------------
// <copyright file="State.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the State type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;

    /// <summary>
    /// <para>
    ///   The state of the drivers terminal.
    ///   Note: a State never direct link to an object from type IMainField!!!
    ///   The State is no Visible screen! It's the state from the terminal!
    /// </para>
    /// <para>
    ///   The State is responsible for following tasks:
    ///   1.) Menu
    ///   Each State may has a different menu list. Not all menu options are allowed in a state.
    ///   For example if driver is logged off, he should not be able to activate a redirection
    /// </para>
    /// <para>
    ///   2.) Short keys
    ///   Same idea as Menu. For example if driver not yet logged in or no special or block drive is active,
    ///   an Razzia Short key should be disabled
    ///   (of course also in the menu)
    /// </para>
    /// <para>
    ///   3.) DFA
    ///   A state is the central part of the DFA. Each state needs to listen for the Events from Medi or from system.
    ///   And activate, if necessary the State change. (Through the Context.SetState inside the Process(...) Method.
    ///   A State change is only allowed to do inside the Process method!!!
    ///   Respect the DFA (http://en.wikipedia.org/wiki/Deterministic_finite-state_machine)
    ///   And respect the GoF pattern!!!
    /// </para>
    /// </summary>
    internal abstract class State
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="State"/> class.
        /// </summary>
        /// <param name="context">
        /// The state machine context.
        /// </param>
        internal State(IContext context)
        {
            this.Context = context;

            if (this is DriveSelect)
            {
                context.StatusHandler.DriveInfo.DriveType = DriveType.None;
            }
            else if (this is DutyBlock)
            {
                context.StatusHandler.DriveInfo.DriveType = DriveType.Block;
            }
            else if (this is DutySpecialDestination)
            {
                context.StatusHandler.DriveInfo.DriveType = DriveType.SpecialDestination;
            }
        }

        /// <summary>
        /// Gets the state machine context.
        /// </summary>
        public IContext Context { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// The ProcessActions is the input alphabet from the state machine.
        /// </summary>
        /// <param name = "action">
        /// The input to process.
        /// </param>
        /// <returns>
        /// The new <see cref="State"/> after processing the given input.
        /// </returns>
        internal State Process(InputAlphabet action)
        {
            switch (action)
            {
                case InputAlphabet.LogOffAll:
                    this.Context.StatusHandler.DriveInfo.ClearAll();
                    return new DriveSelect(this.Context);
                case InputAlphabet.LogOffDrive:
                    this.Context.StatusHandler.DriveInfo.ClearDrive();
                    return new DriveSelect(this.Context);
                default:
                    return this.DoProcess(action);
            }
        }

        /// <summary>
        ///   Gets the Root screen of the state
        /// </summary>
        /// <returns>
        /// The root screen.
        /// </returns>
        internal abstract MainFieldKey GetRootScreen();

        /// <summary>
        ///   This method will be called by entering this state.
        ///   Register here to events you want to receive!
        /// </summary>
        internal abstract void EnterState();

        /// <summary>
        ///   This method will be called by leaving this state.
        ///   UNREGISTER ALL EVENTS you have registered in the EnterState() method!!!
        /// </summary>
        internal abstract void LeaveState();

        /*   /// <summary>
        /// Returns the state specific menu list
        /// </summary>
        /// <returns></returns>
        internal abstract IListItem GetMenu();*/

        ////internal abstract ShortKeyConfig GetShortKeyConfig();

        /// <summary>
        /// Processes an input.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The new <see cref="State"/> after processing the given input.
        /// </returns>
        protected abstract State DoProcess(InputAlphabet action);
    }
}