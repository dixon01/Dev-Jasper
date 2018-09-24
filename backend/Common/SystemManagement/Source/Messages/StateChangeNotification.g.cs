// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangeNotification.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StateChangeNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// This message informs every client about the state change of an application.
    /// It is not sent directly by the application but rather by the System Manager.
    /// </summary>
    public class StateChangeNotification : StateChangeMessageBase
    {
    }
}