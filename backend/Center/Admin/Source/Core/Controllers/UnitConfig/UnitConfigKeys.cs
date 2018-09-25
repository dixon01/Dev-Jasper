// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigKeys.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigKeys type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// Keys used to find <see cref="PartViewModelBase"/> objects.
    /// </summary>
    /// <seealso cref="PartViewModelBase.PartKey"/>
    public static class UnitConfigKeys
    {
        /// <summary>
        /// The initialization category.
        /// </summary>
        public static class Initialization
        {
            /// <summary>
            /// The initialization category key.
            /// </summary>
            public const string Category = "Initialization";

            /// <summary>
            /// The load data part key.
            /// </summary>
            public const string LoadData = "Initialization.LoadData";
        }

        /// <summary>
        /// The hardware category.
        /// </summary>
        public static class Hardware
        {
            /// <summary>
            /// The hardware category key.
            /// </summary>
            public const string Category = "Hardware";

            /// <summary>
            /// The screen resolutions selection part key.
            /// </summary>
            public const string ScreenResolutions = "Hardware.ScreenResolutions";

            /// <summary>
            /// The screen orientation selection part key.
            /// </summary>
            public const string ScreenOrientation = "Hardware.ScreenOrientation";

            /// <summary>
            /// The multi screen mode handling part key.
            /// </summary>
            public const string MultiScreenMode = "Hardware.MultiScreenMode";

            /// <summary>
            /// The display brightness setting part key.
            /// </summary>
            public const string DisplayBrightness = "Hardware.DisplayBrightness";

            /// <summary>
            /// The DVI level shifters part key.
            /// </summary>
            public const string DviLevelShifters = "Hardware.DviLevelShifters";

            /// <summary>
            /// The inputs part key.
            /// </summary>
            public const string Inputs = "Hardware.Inputs";

            /// <summary>
            /// The outputs part key.
            /// </summary>
            public const string Outputs = "Hardware.Outputs";

            /// <summary>
            /// The RS-485 mode part key.
            /// </summary>
            public const string Rs485Mode = "Hardware.Rs485Mode";

            /// <summary>
            /// The transceivers part key.
            /// </summary>
            public const string Transceivers = "Hardware.Transceivers";

            /// <summary>
            /// The reboot part key.
            /// </summary>
            public const string Reboot = "Hardware.Reboot";
        }

        /// <summary>
        /// The software category.
        /// </summary>
        public static class Software
        {
            /// <summary>
            /// The hardware part key.
            /// </summary>
            public const string Category = "Software";

            /// <summary>
            /// The incoming data part key.
            /// </summary>
            public const string Incoming = "Software.Incoming";

            /// <summary>
            /// The outgoing data part key.
            /// </summary>
            public const string Outgoing = "Software.Outgoing";

            /// <summary>
            /// The Medi slave part key.
            /// </summary>
            public const string MediSlave = "Software.MediSlave";

            /// <summary>
            /// The Background System connection part key.
            /// </summary>
            public const string BackgroundSystemConnection = "Software.BackgroundSystemConnection";
        }

        /// <summary>
        /// The time synchronization category.
        /// </summary>
        public static class TimeSync
        {
            /// <summary>
            /// The time synchronization category key.
            /// </summary>
            public const string Category = "TimeSync";

            /// <summary>
            /// The time source part key.
            /// </summary>
            public const string TimeSource = "TimeSync.TimeSource";

            /// <summary>
            /// The IBIS part key.
            /// </summary>
            public const string Ibis = "TimeSync.Ibis";

            /// <summary>
            /// The VDV 301 part key.
            /// </summary>
            public const string Vdv301 = "TimeSync.Vdv301";

            /// <summary>
            /// The SNTP part key.
            /// </summary>
            public const string Sntp = "TimeSync.Sntp";
        }

        /// <summary>
        /// The system config category.
        /// </summary>
        public static class SystemConfig
        {
            /// <summary>
            /// The category key.
            /// </summary>
            public const string Category = "SystemConfig";

            /// <summary>
            /// The config mode part key.
            /// </summary>
            public const string ConfigMode = "SystemConfig.ConfigMode";

            /// <summary>
            /// The single config part key.
            /// </summary>
            public const string Single = "SystemConfig.Single";

            /// <summary>
            /// The global config part key.
            /// </summary>
            public const string Global = "SystemConfig.Global";

            /// <summary>
            /// The format for the per-I/O config part key.
            /// </summary>
            public const string IoFormat = "SystemConfig.IO-{0}";
        }

        /// <summary>
        /// The splash screens category.
        /// </summary>
        public static class SplashScreens
        {
            /// <summary>
            /// The splash screens category key.
            /// </summary>
            public const string Category = "SplashScreens";

            /// <summary>
            /// The start-up part key.
            /// </summary>
            public const string StartUp = "SplashScreens.StartUp";

            /// <summary>
            /// The hotkey part key.
            /// </summary>
            public const string HotKey = "SplashScreens.HotKey";

            /// <summary>
            /// The button part key.
            /// </summary>
            public const string Button = "SplashScreens.Button";
        }

        /// <summary>
        /// The update category.
        /// </summary>
        public static class Update
        {
            /// <summary>
            /// The update category key.
            /// </summary>
            public const string Category = "Update";

            /// <summary>
            /// The update agent part key.
            /// </summary>
            public const string Agent = "Update.Agent";

            /// <summary>
            /// The methods part key.
            /// </summary>
            public const string Methods = "Update.Methods";

            /// <summary>
            /// The Azure part key.
            /// </summary>
            public const string Azure = "Update.Azure";

            /// <summary>
            /// The USB part key.
            /// </summary>
            public const string Usb = "Update.USB";

            /// <summary>
            /// The FTP part key.
            /// </summary>
            public const string Ftp = "Update.FTP";

            /// <summary>
            /// The Medi master part key.
            /// </summary>
            public const string MediMaster = "Update.MediMaster";

            /// <summary>
            /// The Medi slave part key.
            /// </summary>
            public const string MediSlave = "Update.MediSlave";
        }

        /// <summary>
        /// The Protran category.
        /// </summary>
        public static class Protran
        {
            /// <summary>
            /// The Protran category key.
            /// </summary>
            public const string Category = "Protran";

            /// <summary>
            /// The persistence part key.
            /// </summary>
            public const string Persistence = "Protran.Persistence";
        }

        /// <summary>
        /// The I/O Protocol category.
        /// </summary>
        public static class IoProtocol
        {
            /// <summary>
            /// The I/O Protocol category key.
            /// </summary>
            public const string Category = "IoProtocol";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "IoProtocol.General";

            /// <summary>
            /// The input handler part key format.
            /// </summary>
            public const string InputHandlerFormat = "IoProtocol.InputHandler-{0}";
        }

        /// <summary>
        /// The IBIS Protocol category.
        /// </summary>
        public static class IbisProtocol
        {
            /// <summary>
            /// The IBIS Protocol category key.
            /// </summary>
            public const string Category = "IbisProtocol";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "IbisProtocol.General";

            /// <summary>
            /// The simulation part key.
            /// </summary>
            public const string Simulation = "IbisProtocol.Simulation";

            /// <summary>
            /// The UDP server part key.
            /// </summary>
            public const string UdpServer = "IbisProtocol.UdpServer";

            /// <summary>
            /// The telegram selection part key.
            /// </summary>
            public const string TelegramSelection = "IbisProtocol.TelegramSelection";

            /// <summary>
            /// The telegram part key format.
            /// </summary>
            public const string TelegramFormat = "IbisProtocol.Telegram-{0}";

            /// <summary>
            /// The telegram part key format.
            /// </summary>
            public const string DS021AConnections = "IbisProtocol.DS021a-Connections";
        }

        /// <summary>
        /// The VDV 301 Protocol category.
        /// </summary>
        public static class Vdv301Protocol
        {
            /// <summary>
            /// The VDV 301 Protocol category key.
            /// </summary>
            public const string Category = "Vdv301Protocol";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "Vdv301Protocol.General";

            /// <summary>
            /// The languages part key.
            /// </summary>
            public const string Languages = "Vdv301Protocol.Languages";

            /// <summary>
            /// The telegram selection part key.
            /// </summary>
            public const string TelegramSelection = "Vdv301Protocol.TelegramSelection";

            /// <summary>
            /// The service part key format.
            /// </summary>
            public const string ServiceFormat = "Vdv301Protocol.Service-{0}";

            /// <summary>
            /// The data item part key format.
            /// </summary>
            public const string DataItemFormat = "Vdv301Protocol.DataItem-{0}";
        }

        /// <summary>
        /// The Transformations category.
        /// </summary>
        public static class Transformations
        {
            /// <summary>
            /// The Transformations category key.
            /// </summary>
            public const string Category = "Transformations";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "Transformations.General";

            /// <summary>
            /// The transformation part key format.
            /// </summary>
            public const string TransformationFormat = "Transformations.Transformation-{0}";
        }

        /// <summary>
        /// The Composer category.
        /// </summary>
        public static class Composer
        {
            /// <summary>
            /// The Composer category key.
            /// </summary>
            public const string Category = "Composer";

            /// <summary>
            /// The general Composer part key.
            /// </summary>
            public const string General = "Composer.General";
        }

        /// <summary>
        /// The DirectX Renderer category.
        /// </summary>
        public static class DirectXRenderer
        {
            /// <summary>
            /// The DirectX Renderer category key.
            /// </summary>
            public const string Category = "DirectXRenderer";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "DirectXRenderer.General";

            /// <summary>
            /// The text part key.
            /// </summary>
            public const string Text = "DirectXRenderer.Text";

            /// <summary>
            /// The image part key.
            /// </summary>
            public const string Image = "DirectXRenderer.Image";

            /// <summary>
            /// The video part key.
            /// </summary>
            public const string Video = "DirectXRenderer.Video";
        }

        /// <summary>
        /// The AHDLC Renderer category.
        /// </summary>
        public static class AhdlcRenderer
        {
            /// <summary>
            /// The AHDLC Renderer category key.
            /// </summary>
            public const string Category = "AhdlcRenderer";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "AhdlcRenderer.General";

            /// <summary>
            /// The sign part key format.
            /// </summary>
            public const string SignFormat = "AhdlcRenderer.Sign-{0}";
        }

        /// <summary>
        /// The Audio Renderer category.
        /// </summary>
        public static class AudioRenderer
        {
            /// <summary>
            /// The Audio Renderer category key.
            /// </summary>
            public const string Category = "AudioRenderer";

            /// <summary>
            /// The general part key.
            /// </summary>
            public const string General = "AudioRenderer.General";

            /// <summary>
            /// The sign part key format.
            /// </summary>
            public const string ChannelFormat = "AudioRenderer.Channel-{0}";

            /// <summary>
            /// The Acapela part key.
            /// </summary>
            public const string Acapela = "AudioRenderer.Acapela";
        }

        /// <summary>
        /// The Conclusion category.
        /// </summary>
        public static class Conclusion
        {
            /// <summary>
            /// The Conclusion category key.
            /// </summary>
            public const string Category = "Conclusion";

            /// <summary>
            /// The software versions part key.
            /// </summary>
            public const string SoftwareVersions = "Conclusion.SoftwareVersions";

            /// <summary>
            /// The Export Preparation part key.
            /// </summary>
            public const string ExportPreparation = "Conclusion.ExportPreparation";

            /// <summary>
            /// The Export execution part key.
            /// </summary>
            public const string ExportExecution = "Conclusion.ExportExecution";

            /// <summary>
            /// The local download part key.
            /// </summary>
            public const string LocalDownload = "Conclusion.LocalDownload";
        }

        /// <summary>
        /// The Thoreb C90+c74 category.
        /// </summary>
        public static class ThorebC90C74
        {
            /// <summary>
            /// The Thoreb C90+c74 category key.
            /// </summary>
            public const string Category = "ThorebC90c74";

            /// <summary>
            /// The bus.exe part key.
            /// </summary>
            public const string Bus = "ThorebC90c74.Bus";

            /// <summary>
            /// The terminal part key.
            /// </summary>
            public const string Terminal = "ThorebC90c74.Terminal";

            /// <summary>
            /// The IBIS module part key.
            /// </summary>
            public const string Ibis = "ThorebC90c74.Ibis";
        }

        /// <summary>
        /// The main unit category.
        /// </summary>
        public static class MainUnit
        {
            /// <summary>
            /// The main unit category.
            /// </summary>
            public const string Category = "MainUnit";

            /// <summary>
            /// The main unit configuration part key.
            /// </summary>
            public const string Configuration = "MainUnit.Configuration";

            /// <summary>
            /// The display unit(s) configuration part key.
            /// </summary>
            public const string DisplayUnit = "MainUnit.DisplayUnit";
        }
    }
}
