// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDestinationSelectionInputScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialDestinationSelectionInputScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;

    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The special destination selection input screen.
    /// </summary>
    internal class SpecialDestinationSelectionInputScreen : NumberInputScreen
    {
        private readonly SpecialDestinationList specDestinationList;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDestinationSelectionInputScreen"/> class.
        /// </summary>
        /// <param name="specDestinationList">
        /// The special destination list.
        /// </param>
        /// <param name="numberInput">
        /// The number input.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public SpecialDestinationSelectionInputScreen(
            SpecialDestinationList specDestinationList,
            INumberInput numberInput,
            IContext context)
            : base(numberInput, context, 4)
        {
            this.specDestinationList = specDestinationList;
        }

        /// <summary>
        /// Gets the caption/text from this numberInputScreen
        /// </summary>
        /// <returns>
        /// The main caption.
        /// </returns>
        protected override string GetMainCaption()
        {
            return ml.ml_string(85, "Special destination");
        }

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected override string GetTextInputCaption()
        {
            return this.GetMainCaption();
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
            return string.Empty;
        }

        /// <summary>
        /// Will be called when the user has ESC pressed
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void HandleNumberInputEscapeEvent(object sender, EventArgs e)
        {
            // do nothing
        }

        /// <summary>
        /// Handles the number input update event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void HandleNumberInputUpdateEvent(object sender, IndexEventArgs e)
        {
            this.MainField.HintText = this.specDestinationList.GetText(e.Index);
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
            if (this.specDestinationList.ExistServiceNumber(inputNumber.ToString()))
            {
                int destinationCode = this.specDestinationList.GetDestinationCode(inputNumber);
                new SpecialDestinationLoad(this.Context).LoadSpecialDestDrive(
                    inputNumber,
                    destinationCode,
                    this.specDestinationList.GetText(inputNumber),
                    ml.ml_string(140, "Load Special Destination"));
            }
            else
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(86, "Error"),
                        ml.ml_string(88, "The corresponding destination code was not found (extraservices.csv)"),
                        MessageBoxInfo.MsgType.Error));
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