// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActionHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control
{
    using System;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Commands;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    ///   Handles actions.
    ///   An action is normally started in the menu or by pressing a short key
    /// </summary>
    internal class ActionHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<ActionHandler>();

        private readonly IContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public ActionHandler(IContext context)
        {
            this.context = context;
            context.UiRoot.ShortKeyPressed += this.ActionHandler_ShortKeyEvent;
            context.UiRoot.ButtonBar.ButtonClick += this.ButtonBarOnButtonClick;
        }

        /// <summary>
        ///   Handle the action
        ///   An action is normally started in the menu or short key
        /// </summary>
        /// <param name = "commandName">The command to execute</param>
        public void HandleAction(CommandName commandName)
        {
            this.HandleAction(GetCommand(commandName.ToString()), false);
        }

        private static Command GetCommand(string name)
        {
            try
            {
                return Command.GetCommand(name);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Unable to parse Enum: " + name, ex); // MLHIDE
                return Command.None;
            }
        }

        private void HandleAction(Command command, bool sourceIsMenu)
        {
            Logger.Debug("Handle action: {0}", command.Name); // MLHIDE
            try
            {
                command.Execute(this.context, sourceIsMenu);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Unable to execute command: " + command.Name, ex); // MLHIDE
                this.ShowWarning(ex.Message);
            }
        }

        private void ShowWarning(string reason)
        {
            this.context.Screen.ShowMessageBox(
                new MessageBoxInfo(
                    ml.ml_string(7, "Warning"),
                    ml.ml_string(130, "Configuration fault. ") + reason,
                    MessageBoxInfo.MsgType.Warning));
        }

        private void ButtonBarOnButtonClick(object sender, CommandEventArgs e)
        {
            this.HandleAction(e.Command);
        }

        private void ActionHandler_ShortKeyEvent(object sender, ShortKeyEventArgs e)
        {
            Logger.Debug("Handle short key: {0}", e.ShortKey); // MLHIDE
            var skc = this.context.ConfigHandler.GetShortKeyConfig();
            switch (e.ShortKey)
            {
                case ShortKey.F1Short:
                    this.HandleAction(skc.F1ShortAction.Command);
                    break;
                case ShortKey.F1Long:
                    this.HandleAction(skc.F1LongAction.Command);
                    break;
                case ShortKey.F2Short:
                    this.HandleAction(skc.F2ShortAction.Command);
                    break;
                case ShortKey.F2Long:
                    this.HandleAction(skc.F2LongAction.Command);
                    break;
                case ShortKey.F3Short:
                    this.HandleAction(skc.F3ShortAction.Command);
                    break;
                case ShortKey.F3Long:
                    this.HandleAction(skc.F3LongAction.Command);
                    break;
                case ShortKey.F4Short:
                    this.HandleAction(skc.F4ShortAction.Command);
                    break;
                case ShortKey.F4Long:
                    this.HandleAction(skc.F4LongAction.Command);
                    break;
                case ShortKey.F5Short:
                    this.HandleAction(skc.F5ShortAction.Command);
                    break;
                case ShortKey.F5Long:
                    this.HandleAction(skc.F5LongAction.Command);
                    break;
                case ShortKey.F6Short:
                    this.HandleAction(skc.F6ShortAction.Command);
                    break;
            }
        }
    }
}