// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiMessageEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Continer of a generic ISI message received from the ISI server.
    /// </summary>
    public class IsiMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the event's ISI message.
        /// </summary>
        public IsiMessageBase IsiMessage { get; set; }
    }
}
