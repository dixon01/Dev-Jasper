// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteSystemManagerClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteSystemManagerClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// A client to access a remote System Manager to query information.
    /// </summary>
    public class RemoteSystemManagerClient : SystemManagerClientBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSystemManagerClient"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The name of the unit on which the System Manager resides.
        /// </param>
        public RemoteSystemManagerClient(string unitName)
            : this(unitName, MessageDispatcher.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSystemManagerClient"/> class.
        /// </summary>
        /// <param name="unitName">
        /// The name of the unit on which the System Manager resides.
        /// </param>
        /// <param name="messageDispatcher">
        /// The <see cref="MessageDispatcher"/> to use for communication with the remote System Manager.
        /// </param>
        public RemoteSystemManagerClient(string unitName, IMessageDispatcher messageDispatcher)
            : base(messageDispatcher, unitName)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }
    }
}