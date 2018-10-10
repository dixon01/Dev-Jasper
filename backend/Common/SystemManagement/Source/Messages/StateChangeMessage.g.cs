// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangeMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateChangeMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from clients to the System Manager.
    /// Do not use outside this DLL.
    /// This message is sent from a client to tell the System Manager
    /// that its state has changed. This message is acknowledged with
    /// <see cref="StateChangeAcknowledge"/>.
    /// </summary>
    public class StateChangeMessage : StateChangeMessageBase
    {
    }
}