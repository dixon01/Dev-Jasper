// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationDoneEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValidationDoneEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Validator
{
    using System;

    /// <summary>
    /// The validation done event arguments.
    /// </summary>
    internal class ValidationDoneEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationDoneEventArgs"/> class.
        /// </summary>
        /// <param name="validatedInput">
        /// The validated input.
        /// </param>
        /// <param name="validatedInput2">
        /// The validated input 2.
        /// </param>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="error">
        /// The error message.
        /// </param>
        public ValidationDoneEventArgs(int validatedInput, int validatedInput2, int errorCode, string error)
        {
            this.ValidatedInput = validatedInput;
            this.ValidatedInput2 = validatedInput2;
            this.ErrorCode = errorCode;
            this.Error = error;
        }

        /// <summary>
        /// Gets the validated input.
        /// </summary>
        public int ValidatedInput { get; private set; }

        /// <summary>
        /// Gets the validated input 2.
        /// </summary>
        public int ValidatedInput2 { get; private set; }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Error { get; private set; }
    }
}