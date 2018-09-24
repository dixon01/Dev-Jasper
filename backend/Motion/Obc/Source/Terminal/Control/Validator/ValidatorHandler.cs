// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValidatorHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The ValidatorHandler
    /// You can use this for all common validations.
    /// Special it's needed Validate with i.Motion, Bus.exe and iqube.radio.
    /// All Added Validator Objects will be started in parallel.
    /// So the validation for each type (i.Motion, Bus.exe,...) will run in parallel.
    /// </summary>
    internal class ValidatorHandler
    {
        /// <summary>
        /// The no error code.
        /// </summary>
        public const int ErrNoError = 0;

        /// <summary>
        /// The unknown error code.
        /// </summary>
        public const int ErrUnknown = -1;

        /// <summary>
        /// The no answer error code.
        /// </summary>
        public const int ErrNoAnswer = -2;

        private static readonly Logger Logger = LogHelper.GetLogger<ValidatorHandler>();

        private readonly int numberToValidate;

        private readonly int numberToValidate2;

        private readonly Validator[] validators;

        private int maxTimeToWaitMs;

        private ITimer timer;

        private int timerCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorHandler"/> class.
        /// A list of validators which will be started in parallel.
        /// </summary>
        /// <param name="numberToValidate">
        /// The number to validate.
        /// </param>
        /// <param name="numberToValidate2">
        /// The number to validate 2.
        /// </param>
        /// <param name="validators">
        /// The validators.
        /// </param>
        public ValidatorHandler(int numberToValidate, int numberToValidate2, params Validator[] validators)
        {
            this.validators = validators;
            this.CalcMaxTimeToWait();
            this.numberToValidate = numberToValidate;
            this.numberToValidate2 = numberToValidate2;
        }

        /// <summary>
        /// The validation done event.
        /// </summary>
        internal event EventHandler<ValidationDoneEventArgs> ValidationDone;

        /// <summary>
        /// Gets the max time in seconds.
        /// </summary>
        internal int MaxTimeSec
        {
            get
            {
                return this.maxTimeToWaitMs / 1000;
            }
        }

        /// <summary>
        /// Starts all validators.
        /// </summary>
        public void Start()
        {
            foreach (var v in this.validators)
            {
                v.Start();
            }

            if (this.timer == null)
            {
                this.timer = TimerFactory.Current.CreateTimer("Validators");
                this.timer.Elapsed += this.TimerOnElapsed;
                this.timer.AutoReset = true;
                this.timer.Interval = TimeSpan.FromSeconds(1);
            }

            this.timer.Enabled = true;
        }

        private void CalcMaxTimeToWait()
        {
            foreach (var v in this.validators)
            {
                if (v.MaxTimeSec * 1000 > this.maxTimeToWaitMs)
                {
                    this.maxTimeToWaitMs = v.MaxTimeSec * 1000;
                }
            }
        }

        private void Stop()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            foreach (var v in this.validators)
            {
                v.Stop();
            }
        }

        private void RaiseValidationDone(int errCode, string errDescription)
        {
            var handler = this.ValidationDone;
            if (handler != null)
            {
                handler(
                    this,
                    new ValidationDoneEventArgs(
                        this.numberToValidate, this.numberToValidate2, errCode, errDescription));
            }
        }

        private void TimerOnElapsed(object sender, EventArgs eventArgs)
        {
            this.timerCounter++;
            bool allDone = true;
            try
            {
                foreach (var v in this.validators)
                {
                    if (v.ValidationState == ValidationState.Running)
                    {
                        allDone = false;
                    }
                    else if (v.ValidationState == ValidationState.Failed)
                    {
                        // if one validation fails, immediatly stop validation
                        // because user needs to wait too long for the other validators
                        this.Stop();
                        this.RaiseValidationDone(v.ErrorCode, v.ErrorDescription);
                        return;
                    }
                }

                if (allDone || (this.timerCounter * 1000 > this.maxTimeToWaitMs))
                {
                    // All done or Maxtime exceeded
                    this.Stop();
                    foreach (var v in this.validators)
                    {
                        if (v.ValidationState == ValidationState.Failed)
                        {
                            this.RaiseValidationDone(v.ErrorCode, v.ErrorDescription);
                            return;
                        }
                    }

                    this.RaiseValidationDone(ErrNoError, null);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't check validation", ex);
            }
        }
    }
}