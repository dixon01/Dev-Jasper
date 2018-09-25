// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageListScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageListScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The message list screen.
    /// </summary>
    internal class MessageListScreen : Screen<IMessageList>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public MessageListScreen(IMessageList mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.MainField.Init(
                                ml.ml_string(45, "Received Messages"),
                                this.Context.MessageHandler.GetAllMessages());
            this.MainField.EscapePressed += this.HandleReturnEvent;
            this.MainField.ReturnPressed += this.HandleReturnEvent;
            this.MainField.SelectedIndexChanged += this.ListSelectedItemIndexEvent;
            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.EscapePressed -= this.HandleReturnEvent;
            this.MainField.ReturnPressed -= this.HandleReturnEvent;
            this.MainField.SelectedIndexChanged -= this.ListSelectedItemIndexEvent;
            base.Hide();
        }

        private void HandleReturnEvent(object sender, EventArgs e)
        {
            this.Context.ShowPreviousScreen();
        }

        private void ListSelectedItemIndexEvent(object sender, IndexEventArgs e)
        {
            this.Context.ShowRootScreen();
        }
    }
}