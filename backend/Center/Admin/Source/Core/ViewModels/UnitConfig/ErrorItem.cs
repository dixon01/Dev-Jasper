// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using System;

    /// <summary>
    /// An object representing an error in the unit configurator.
    /// </summary>
    public class ErrorItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorItem"/> class.
        /// </summary>
        /// <param name="state">
        /// The error state (<see cref="ErrorState.Ok"/> is not allowed).
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public ErrorItem(ErrorState state, string message)
        {
            if (state == ErrorState.Ok)
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentOutOfRangeException("state", "Error state OK not allowed");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.State = state;
            this.Message = message;
        }

        /// <summary>
        /// Gets the error state.
        /// </summary>
        public ErrorState State { get; private set; }

        /// <summary>
        /// Gets the human readable error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            var other = obj as ErrorItem;
            return other != null && other.State == this.State && other.Message == this.Message;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.State.GetHashCode() ^ this.Message.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Message;
        }
    }
}
