// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareHandlerFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core
{
    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Motion.HardwareManager.Core.Mgi;

    /// <summary>
    /// Factory for <see cref="IHardwareHandler"/> implementations.
    /// </summary>
    public partial class HardwareHandlerFactory
    {
        partial void CreateHardwareHandler(HardwareManagerConfig hardwareManagerConfig, ref IHardwareHandler handler)
        {
            if (hardwareManagerConfig.Mgi != null && hardwareManagerConfig.Mgi.Enabled)
            {
                handler = new MgiHardwareHandler(hardwareManagerConfig.Mgi);
            }
        }
    }
}
