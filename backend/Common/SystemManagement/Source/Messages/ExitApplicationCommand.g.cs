// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExitApplicationCommand.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExitApplicationCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ServiceModel.Messages
{
    /// <summary>
    /// Message sent from System Manager to clients.
    /// Do not use outside this DLL.
    /// Asks an application to exit immediately. The application should change its state to 
    /// <see cref="ApplicationState.Exiting"/> and react properly, otherwise System Manager
    /// will kill the application forcefully.
    /// </summary>
    public class ExitApplicationCommand
    {
        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId { get; set; }
    }
}
