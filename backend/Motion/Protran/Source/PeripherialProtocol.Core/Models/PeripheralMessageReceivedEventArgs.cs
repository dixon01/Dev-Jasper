// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="MessageAvailableEventArgs.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;

    /// <summary>The message available event args.</summary>
    public class PeripheralMessageReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralMessageReceivedEventArgs"/> class.</summary>
        /// <param name="messageSize">The message size.</param>
        public PeripheralMessageReceivedEventArgs(int messageSize)
        {
            this.MessageSize = messageSize;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the message size.</summary>
        public int MessageSize { get; private set; }

        #endregion
    }
}