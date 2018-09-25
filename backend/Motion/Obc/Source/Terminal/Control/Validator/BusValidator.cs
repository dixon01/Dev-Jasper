// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusValidator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BusValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.StatusInfo;

    /// <summary>
    /// The bus validator.
    /// </summary>
    internal class BusValidator : Validator
    {
        private const int ErrInvalidBlock = -10;

        private const int ErrNoValidTime = -11;

        private readonly IContext context;

        private readonly int blockNumber;

        private int errorCode = ValidatorHandler.ErrNoAnswer;

        private bool isDone;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusValidator"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="blockNumber">
        /// The block number.
        /// </param>
        public BusValidator(IContext context, int blockNumber)
            : base(20, false)
        {
            this.context = context;
            this.blockNumber = blockNumber;
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
                switch (this.ErrorCode)
                {
                    case ErrInvalidBlock:
                        return ml.ml_string(20, "Invalid block number");
                    case ErrNoValidTime:
                        return ml.ml_string(21, "No valid time (GPS)");
                    default:
                        return base.ErrorDescription;
                }
            }
        }

        /// <summary>
        ///   Starts the validator. This method MUST NOT block!!!
        /// </summary>
        public override void Start()
        {
            this.isDone = false;
            this.errorCode = ValidatorHandler.ErrNoAnswer;

            MessageDispatcher.Instance.Subscribe<evSetServiceAck>(this.EvSetServiceAckEvent);

            if (TimeProvider.Current.Now.Year < 2010)
            {
                this.errorCode = ErrNoValidTime;
                this.isDone = true;
            }

            ////context.GetStatusHandler().GetDriveInfo().ClearDrive();
            MessageDispatcher.Instance.Broadcast(new evSetService(this.blockNumber));
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
        }

        private void EvSetServiceAckEvent(object sender, MessageEventArgs<evSetServiceAck> e)
        {
            MessageDispatcher.Instance.Unsubscribe<evSetServiceAck>(this.EvSetServiceAckEvent);
            if (e.Message.Success)
            {
                DriveInfo di = this.context.StatusHandler.DriveInfo;
                MessageDispatcher.Instance.Broadcast(
                    new evServiceStarted(this.blockNumber, false, di.IsDrivingSchool, di.IsAdditionalDrive));
                ////new DutyEvent().Send(context.GetStatusHandler().GetDriveInfo(),
                ////  evDuty.Types.DutyOnRegular, Gorba.Motion.Obc.Terminal.Control.StatusInfo.DriveType.BlockDrive);
                this.errorCode = ValidatorHandler.ErrNoError;
            }
            else
            {
                this.errorCode = ErrInvalidBlock;
            }

            this.isDone = true;
        }
    }
}