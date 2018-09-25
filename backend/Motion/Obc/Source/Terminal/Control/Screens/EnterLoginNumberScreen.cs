// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterLoginNumberScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterLoginNumberScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The enter login number screen.
    /// </summary>
    internal class EnterLoginNumberScreen : NumberInputBaseScreen<ILoginNumberInput>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnterLoginNumberScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public EnterLoginNumberScreen(ILoginNumberInput mainField, IContext context)
            : base(mainField, context)
        {
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.MainField.LanguageChanged += this.NumberInputLanguageEvent;
            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.LanguageChanged -= this.NumberInputLanguageEvent;
            base.Hide();
        }

        /// <summary>
        /// Initializes the main field.
        /// </summary>
        protected override void InitMainField()
        {
            bool showGerman = false;
            bool showEnglish = false;
            bool showFrench = false;
            TerminalConfig config = this.Context.ConfigHandler.GetConfig();

            // If only the language1 is set, then there's no choice. so don't display any flag...
            if (config.Languages.Value.Count > 1)
            {
                showGerman =
                    config.Languages.Value.Find(item => item.Equals("de", StringComparison.CurrentCultureIgnoreCase))
                    != null; // MLHIDE
                showFrench =
                    config.Languages.Value.Find(item => item.Equals("fr", StringComparison.CurrentCultureIgnoreCase))
                    != null; // MLHIDE
                showEnglish =
                    config.Languages.Value.Find(item => item.Equals("en", StringComparison.CurrentCultureIgnoreCase))
                    != null; // MLHIDE
            }

            this.MainField.Init(
                this.GetMainCaption(),
                this.GetTextInputCaption(),
                6,
                this.GetTextInput2Caption(),
                showGerman,
                showFrench,
                showEnglish);
        }

        /// <summary>
        /// Gets the caption/text from this numberInputScreen
        /// </summary>
        /// <returns>
        /// The main caption.
        /// </returns>
        protected override string GetMainCaption()
        {
            return ml.ml_string(25, "Login");
        }

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected override string GetTextInputCaption()
        {
            return ml.ml_string(27, "Driver number");
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
            return ml.ml_string(28, "Validating driver number");
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
            this.Context.StatusHandler.DriveInfo.DriverNumber = inputNumber;

            var config = this.Context.ConfigHandler.GetConfig();
            if (config.DriverPinCode)
            {
                this.Context.StatusHandler.DriveInfo.DriverPin = inputNumber2;
            }
            else
            {
                this.Context.StatusHandler.DriveInfo.DriverPin = -1; // pas de code pin
            }

            this.Context.ShowRootScreen();
        }

        /// <summary>
        /// Will be called when the user has ESC pressed
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected override void HandleNumberInputEscapeEvent(object sender, EventArgs e)
        {
            if (this.Context.StatusHandler.DriveInfo.DriverNumber != 0)
            {
                Logger.Debug("EscEvent. Handle it"); // MLHIDE
                this.Context.ShowPreviousScreen();
            }
            else
            {
                Logger.Debug("EscEvent. Don't Handle it"); // MLHIDE
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
            if (!this.Context.ConfigHandler.GetConfig().ICenterValidation)
            {
                return null;
            }

            var di = this.Context.StatusHandler.DriveInfo;

            var config = this.Context.ConfigHandler.GetConfig();
            if (!config.DriverPinCode)
            {
                numberToValidate2 = -1; // pas de code pin
            }

            evDuty duty;
            if (di.DriveType == DriveType.Block)
            {
                // When already logged in in block drive -> Driver change
                duty = new evDuty(
                    numberToValidate.ToString(CultureInfo.InvariantCulture),
                    numberToValidate2.ToString(CultureInfo.InvariantCulture),
                    di.BlockNumber.ToString(CultureInfo.InvariantCulture),
                    evDuty.Types.DutyOnRegular,
                    di.EvDutyOptions);
            }
            else
            {
                duty = new evDuty(
                    numberToValidate.ToString(CultureInfo.InvariantCulture),
                    numberToValidate2.ToString(CultureInfo.InvariantCulture),
                    di.RunNumber.ToString(CultureInfo.InvariantCulture),
                    evDuty.Types.DutyOnSpecialService,
                    di.EvDutyOptions);
            }

            int timeToWait = 10;
            if (this.Context.ConfigHandler.GetConfig().SpeechType.Value == SpeechType.Radio)
            {
                timeToWait = 20;
            }

            return new ValidatorHandler(
                numberToValidate,
                numberToValidate2,
                new EvDutyValidator(duty, false, timeToWait));
        }

        private string GetTextInput2Caption()
        {
            TerminalConfig config = this.Context.ConfigHandler.GetConfig();

            // Code PIN
            return config.DriverPinCode ? "Code PIN" : string.Empty;
        }

        private void NumberInputLanguageEvent(object sender, IndexEventArgs e)
        {
            LanguageManager.Instance.CurrentLanguage = LanguageManager.Instance.GetLanguage(e.Index);
            this.Context.ShowMainField(MainFieldKey.DriverLogin);
        }
    }
}