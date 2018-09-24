// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareAbstractionBase.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareAbstractionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core.Hal
{
    /// <summary>
    /// The base class for the different HALs.
    /// </summary>
    public abstract partial class HardwareAbstractionBase
    {
        private static HardwareAbstractionBase Create()
        {
            return new VmCuHardwareAbstraction();
        }
    }
}
