// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterDriverBlockNumberScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterDriverBlockNumberScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Obc.Common;
    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;
    using Gorba.Motion.Obc.Terminal.Control.Validator;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The enter driver block number screen.
    /// </summary>
    internal class EnterDriverBlockNumberScreen : Screen<IDriverBlockInput>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<EnterDriverBlockNumberScreen>();

        private ValidatorHandler validatorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnterDriverBlockNumberScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public EnterDriverBlockNumberScreen(IDriverBlockInput mainField, IContext context)
            : base(mainField, context)
        {
            mainField.InputDone += this.MainFieldOnInputDone;
            mainField.InputUpdate += this.MainFieldOnInputUpdate;
        }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public override void Show()
        {
            TerminalConfig config = this.Context.ConfigHandler.GetConfig();

            this.MainField.Init(
                string.Empty,
                string.Empty,
                0,
                CheckPopulated(config.DB_Btn1),
                CheckPopulated(config.DB_Btn2),
                CheckPopulated(config.DB_Btn3),
                CheckPopulated(config.DB_Btn4),
                CheckPopulated(config.DB_Btn5),
                CheckPopulated(config.DB_Btn6),
                CheckPopulated(config.DB_Btn7),
                CheckPopulated(config.DB_Btn8),
                config.DB_Btn1ShortName,
                config.DB_Btn2ShortName,
                config.DB_Btn3ShortName,
                config.DB_Btn4ShortName,
                config.DB_Btn5ShortName,
                config.DB_Btn6ShortName,
                config.DB_Btn7ShortName,
                config.DB_Btn8ShortName);

            this.MainField.EscapePressed += this.InputEscapeEvent;

            base.Show();
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public override void Hide()
        {
            this.MainField.EscapePressed -= this.InputEscapeEvent;

            if (this.validatorHandler != null)
            {
                this.validatorHandler.ValidationDone -= this.ValidatorValidationDoneEvent;
                this.validatorHandler = null;
            }

            base.Hide();
        }

        private static bool StartWithValue(string value)
        {
            List<string> ldb = DriverBlocks.LoadAllByDayKind(RemoteEventHandler.VehicleConfig.DayKind);

            foreach (string str in ldb)
            {
                if (str.StartsWith(value))
                {
                    return true;
                }
            }

            return false;
        }

        private static string CheckPopulated(ConfigItem<string> configItem)
        {
            // Renvoi la valeur du bouton ou "" si aucun service ne correspond
            if (configItem == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(configItem.Value))
            {
                return string.Empty;
            }

            return StartWithValue(configItem.Value) ? configItem.Value : string.Empty;
        }

        /// <summary>
        /// Gets the validator handler.
        /// </summary>
        /// <param name="serviceAgent">
        /// The service agent.
        /// </param>
        /// <returns>
        /// The <see cref="ValidatorHandler"/>.
        /// </returns>
        private ValidatorHandler GetValidatorHandler(string serviceAgent)
        {
            DriveInfo di = this.Context.StatusHandler.DriveInfo;
            var validators = new List<Validator>();

            var duty = new evDuty(
                di.DriverNumber.ToString(CultureInfo.InvariantCulture),
                di.DriverPin.ToString(CultureInfo.InvariantCulture),
                serviceAgent,
                evDuty.Types.DutyOnDriver,
                di.EvDutyOptions);

            ////if (this.Context.ConfigHandler.GetConfig().ICenterVlidation)
            ////{
            validators.Add(new EvDutyValidator(duty, false));
            //// }
            validators.Add(new BusValidator(this.Context, -9));

            return new ValidatorHandler(0, 0, validators.ToArray());
        }

        private void ValidationSuccess()
        {
            DriveInfo di = this.Context.StatusHandler.DriveInfo;
            di.DriverBlock = this.MainField.Block;
            this.Context.Process(InputAlphabet.BlockSet);
        }

        private void InputEscapeEvent(object sender, EventArgs e)
        {
            this.Context.ShowPreviousScreen();
        }

        private void MainFieldOnInputUpdate(object sender, IndexEventArgs indexEventArgs)
        {
            if (!this.Context.ConfigHandler.GetConfig().DriverBlock_AutoCompletion)
            {
                return;
            }

            var screen =
                this.Context.MainFieldHandler.GetScreen<BlockAutoCompletionScreen>(MainFieldKey.BlockAutoCompletion);
            screen.SetCurrentText(indexEventArgs.Index.ToString(CultureInfo.InvariantCulture));
            this.Context.ShowMainField(MainFieldKey.BlockAutoCompletion);
        }

        private void MainFieldOnInputDone(object sender, NumberInputEventArgs e)
        {
            Logger.Debug("Input done: {0}", this.MainField.Block); // MLHIDE
            if (this.validatorHandler != null)
            {
                this.validatorHandler.ValidationDone -= this.ValidatorValidationDoneEvent;
            }

            this.validatorHandler = this.GetValidatorHandler(this.MainField.Block);
            if (this.validatorHandler == null)
            {
                this.ValidationSuccess();
            }
            else
            {
                this.validatorHandler.ValidationDone += this.ValidatorValidationDoneEvent;
                this.validatorHandler.Start();
                if (this.validatorHandler.MaxTimeSec > 1)
                {
                    // if validation need more than 1 second -> show a progress bar
                    this.ShowProgressBar(
                        new ProgressBarInfo(
                            ml.ml_string(24, "Validating block number"),
                            this.validatorHandler.MaxTimeSec));
                }
            }
        }

        private void ValidatorValidationDoneEvent(object sender, ValidationDoneEventArgs e)
        {
            this.HideProgressBar();
            ////System.Threading.Thread.Sleep(100);
            if (e.ErrorCode == ValidatorHandler.ErrNoError)
            {
                this.ValidationSuccess();
            }
            else
            {
                this.ShowMessageBox(
                    new MessageBoxInfo(
                        ml.ml_string(74, "Verification failed"), e.Error, MessageBoxInfo.MsgType.Warning));
            }
        }
    }
}