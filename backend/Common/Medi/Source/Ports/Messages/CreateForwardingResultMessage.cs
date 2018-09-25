// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateForwardingResultMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CreateForwardingResultMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Messages
{
    using Gorba.Common.Medi.Ports.Config;

    /// <summary>
    /// The create forwarding result message.
    /// Don't use this class outside this library.
    /// </summary>
    public class CreateForwardingResultMessage
    {
        /// <summary>
        /// Gets or sets the forwarding id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the error message if there was an error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the resulting config if there was no error.
        /// </summary>
        public ForwardingEndPointConfig ResultingConfig { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "{0}: Id: {1}, config: {2}, error: {3}",
                this.GetType().Name,
                this.Id,
                this.ResultingConfig,
                this.ErrorMessage);
        }
    }
}