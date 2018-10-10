// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessApplicationController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System.Diagnostics;

    /// <summary>
    /// Application controller that launches a separate process.
    /// </summary>
    public partial class ProcessApplicationController
    {
        partial void PrepareProcessInfo(ProcessStartInfo info)
        {
            info.ErrorDialog = false;

            if (this.config.WindowMode != null)
            {
                info.WindowStyle = this.config.WindowMode.Value;
            }
        }

        partial void SetProcessPriority(ProcessPriorityClass value)
        {
            this.process.PriorityClass = value;
        }
    }
}