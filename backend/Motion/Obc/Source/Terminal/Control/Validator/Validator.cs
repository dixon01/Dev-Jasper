// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Validator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Validator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    /// <summary>
    /// Base class for all data validators.
    /// </summary>
    internal abstract class Validator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Validator"/> class.
        /// </summary>
        /// <param name = "maxTimeSec">
        /// The maximum time in seconds.
        /// </param>
        /// <param name = "noAnswerAllowed">
        /// Set this to true if you want that the Validator will return success in case of a time out
        /// <br></br>This may be useful if you are not sure for network problems and so on
        /// </param>
        protected Validator(int maxTimeSec, bool noAnswerAllowed)
        {
            this.MaxTimeSec = maxTimeSec;
            this.NoAnswerAllowed = noAnswerAllowed;
        }

        /// <summary>
        /// Gets the maximum time for this validator (total seconds)
        /// </summary>
        public int MaxTimeSec { get; private set; }

        /// <summary>
        ///   Gets the current state of this Validator
        /// </summary>
        /// <value></value>
        public ValidationState ValidationState
        {
            get
            {
                if (this.IsDone == false)
                {
                    return ValidationState.Running;
                }

                if ((this.ErrorCode == ValidatorHandler.ErrNoAnswer && this.NoAnswerAllowed) ||
                    (this.ErrorCode == ValidatorHandler.ErrNoError))
                {
                    return ValidationState.Success;
                }

                return ValidationState.Failed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether no answer is allowed.
        /// If validation IsDone() is false, and IsTimeOutAllowed the validation will be successful.
        /// This is helpful if a validator needs network communication which is not so stable
        /// </summary>
        public bool NoAnswerAllowed { get; private set; }

        /// <summary>
        ///   Gets a value indicating whether the validation is done/finish
        ///   if value is true is only the meaning the validation is done. it's not the meaning that it was successful!
        ///   After calling the method Stop(), IsDone() will return true
        /// </summary>
        public abstract bool IsDone { get; }

        /// <summary>
        ///   Gets the error code.
        /// </summary>
        /// <value>0: No Error</value>
        public abstract int ErrorCode { get; }

        /// <summary>
        /// Gets an error description. Returns null if GetErrorCode is 0
        /// You should overwrite this method.
        /// If you don't have an error description for an error code, so call this (parent) property
        /// </summary>
        public virtual string ErrorDescription
        {
            get
            {
                switch (this.ErrorCode)
                {
                    case ValidatorHandler.ErrNoError:
                        return null;
                    case ValidatorHandler.ErrNoAnswer:
                        return ml.ml_string(96, "No Answer received"); // MLHIDE
                    ////case ValidatorHandler.ERR_UNKNOWN:
                    default:
                        return ml.ml_string(97, "Unknown Error"); // MLHIDE
                }
            }
        }

        /// <summary>
        ///   Starts the validator. This method MUST NOT block!!!
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///   Aborts the validator. If validation is already done, an abort should be able to be performed
        ///   You can unsubscribe by the registered events
        /// </summary>
        public abstract void Stop();
    }
}