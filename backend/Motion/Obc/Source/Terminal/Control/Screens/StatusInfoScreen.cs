// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusInfoScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StatusInfoScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The status info screen.
    /// </summary>
    internal class StatusInfoScreen : Screen<IStatusMainField>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfoScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public StatusInfoScreen(IStatusMainField mainField, IContext context)
            : base(mainField, context)
        {
            this.MainField.EscapePressed += this.HandleReturnEvent;
            this.MainField.ReturnPressed += this.HandleReturnEvent;
        }

        private void HandleReturnEvent(object sender, EventArgs e)
        {
            this.Context.ShowPreviousScreen();
        }
    }
}