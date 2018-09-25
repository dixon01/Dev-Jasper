// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using Gorba.Motion.Obc.Terminal.Control.Alarm;
    using Gorba.Motion.Obc.Terminal.Control.Communication;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.Message;
    using Gorba.Motion.Obc.Terminal.Control.Screens;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    ///   The DFA input alphabet interface
    ///   See http://en.wikipedia.org/wiki/Deterministic_finite-state_machine for definition
    /// </summary>
    internal interface IContext
    {
        /// <summary>
        ///   Gets the Status handler. This object represent the status from the bus(drive)
        /// </summary>
        StatusHandler StatusHandler { get; }

        /// <summary>
        /// Gets the ActionHandler. The ActionHandler handles Action types.
        /// Actions are normally used in the menu and short keys.
        /// </summary>
        ActionHandler ActionHandler { get; }

        /// <summary>
        /// Gets a AlarmHandler. Take care this value can be null!
        /// </summary>
        AlarmHandler AlarmHandler { get; }

        /// <summary>
        /// Gets an IraHandler. Take care this value can be null!
        /// </summary>
        IraHandler IraHandler { get; }

        /// <summary>
        /// Gets the main field handler.
        /// </summary>
        MainFieldHandler MainFieldHandler { get; }

        /// <summary>
        ///   Gets the current screen.
        /// </summary>
        IScreen Screen { get; }

        /// <summary>
        ///   Gets the WANManager. Take care this value can be null!
        /// </summary>
        WanManager WanManager { get; }

        /// <summary>
        ///   Gets the current config
        /// </summary>
        ConfigHandler ConfigHandler { get; }

        /// <summary>
        /// Gets the root object of the UI.
        /// </summary>
        IUiRoot UiRoot { get; }

        /// <summary>
        /// Gets the message handler.
        /// </summary>
        MessageHandler MessageHandler { get; }

        /// <summary>
        /// The result of calling this method could be a state change from the State machine.
        /// Only through this method it's possible to change a state
        /// </summary>
        /// <param name = "action">a character from the input alphabet</param>
        void Process(InputAlphabet action);

        /// <summary>
        ///   Lets show a main field
        /// </summary>
        /// <param name = "key">key from the screen to be shown</param>
        void ShowMainField(MainFieldKey key);

        /// <summary>
        ///   Lets show the previous main field. This is normally activated when user press the ESC button
        /// </summary>
        void ShowPreviousScreen();

        /// <summary>
        ///   Lets show the previous previous main field.
        /// </summary>
        void Show2PreviousScreen();

        /// <summary>
        ///   Shows the root screen
        /// In the state machine it's the screen which is referenced from the initial
        /// </summary>
        void ShowRootScreen();
    }
}