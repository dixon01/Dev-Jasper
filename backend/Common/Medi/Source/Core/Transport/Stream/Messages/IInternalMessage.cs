// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInternalMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IInternalMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream.Messages
{
    /// <summary>
    /// Tagging interface to tell Medi that this is a message only used by the Stream stack.
    /// </summary>
    internal interface IInternalMessage
    {
    }
}