// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangeAcknowledge.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateChangeAcknowledge type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// Acknowledge to a <see cref="StateChangeMessage"/>, telling
    /// the client that the message was received and the state was updated
    /// accordingly.
    /// </summary>
    public class StateChangeAcknowledge : StateChangeMessageBase
    {
    }
}