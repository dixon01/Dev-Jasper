// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeviceRenderContextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDeviceRenderContextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer
{
    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Motion.Infomedia.DirectXRenderer.Engine;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Factory for <see cref="IDxDeviceRenderContext"/> objects.
    /// </summary>
    public interface IDeviceRenderContextFactory
    {
        /// <summary>
        /// Gets the device configuration.
        /// </summary>
        DeviceConfig DeviceConfig { get; }

        /// <summary>
        /// Creates a new context for the given device.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IDxDeviceRenderContext"/> implementation.
        /// </returns>
        IDxDeviceRenderContext CreateContext(Device device);
    }
}