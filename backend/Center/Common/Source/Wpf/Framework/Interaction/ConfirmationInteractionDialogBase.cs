// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfirmationInteractionDialogBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfirmationInteractionDialogBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;

    /// <summary>
    /// Defines a base dialog for interaction.
    /// </summary>
    public class ConfirmationInteractionDialogBase : InteractionDialogBase
    {
        /// <summary>
        /// Occurs when the dialog is confirmed.
        /// </summary>
        public event EventHandler ConfirmEventHandler;

        /// <summary>
        /// Occurs when the dialog is canceled.
        /// </summary>
        public event EventHandler CancelEventHandler;

        /// <summary>
        /// Oks this instance.
        /// </summary>
        public virtual void Yes()
        {
            this.OnClose(new InteractionEventArgs(InteractionType.Yes));
        }

        /// <summary>
        /// Oks this instance.
        /// </summary>
        public virtual void No()
        {
            this.OnClose(new InteractionEventArgs(InteractionType.No));
        }

        /// <summary>
        /// Oks this instance.
        /// </summary>
        public virtual void Ok()
        {
            this.OnClose(new InteractionEventArgs(InteractionType.Ok));
        }

        /// <summary>
        /// Cancels the dialog.
        /// </summary>
        public virtual void Cancel()
        {
            this.OnClose(new InteractionEventArgs(InteractionType.Cancel));
        }

        private void OnClose(InteractionEventArgs e)
        {
            var handler = (e.Type == InteractionType.Ok) ? this.ConfirmEventHandler : this.CancelEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}