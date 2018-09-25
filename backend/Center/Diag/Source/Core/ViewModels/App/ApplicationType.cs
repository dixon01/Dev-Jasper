// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.App
{
    /// <summary>
    /// The application type.
    /// </summary>
    public enum ApplicationType
    {
        /// <summary>
        /// The type of the application is (currently) unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The System Manager (or System Manager Shell).
        /// </summary>
        SystemManager,

        /// <summary>
        /// The Hardware Manager.
        /// </summary>
        HardwareManager,

        /// <summary>
        /// The Update application.
        /// </summary>
        Update,

        /// <summary>
        /// The Protran.
        /// </summary>
        Protran,

        /// <summary>
        /// The Composer.
        /// </summary>
        Composer,

        /// <summary>
        /// The DirectX Renderer.
        /// </summary>
        DirectXRenderer,

        /// <summary>
        /// The AHDLC Renderer.
        /// </summary>
        AhdlcRenderer,

        /// <summary>
        /// The Audio Renderer.
        /// </summary>
        AudioRenderer
    }
}
