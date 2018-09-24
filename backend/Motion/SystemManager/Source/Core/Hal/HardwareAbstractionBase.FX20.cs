// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareAbstractionBase.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareAbstractionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    using System;

    using Gorba.Common.Configuration.SystemManager;

    using Kontron.Jida32;

    using NLog;

    /// <summary>
    /// The base class for the different HALs.
    /// </summary>
    public abstract partial class HardwareAbstractionBase
    {
        private static HardwareAbstractionBase CreateHardwareLayer(SystemConfig config)
        {
            var gorbaHardware = new GorbaHardwareAbstraction();
            
            if (IsLuminatorHardware(config))
            {
                return new LuminatorHardwareAbstraction();
            }
            else if (IsMgiHardware())
            {
                return new MgiHardwareAbstraction();
            }

            return gorbaHardware;
        }

        private static bool IsLuminatorHardware(SystemConfig config)
        {
            // For now we will us the existing system config flag to indicate we are or are not LTG hardware. 
            // Either case today we can use the Gorba HAL for the 18" displays but must use the Goba HAL with the watchdog till re-design.            
            var watchDog = config?.KickWatchdog ?? false;
            return watchDog == false;
        }

        private static bool IsMgiHardware()
        {
            try
            {
                using (var jida = new JidaApi())
                {
                    return jida.Initialize();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Debug(ex, "Couldn't access JIDA, assuming we are on a legacy Gorba Topbox");
            }

            return false;
        }
    }
}
