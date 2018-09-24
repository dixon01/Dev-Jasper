// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationManager.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The application manager that takes care of all <see cref="ApplicationControllerBase"/>s.
    /// </summary>
    public partial class ApplicationManager
    {
        partial void KillProcesses(List<ProcessConfig> toKill)
        {
            // TODO: implement killing processes
        }
    }
}