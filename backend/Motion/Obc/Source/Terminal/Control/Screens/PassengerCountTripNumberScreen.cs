// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassengerCountTripNumberScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PassengerCountTripNumberScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The passenger count trip number screen.
    /// </summary>
    internal class PassengerCountTripNumberScreen : NumberInputScreen
    {
        private int passengerCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassengerCountTripNumberScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public PassengerCountTripNumberScreen(INumberInput mainField, IContext context)
            : base(mainField, context, 4)
        {
        }

        /// <summary>
        /// Sets the passenger count.
        /// </summary>
        /// <param name="count">
        /// The passenger count.
        /// </param>
        public void SetPassengerCount(int count)
        {
            this.passengerCount = count;
        }

        /// <summary>
        /// Gets the caption/text from this numberInputScreen
        /// </summary>
        /// <returns>
        /// The main caption.
        /// </returns>
        protected override string GetMainCaption()
        {
            return ml.ml_string(250, "Trip number");
        }

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected override string GetTextInputCaption()
        {
            return ml.ml_string(250, "Trip number");
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
            return ml.ml_string(152, "Processing...");
        }

        /// <summary>
        /// Will be called when the user has ESC pressed
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void HandleNumberInputEscapeEvent(object sender, EventArgs e)
        {
            this.Context.ShowMainField(MainFieldKey.Menu);
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
            var drive = this.Context.StatusHandler.DriveInfo;
            MessageDispatcher.Instance.Broadcast(
                new evPassengerCount(
                    drive.LineNumber,
                    drive.BlockNumber,
                    inputNumber,
                    drive.StopIdCurrent,
                    drive.DriverNumber,
                    this.passengerCount));

            var message = string.Format(
                ml.ml_string(255, "Passenger count: {0}, Trip number: {1} was submitted"),
                this.passengerCount,
                inputNumber);
            var info = new MessageBoxInfo(ml.ml_string(250, "Trip number"), message, MessageBoxInfo.MsgType.Info);
            info.Closed += (s, e) => this.Context.Show2PreviousScreen();
            this.ShowMessageBox(info);
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