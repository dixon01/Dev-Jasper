// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QmailIdentifierProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class QmailIdentifierProvider : IQmailIdentifierProvider
    {
        private int counter = 1;

        #region Implementation of IQmailIdentifierProvider

        /// <summary>
        /// Creates a unique mail identifier based on the curretn date time and a counter with a range number from 1 to 999.
        /// </summary>
        /// <returns>
        /// String containing an unique mail identifier.
        /// </returns>
        public string GetUniqueMailIdentifier()
        {
            if (++this.counter > 999)
            {
                this.counter = 1;
            }

            return DateTime.UtcNow.ToString("ddMMyyHHmmss") + string.Format("{0:000}", this.counter);
        }

        #endregion
    }
}
