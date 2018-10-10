// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterBlockNumberScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterBlockNumberScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The enter block number screen.
    /// </summary>
    internal class EnterBlockNumberScreen : NumberInputScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnterBlockNumberScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public EnterBlockNumberScreen(INumberInput mainField, IContext context)
            : base(mainField, context, 7) // 7 pour neuchatel
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
            return ml.ml_string(22, "Block drive");
        }

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected override string GetTextInputCaption()
        {
            return ml.ml_string(23, "Block number");
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
            return ml.ml_string(24, "Validating block number");
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
            this.Context.Process(InputAlphabet.BlockSet);
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
            DriveInfo di = this.Context.StatusHandler.DriveInfo;
            var validators = new List<Validator>();

            var duty = new evDuty(
                di.DriverNumber.ToString(CultureInfo.InvariantCulture),
                di.DriverPin.ToString(CultureInfo.InvariantCulture),
                numberToValidate.ToString(CultureInfo.InvariantCulture),
                evDuty.Types.DutyOnRegular,
                di.EvDutyOptions);

            validators.Add(new BusValidator(this.Context, numberToValidate));

            if (this.Context.ConfigHandler.GetConfig().ICenterValidation)
            {
                bool isIra = this.Context.ConfigHandler.GetConfig().SpeechType.Value == SpeechType.Radio;

                validators.Add(new EvDutyValidator(duty, isIra));
            }

            return new ValidatorHandler(numberToValidate, numberToValidate2, validators.ToArray());
        }
    }
}