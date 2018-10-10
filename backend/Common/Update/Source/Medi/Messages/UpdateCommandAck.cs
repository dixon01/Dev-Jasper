// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandAck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi.Messages
{
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// The update command acknowledge message.
    /// This message is sent from the client to the provider to tell that
    /// a certain command has been processed in the client.
    /// </summary>
    public class UpdateCommandAck
    {
        /// <summary>
        /// Gets or sets the update identifier.
        /// </summary>
        public UpdateId UpdateId { get; set; }

        /// <summary>
        /// Gets or sets the unit ID for which the update was destined.
        /// </summary>
        public UnitId UnitId { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}/{1} for {2}", this.UpdateId.BackgroundSystemGuid, this.UpdateId.UpdateIndex, this.UnitId.UnitName);
        }
    }
}
