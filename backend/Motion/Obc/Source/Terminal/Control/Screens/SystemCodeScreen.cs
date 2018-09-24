// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemCodeScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemCodeScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The system code screen.
    /// </summary>
    internal class SystemCodeScreen : NumberInputScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCodeScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SystemCodeScreen(INumberInput mainField, IContext context)
            : base(mainField, context, 4)
        {
        }

        /// <summary>
        /// Gets the caption/text from this numberInputScreen
        /// </summary>
        /// <returns>
        /// The main caption.
        /// </returns>
        protected override string GetMainCaption()
        {
            return "System Manager";
        }

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected override string GetTextInputCaption()
        {
            return "Sys Code";
        }

        /// <summary>
        /// Gets a text for the progress bar.
        /// The progress bar will appear when the validator handler will start to work.
        /// </summary>
        /// <returns>
        /// The progress bar text.
        /// </returns>
        protected override string GetProgressBarText()
        {
            return "Validate code...";
        }

        /// <summary>
        /// Will be called when the user has ESC pressed
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void HandleNumberInputEscapeEvent(object sender, EventArgs e)
        {
            this.Context.ShowPreviousScreen();
        }

        /// <summary>
        /// Will be called when validation was successful.
        /// </summary>
        /// <param name="inputNumber">
        /// The first validated input number
        /// </param>
        /// <param name="inputNumber2">
        /// The second input number.
        /// </param>
        protected override void ValidationSuccess(int inputNumber, int inputNumber2)
        {
            if (inputNumber == 1234)
            {
                this.Context.ActionHandler.HandleAction(CommandName.Quit);
            }
            else
            {
                this.ShowMessageBox(
                                    new MessageBoxInfo(
                                        "hello", // MLHIDE
                                        "implement your action. code: " + inputNumber, // MLHIDE
                                        MessageBoxInfo.MsgType.Info));
            }
        }

        /// <summary>
        /// Gets the validator handler.
        /// </summary>
        /// <param name="numberToValidate">
        /// The number to validate.
        /// </param>
        /// <param name="numberToValidate2">
        /// The second number to validate.
        /// </param>
        /// <returns>
        /// The <see cref="ValidatorHandler"/> or null.
        /// </returns>
        protected override ValidatorHandler GetValidatorHandler(int numberToValidate, int numberToValidate2)
        {
            return null;
        }
    }
}