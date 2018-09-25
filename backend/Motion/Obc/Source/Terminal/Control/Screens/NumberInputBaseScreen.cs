// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInputBaseScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInputBaseScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// Base class for all number input screens.
    /// </summary>
    /// <typeparam name="TNumberInputBase">
    /// The type of input.
    /// </typeparam>
    internal abstract class NumberInputBaseScreen<TNumberInputBase> : Screen<TNumberInputBase>
        where TNumberInputBase : INumberInputBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private ValidatorHandler validatorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInputBaseScreen{TNumberInputBase}"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected NumberInputBaseScreen(TNumberInputBase mainField, IContext context)
            : base(mainField, context)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            this.InitMainField();

            this.MainField.InputUpdate += this.HandleNumberInputUpdateEvent;
            this.MainField.InputDone += this.NumberInputInputDoneEvent;
            this.MainField.EscapePressed += this.HandleNumberInputEscapeEvent;
            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.InputUpdate -= this.HandleNumberInputUpdateEvent;
            this.MainField.InputDone -= this.NumberInputInputDoneEvent;
            this.MainField.EscapePressed -= this.HandleNumberInputEscapeEvent;
            if (this.validatorHandler != null)
            {
                this.validatorHandler.ValidationDone -= this.ValidatorValidationDoneEvent;
                this.validatorHandler = null;
            }

            base.Hide();
        }

        /// <summary>
        /// Initializes the main field.
        /// </summary>
        protected abstract void InitMainField();

        /// <summary>
        /// Gets the caption/text from this numberInputScreen
        /// </summary>
        /// <returns>
        /// The main caption.
        /// </returns>
        protected abstract string GetMainCaption();

        /// <summary>
        /// Gets the text which will appear direct above the number input box
        /// </summary>
        /// <returns>
        /// The input caption.
        /// </returns>
        protected abstract string GetTextInputCaption();

        /// <summary>
        /// Gets a text for the progress bar.
        /// The progress bar will appear when the validator handler will start to work.
        /// </summary>
        /// <returns>
        /// The progress bar text.
        /// </returns>
        protected abstract string GetProgressBarText();

        /// <summary>
        /// Will be called when the user has ESC pressed
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void HandleNumberInputEscapeEvent(object sender, EventArgs e);

        /// <summary>
        /// Will be called when validation was successful.
        /// </summary>
        /// <param name="inputNumber">
        /// The first validated input number
        /// </param>
        /// <param name="inputNumber2">
        /// The second input number.
        /// </param>
        protected abstract void ValidationSuccess(int inputNumber, int inputNumber2);

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
        protected abstract ValidatorHandler GetValidatorHandler(int numberToValidate, int numberToValidate2);

        /// <summary>
        /// Handles the number input update event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void HandleNumberInputUpdateEvent(object sender, IndexEventArgs e)
        {
        }

        private void NumberInputInputDoneEvent(object sender, NumberInputEventArgs e)
        {
            this.Logger.Debug("input done: {0}, {1}", e.Input1, e.Input2); // MLHIDE
            if (this.validatorHandler != null)
            {
                this.validatorHandler.ValidationDone -= this.ValidatorValidationDoneEvent;
            }

            this.validatorHandler = this.GetValidatorHandler(e.Input1, e.Input2);
            if (this.validatorHandler == null)
            {
                this.ValidationSuccess(e.Input1, e.Input2);
            }
            else
            {
                this.validatorHandler.ValidationDone += this.ValidatorValidationDoneEvent;
                this.validatorHandler.Start();
                if (this.validatorHandler.MaxTimeSec > 1)
                {
                    // if validation need more than 1 second -> show a progress bar
                    this.ShowProgressBar(
                        new ProgressBarInfo(this.GetProgressBarText(), this.validatorHandler.MaxTimeSec));
                }
            }
        }

        private void ValidatorValidationDoneEvent(object sender, ValidationDoneEventArgs e)
        {
            this.HideProgressBar();
            ////System.Threading.Thread.Sleep(100);
            if (e.ErrorCode == ValidatorHandler.ErrNoError)
            {
                this.ValidationSuccess(e.ValidatedInput, e.ValidatedInput2);
                return;
            }

            this.ShowMessageBox(
                new MessageBoxInfo(ml.ml_string(74, "Verification failed"), e.Error, MessageBoxInfo.MsgType.Warning));
        }
    }
}