// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageIds.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageIds type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.SoftwareDescription
{
    /// <summary>
    /// Defines the globally known package ids (see <see cref="SoftwarePackageDescriptor.PackageId"/>.
    /// </summary>
    public static class PackageIds
    {
        /// <summary>
        /// The imotion package ids.
        /// </summary>
        public static class Motion
        {
            /// <summary>
            /// The Hardware Manager.
            /// </summary>
            public const string HardwareManager = "Gorba.Motion.HardwareManager";

            /// <summary>
            /// The AHDLC Renderer.
            /// </summary>
            public const string AhdlcRenderer = "Gorba.Motion.Infomedia.AhdlcRenderer";

            /// <summary>
            /// The Audio Renderer.
            /// </summary>
            public const string AudioRenderer = "Gorba.Motion.Infomedia.AudioRenderer";

            /// <summary>
            /// The Composer.
            /// </summary>
            public const string Composer = "Gorba.Motion.Infomedia.Composer";

            /// <summary>
            /// The DirectX Renderer.
            /// </summary>
            public const string DirectXRenderer = "Gorba.Motion.Infomedia.DirectXRenderer";

            /// <summary>
            /// The Protran.
            /// </summary>
            public const string Protran = "Gorba.Motion.Protran";

            /// <summary>
            /// The System Manager.
            /// </summary>
            public const string SystemManager = "Gorba.Motion.SystemManager";

            /// <summary>
            /// The Update.
            /// </summary>
            public const string Update = "Gorba.Motion.Update";

            /// <summary>
            /// The Bus module.
            /// </summary>
            public const string Bus = "Gorba.Motion.Obc.Bus";

            /// <summary>
            /// The Terminal module.
            /// </summary>
            public const string Terminal = "Gorba.Motion.Obc.Terminal";

            /// <summary>
            /// The Ibis module.
            /// </summary>
            public const string IbisControl = "Gorba.Motion.Obc.IbisControl";
        }

        /// <summary>
        /// The Acapela TTS package ids.
        /// </summary>
        public static class Acapela
        {
            /// <summary>
            /// The core Acapela application.
            /// </summary>
            public const string Application = "Acapela.Application";

            /// <summary>
            /// The string format for voices.
            /// Example: German Andreas22k_HD would be named "Acapela.German.Andreas22k_HQ"
            /// </summary>
            public const string VoiceFormat = "Acapela.{0}.{1}";
        }

        /// <summary>
        /// The Acapela TTS package ids.
        /// </summary>
        public static class PowerUnit
        {
            /// <summary>
            /// The main unit firmware.
            /// </summary>
            public const string MainUnitFirmware = "PowerUnit.MainUnit.Firmware";

            /// <summary>
            /// The display unit firmware.
            /// </summary>
            public const string DisplayUnitFirmware = "PowerUnit.DisplayUnit.Firmware";
        }
        /// <summary>
        /// The Luminator package ids.
        /// </summary>
        public static class Luminator
        {
            /// <summary>
            /// The MCU files.
            /// </summary>
            public const string Mcu = "Luminator.Mcu";

            /// <summary>
            /// The LAM files.
            /// </summary>
            public const string Lam = "Luminator.Lam";
        }
    }
}
