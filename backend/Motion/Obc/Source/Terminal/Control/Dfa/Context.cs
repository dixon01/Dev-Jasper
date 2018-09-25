// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Context.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Context type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.DFA
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Alarm;
    using Gorba.Motion.Obc.Terminal.Control.Communication;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.Handlers;
    using Gorba.Motion.Obc.Terminal.Control.Message;
    using Gorba.Motion.Obc.Terminal.Control.Screens;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    ///   It's the States Context (GoF Pattern)
    ///   The context from the state machine.
    ///   Do not publish this class to outside! Only State should have access to it.
    ///   The rest should only access to this class through the IContext interface.
    /// </summary>
    internal class Context : IContext
    {
        private static readonly Logger Logger = LogHelper.GetLogger<Context>();

        /// <summary>
        ///   Stack with the "Screens" (History). The top of the stack is the visible screen.
        ///   In a state change, the stack will be cleared.
        ///   A new screen will be set on top of the sack (push).
        ///   When user press ESC, the stack will be popped and the previous screen will appear.
        /// </summary>
        private Stack<MainFieldKey> screenStack;

        private volatile State state;

        private StateVisualizationHandler stateVisualizationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        public Context()
        {
            this.ConfigHandler = new ConfigHandler();
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public State State
        {
            get
            {
                return this.state;
            }

            private set
            {
                lock (this)
                {
                    if (this.state == value)
                    {
                        return;
                    }

                    try
                    {
                        Logger.Info("State change: {0} -> {1}", this.state, value); // MLHIDE
                        if (this.state != null)
                        {
                            this.state.LeaveState();
                        }

                        this.state = value;
                        this.state.EnterState();
                        this.ShowRootScreen();
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorException("Couldn't change state", ex);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets the current screen.
        /// </summary>
        public IScreen Screen
        {
            get
            {
                return this.MainFieldHandler.CurrentScreen;
            }
        }

        /// <summary>
        ///   Gets the Status handler. This object represent the status from the bus(drive)
        /// </summary>
        public StatusHandler StatusHandler { get; private set; }

        /// <summary>
        /// Gets the ActionHandler. The ActionHandler handles Action types.
        /// Actions are normally used in the menu and short keys.
        /// </summary>
        public ActionHandler ActionHandler { get; private set; }

        /// <summary>
        ///   Gets the current config
        /// </summary>
        public ConfigHandler ConfigHandler { get; private set; }

        /// <summary>
        /// Gets the message handler.
        /// </summary>
        public MessageHandler MessageHandler { get; private set; }

        /// <summary>
        /// Gets a AlarmHandler. Take care this value can be null!
        /// </summary>
        public AlarmHandler AlarmHandler { get; private set; }

        /// <summary>
        /// Gets the main field handler.
        /// </summary>
        public MainFieldHandler MainFieldHandler { get; private set; }

        /// <summary>
        /// Gets an IraHandler. Take care this value can be null!
        /// </summary>
        public IraHandler IraHandler { get; private set; }

        /// <summary>
        ///   Gets the WANManager. Take care this value can be null!
        /// </summary>
        public WanManager WanManager { get; private set; }

        /// <summary>
        /// Gets the root object of the UI.
        /// </summary>
        public IUiRoot UiRoot { get; private set; }

        /// <summary>
        /// The result of calling this method could be a state change from the State machine.
        /// Only through this method it's possible to change a state
        /// </summary>
        /// <param name = "action">a character from the input alphabet</param>
        public void Process(InputAlphabet action)
        {
            try
            {
                Logger.Debug("Context.Process: {0}", action); // MLHIDE
                this.State = this.state.Process(action);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't process action " + action, ex);
            }
        }

        /// <summary>
        ///   Lets show a main field
        /// </summary>
        /// <param name = "key">key from the screen to be shown</param>
        public void ShowMainField(MainFieldKey key)
        {
            lock (this)
            {
                Logger.Debug("Context ShowMainField: {0}", key); // MLHIDE

                if (this.MainFieldHandler.SetMainField(key))
                {
                    this.PutOnStack(key);
                }
            }
        }

        /// <summary>
        ///   Shows the root screen (In the state machine it's the screen which is referenced from the initial)
        /// </summary>
        public void ShowRootScreen()
        {
            lock (this)
            {
                this.screenStack.Clear();
                this.MainFieldHandler.SetMainField(this.state.GetRootScreen());
            }
        }

        /// <summary>
        ///   Lets show the previous main field. This is normally activated when user press the ESC button
        /// </summary>
        public void ShowPreviousScreen()
        {
            lock (this)
            {
                Logger.Debug("Context ShowPreviousScreen"); // MLHIDE
                if (this.screenStack.Count > 1)
                {
                    this.screenStack.Pop();
                    this.ShowMainField(this.screenStack.Peek());
                }
                else
                {
                    Logger.Debug("Context ShowPreviousScreen. Already in stack root"); // MLHIDE
                    this.ShowMainField(this.state.GetRootScreen());
                }
            }
        }

        /// <summary>
        ///   Lets show the previous main field. This is normally activated when user press the ESC button
        /// </summary>
        public void Show2PreviousScreen()
        {
            lock (this)
            {
                Logger.Debug("Context Show2PreviousScreen"); // MLHIDE
                if (this.screenStack.Count > 2)
                {
                    this.screenStack.Pop();
                    this.screenStack.Pop();
                    this.ShowMainField(this.screenStack.Peek());
                }
                else if (this.screenStack.Count == 2)
                {
                    this.screenStack.Pop();
                    this.ShowMainField(this.screenStack.Peek());
                }
                else
                {
                    Logger.Debug("TC", "Context ShowPreviousScreen. Already in stack root"); // MLHIDE
                    this.ShowMainField(this.state.GetRootScreen());
                }
            }
        }

        /// <summary>
        /// Starts this context by creating all necessary objects.
        /// </summary>
        public void Start()
        {
            ////this.ledHandler.LedBlink(LedName.SystemStatus);
            this.screenStack = new Stack<MainFieldKey>();

            this.UiRoot = UiFactory.Instance.CreateRoot();
            this.UiRoot.IconBar.ContextIconClick += this.ContextIconClickedEvent;

            this.StatusHandler = new StatusHandler(this);

            var config = this.ConfigHandler.GetConfig();
            if (config.SpeechType.Value == SpeechType.Radio)
            {
                this.IraHandler = new IraHandler(this);
            }

            this.stateVisualizationHandler = new StateVisualizationHandler(this);

            if (config.HandleGprs.Value)
            {
                this.WanManager = new WanManager(this.stateVisualizationHandler);
                this.WanManager.Start(config.GsmConfig);
            }

            if (!string.IsNullOrEmpty(config.AlarmInput))
            {
                this.AlarmHandler = new AlarmHandler(
                    config.AlarmInput, this.WanManager);
            }

            this.MessageHandler = new MessageHandler(this);

            this.MainFieldHandler = new MainFieldHandler(this);

            this.UiRoot.StatusField.Click += this.ContextStatusClickedEvent;

            // set the default language
            LanguageManager.Instance.CurrentLanguage =
                LanguageManager.Instance.GetLanguage(this.ConfigHandler.GetConfig().Languages.Value[0]);

            this.State = new DriveSelect(this); // Sets the initial state

            this.StatusHandler.LoadSavedStatus();

            this.ActionHandler = new ActionHandler(this);
            MessageDispatcher.Instance.Subscribe<evDuty>(this.EvDutyEvent);
            MessageDispatcher.Instance.Subscribe<evMaintenance>(this.EvMaintenanceEvent);

            ////this.ledHandler.LedOn(LedName.SystemStatus);
        }

        private void EvMaintenanceEvent(object sender, MessageEventArgs<evMaintenance> e)
        {
            if (e.Message.Type == evMaintenance.Types.InitDuty)
            {
                this.Process(InputAlphabet.LogOffAll);
                this.Screen.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(13, "Logged off"),
                        ml.ml_string(14, "Log off event received from center."),
                        MessageBoxInfo.MsgType.Info));
            }
        }

        private void EvDutyEvent(object sender, MessageEventArgs<evDuty> e)
        {
            try
            {
                if ((e.Message.OptionEnum == evDuty.Options.Forced) || (e.Message.OptionEnum == evDuty.Options.System))
                {
                    MessageDispatcher.Instance.Broadcast(
                        new evDuty(e.Message.PersonelId, "-1", e.Message.Service, e.Message.Type, evDuty.Options.None));
                    this.Screen.HideMessageBox();
                    this.Screen.HideProgressBar();
                    Logger.Debug("evDuty from i.Motion received. Type: {0}", e.Message.Type); // MLHIDE
                    /* if (this.state is DFA.DriveSelect == false)
                    {
                        this.Process(InputAlphabet.LogOffAll);
                    }*/

                    switch (e.Message.Type)
                    {
                        case evDuty.Types.DutyOff:
                            // new evDuty(aPersonelId, aService, aType, short.MaxValue, evDuty.Options.None).Post();
                            this.Process(InputAlphabet.LogOffAll);
                            this.Screen.ShowMessageBox(
                                new MessageBoxInfo(
                                    ml.ml_string(13, "Logged off"),
                                    ml.ml_string(14, "Log off event received from center."),
                                    MessageBoxInfo.MsgType.Info));
                            break;
                        case evDuty.Types.DutyOnRegular:
                            ////  new evDuty(aPersonelId, aService, aType, short.MaxValue, evDuty.Options.None).Post();
                            this.StatusHandler.DriveInfo.DriverNumber = int.Parse(e.Message.PersonelId);
                            new BlockDriveLoad(this).LoadBlockDrive(
                                int.Parse(e.Message.Service),
                                ml.ml_string(141, "Forced duty on from Center"));
                            break;
                        case evDuty.Types.DutyOnSpecialService:
                            // Not necessary to send it. it will be sent in the SpecialDestinationLoad:
                            ////new evDuty(aPersonelId, aService, aType, short.MaxValue, evDuty.Options.None).Post();
                            this.StatusHandler.DriveInfo.DriverNumber = int.Parse(e.Message.PersonelId);
                            var destList = new SpecialDestinationList(ConfigPaths.ExtraServiceCsv);
                            string destName = destList.GetText(e.Message.Service);
                            if (string.IsNullOrEmpty(destName))
                            {
                                destName = ml.ml_string(142, "Special drive. ID: ") + e.Message.Service;
                            }

                            new SpecialDestinationLoad(this).LoadSpecialDestDrive(
                                int.Parse(e.Message.Service),
                                int.Parse(e.Message.Service),
                                destName,
                                ml.ml_string(141, "Forced duty on from Center"));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't handle duty event", ex);
            }
        }

        private void ContextIconClickedEvent(object sender, EventArgs e)
        {
            if (this.screenStack.Count > 0)
            {
                if (this.screenStack.Peek() == MainFieldKey.Menu)
                {
                    this.ShowPreviousScreen();
                    return;
                }
            }

            this.ShowMainField(MainFieldKey.Menu);
        }

        private void ContextStatusClickedEvent(object sender, EventArgs e)
        {
            if (this.screenStack.Count > 0)
            {
                if (this.screenStack.Peek() != MainFieldKey.Status)
                {
                    this.ShowMainField(MainFieldKey.Status);
                }
                else
                {
                    this.ShowPreviousScreen();
                }
            }
            else
            {
                this.ShowMainField(MainFieldKey.Status);
            }
        }

        private void PutOnStack(MainFieldKey key)
        {
            lock (this)
            {
                while (this.screenStack.Contains(key))
                {
                    this.screenStack.Pop();
                }

                this.screenStack.Push(key);
            }
        }
    }
}