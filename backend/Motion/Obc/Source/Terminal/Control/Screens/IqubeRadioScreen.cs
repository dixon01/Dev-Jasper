// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IqubeRadioScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IqubeRadioScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Communication;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The iqube radio screen.
    /// </summary>
    internal class IqubeRadioScreen : Screen<IIqubeRadio>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IqubeRadioScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public IqubeRadioScreen(IIqubeRadio mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.MainField.Init(ml.ml_string(115, "Speech Request"));
            this.MainField.InputDone += this.MainFieldInputDoneEvent;
            this.MainField.EscapePressed += this.MainFieldEscapePressed;
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.InputDone -= this.MainFieldInputDoneEvent;
            this.MainField.EscapePressed -= this.MainFieldEscapePressed;
        }

        private void MainFieldEscapePressed(object sender, EventArgs e)
        {
            this.Context.ShowPreviousScreen();
        }

        private void MainFieldInputDoneEvent(object sender, IqubeRadioEventArgs e)
        {
            if (e.Receivers.Length == 0)
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(77, "Information"),
                        ml.ml_string(113, "No receiver is selected"),
                        MessageBoxInfo.MsgType.Info));
            }
            else if (e.Receivers.Length > 4)
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(77, "Information"),
                        ml.ml_string(114, "Too many receivers are selected. Maximum are four receivers."),
                        MessageBoxInfo.MsgType.Info));
            }
            else
            {
                if (this.Context.IraHandler.GetCallState() != IraHandler.IraCallState.None)
                {
                    // If there is already a radio call hang up first
                    MessageDispatcher.Instance.Broadcast(new evSpeechDisconnected(1));
                    Thread.Sleep(1000);
                }

                MessageDispatcher.Instance.Broadcast(new evSpeechRequested(string.Join(string.Empty, e.Receivers)));
                var info = new ProgressBarInfo(ml.ml_string(134, "Send Request"), 5);
                info.ProgressBarElapsed += (s, ev) => this.Context.ShowRootScreen();
                this.ShowProgressBar(info);
            }
        }
    }
}