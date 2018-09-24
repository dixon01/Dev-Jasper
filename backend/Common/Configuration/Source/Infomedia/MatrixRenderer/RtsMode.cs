// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtsMode.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RtsMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.MatrixRenderer
{
    /// <summary>
    /// The possible RTS handling modes.
    /// </summary>
    public enum RtsMode
    {
        /// <summary>
        /// The default behavior for the given hardware platform.
        /// This is <see cref="EnableForTx"/> on InfoVision TFT products and <see cref="Auto"/> for VM.cu.
        /// </summary>
        Default,

        /// <summary>
        /// The RTS line is automatically toggled by the hardware.
        /// This is currently only supported on VM.cu.
        /// </summary>
        Auto,

        /// <summary>
        /// The RTS line is enabled at startup and never changed thereafter.
        /// </summary>
        Enabled,

        /// <summary>
        /// The RTS line is disabled at startup and never changed thereafter.
        /// </summary>
        Disabled,

        /// <summary>
        /// The RTS line is disabled at startup,
        /// then enabled by the software before sending data and disabled again after sending data.
        /// </summary>
        EnableForTx,

        /// <summary>
        /// The RTS line is enabled at startup,
        /// then disabled by the software before sending data and enabled again after sending data.
        /// </summary>
        DisableForTx
    }
}