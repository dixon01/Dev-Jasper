// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerApplicationController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.Applications
{
    using System;

    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Application controller that monitors the system manager itself and its resources.
    /// There should always only be one instance of this class in an <see cref="ApplicationManager"/>.
    /// </summary>
    public partial class SystemManagerApplicationController
    {
        private const int CpuLimitPercent = 70;
        private const int RamLimitMb = 120;

        private static ProcessConfig CreateConfig()
        {
            var args = Environment.GetCommandLineArgs();
            var commandLine = string.Join(" ", args, 1, args.Length - 1);

            return new ProcessConfig
                       {
                           Enabled = true,
                           Name = "System Manager",
                           ExecutablePath = ApplicationHelper.GetEntryAssemblyLocation(),
                           Arguments = commandLine,
                           CpuLimit =
                               new CpuLimitConfig
                                   {
                                       Enabled = true,
                                       MaxCpuPercentage = CpuLimitPercent,
                                       Actions = { new RebootLimitActionConfig() }
                                   },
                           RamLimit =
                               new ApplicationRamLimitConfig
                                   {
                                       Enabled = true,
                                       MaxRamMb = RamLimitMb,
                                       Actions = { new RebootLimitActionConfig() }
                                   }
                       };
        }
    }
}
