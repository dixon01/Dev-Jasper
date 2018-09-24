// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQmailIdentifierProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Qmail idenitfier provider interface used to get the qmail name str.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Qmail idenitfier provider interface used to get the qmail name str in the <see cref="QnetProtocolStack"/>.
    /// </summary>
    public interface IQmailIdentifierProvider
    {
        /// <summary>
        /// Creates a unique mail identifier based on the curretn date time and a counter with a range number from 1 to 999.
        /// </summary>
        /// <returns>
        /// String containing an unique mail identifier.
        /// </returns>
        string GetUniqueMailIdentifier();
    }
}
