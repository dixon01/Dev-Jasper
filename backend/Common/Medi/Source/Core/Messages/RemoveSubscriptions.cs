// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveSubscriptions.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoveSubscriptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public class RemoveSubscriptions : ISubscriptionMessage
    {
        /// <summary>
        /// Gets or sets all types that should be removed from the subscription list.
        /// </summary>
        public string[] Types { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().Name, string.Join(",", this.Types));
        }
    }
}
