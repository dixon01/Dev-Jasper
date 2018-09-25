// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvDutyValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvDutyValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    ///   This class is used for login validation.
    ///   It works for IMotion and Iqube.radio. XOR! Both are not allowed.
    /// </summary>
    internal class EvDutyValidator : Validator
    {
        private readonly evDuty duty;

        private readonly bool isIra;

        private int errorCode = ValidatorHandler.ErrNoAnswer;

        private bool isDone;

        private evDutyAck.Acks response = evDutyAck.Acks.UnknownError;

        private ITimer iraDutyCheckTimer;

        private int iraDutyCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvDutyValidator"/> class.
        /// </summary>
        /// <param name = "duty">
        /// The duty event to be broadcast when successful.
        /// </param>
        /// <param name = "isIra">
        /// Only set this to true when iqube.radio is used and if it's the NOT the driver login.
        /// Otherwise too much time (delay) will be used
        /// </param>
        public EvDutyValidator(evDuty duty, bool isIra)
            : this(duty, isIra, 10)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvDutyValidator"/> class.
        /// </summary>
        /// <param name="duty">
        /// The duty event to be broadcast when successful.
        /// </param>
        /// <param name="isIra">
        /// Only set this to true when iqube.radio is used and if it's the NOT the driver login.
        /// Otherwise too much time (delay) will be used
        /// </param>
        /// <param name="maxTime">
        /// The maximum time for this validator to wait.
        /// </param>
        public EvDutyValidator(evDuty duty, bool isIra, int maxTime)
            : base(maxTime, true)
        {
            this.duty = duty;
            this.isIra = isIra;
        }

        /// <summary>
        ///   Gets a value indicating whether the validation is done/finish
        ///   if value is true is only the meaning the validation is done. it's not the meaning that it was successful!
        ///   After calling the method Stop(), IsDone() will return true
        /// </summary>
        public override bool IsDone
        {
            get
            {
                return this.isDone;
            }
        }

        /// <summary>
        ///   Gets the error code.
        /// </summary>
        /// <value>0: No Error</value>
        public override int ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        /// <summary>
        /// Gets an error description. Returns null if GetErrorCode is 0
        /// You should overwrite this method.
        /// If you don't have an error description for an error code, so call this (parent) property
        /// </summary>
        public override string ErrorDescription
        {
            get
            {
                if (this.response == evDutyAck.Acks.OK)
                {
                    return null;
                }

                string errorDescription;
                if (this.TryGetErrorDescription(out errorDescription))
                {
                    return errorDescription;
                }

                return base.ErrorDescription;
            }
        }

        /// <summary>
        ///   Starts the validator. This method MUST NOT block!!!
        /// </summary>
        public override void Start()
        {
            this.isDone = false;
            this.errorCode = ValidatorHandler.ErrNoAnswer;

            if (this.isIra)
            {
                // This is a hack. Because the validators running in parallel. So the Bus validator is started first
                // (See EnterBlockNumberScreen.cs)
                // The problem is until the bus validation is not done, iraControl does not know the Line number
                // So if you not wait the line number in ira will ever be set to L99 (special line). But at a block
                // drive we should use the real line number.
                // For that we need to wait until RemoteEventHandler.CurrentService.Line is set...
                this.iraDutyCounter = 0;
                this.iraDutyCheckTimer = TimerFactory.Current.CreateTimer("IRA_Duty_Check"); // MLHIDE
                this.iraDutyCheckTimer.AutoReset = true;
                this.iraDutyCheckTimer.Interval = TimeSpan.FromSeconds(1);
                this.iraDutyCheckTimer.Elapsed += this.IraDutyCheckTimerOnElapsed;
                this.iraDutyCheckTimer.Enabled = true;
            }
            else
            {
                MessageDispatcher.Instance.Subscribe<evDutyAck>(this.EvDutyAckEvent);
                MessageDispatcher.Instance.Broadcast(this.duty);
            }
        }

        /// <summary>
        ///   Aborts the validator. If validation is already done, an abort should be able to be performed
        ///   You can unsubscribe by the registered events
        /// </summary>
        public override void Stop()
        {
            if (this.isDone == false)
            {
                this.errorCode = ValidatorHandler.ErrNoAnswer;
            }

            this.isDone = true;
            MessageDispatcher.Instance.Unsubscribe<evDutyAck>(this.EvDutyAckEvent);
        }

        private bool TryGetErrorDescription(out string errorDescription)
        {
            if (this.response == evDutyAck.Acks.UnknownError)
            {
                {
                    errorDescription = ml.ml_string(38, "Unkown error (Center)");
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.BlockAlreadyAssigned) == evDutyAck.Acks.BlockAlreadyAssigned)
            {
                {
                    errorDescription = ml.ml_string(30, "Block number already assigned. Block: ") + this.duty.Service;
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.BusAlreadyAssigned) == evDutyAck.Acks.BusAlreadyAssigned)
            {
                {
                    errorDescription = ml.ml_string(31, "Bus already assigned.");
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.DriverAlreadyAssigned) == evDutyAck.Acks.DriverAlreadyAssigned)
            {
                {
                    errorDescription = ml.ml_string(32, "Driver number already assigned. Driver: ")
                                       + this.duty.PersonelId;
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.InvalidBlock) == evDutyAck.Acks.InvalidBlock)
            {
                {
                    errorDescription = ml.ml_string(33, "Block number invalid (Center). Block: ") + this.duty.Service;
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.InvalidDriver) == evDutyAck.Acks.InvalidDriver)
            {
                {
                    errorDescription = ml.ml_string(34, "Driver number is not valid. Driver: ") + this.duty.PersonelId;
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.InvalidVehicle) == evDutyAck.Acks.InvalidVehicle)
            {
                {
                    errorDescription = ml.ml_string(35, "Vehicle number is not valid");
                    return true;
                }
            }

            if ((this.response & evDutyAck.Acks.SystemError) == evDutyAck.Acks.SystemError)
            {
                {
                    errorDescription = ml.ml_string(37, "System unknown error (Center)");
                    return true;
                }
            }

            errorDescription = null;
            return false;
        }

        private void IraDutyCheckTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            // It's just a delay. Because IraControl needs the Line number. There is no line number in the Duty Event.
            // So BusValidator has to be started first.
            // When BusValidator is done, the line number is available in Event handler
            if (RemoteEventHandler.CurrentService == null)
            {
                this.iraDutyCounter++;
                if (this.iraDutyCounter <= 10)
                {
                    return;
                }
            }

            this.iraDutyCheckTimer.Dispose();
            MessageDispatcher.Instance.Subscribe<evDutyAck>(this.EvDutyAckEvent);
            MessageDispatcher.Instance.Broadcast(this.duty);
        }

        private void EvDutyAckEvent(object sender, MessageEventArgs<evDutyAck> messageEventArgs)
        {
            this.response = messageEventArgs.Message.Response;
            if (this.response == evDutyAck.Acks.OK)
            {
                this.errorCode = ValidatorHandler.ErrNoError;
            }
            else
            {
                this.errorCode = (int)this.response;
            }

            this.isDone = true;
        }
    }
}