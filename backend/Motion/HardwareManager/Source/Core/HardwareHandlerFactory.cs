// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareHandlerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core
{
    using Gorba.Common.Configuration.HardwareManager;

    /// <summary>
    /// Factory for <see cref="IHardwareHandler"/> implementations.
    /// </summary>
    public partial class HardwareHandlerFactory
    {
        /// <summary>
        /// The create hardware handler.
        /// </summary>
        /// <param name="hardwareManagerConfig">
        /// The hardware manager config.
        /// </param>
        /// <returns>
        /// The <see cref="IHardwareHandler"/>.
        /// </returns>
        public IHardwareHandler CreateHardwareHandler(HardwareManagerConfig hardwareManagerConfig)
        {
            IHardwareHandler handler = null;
            this.CreateHardwareHandler(hardwareManagerConfig, ref handler);
            return handler;
        }

        partial void CreateHardwareHandler(HardwareManagerConfig hardwareManagerConfig, ref IHardwareHandler handler);
    }
}
