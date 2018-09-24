// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HardwareManagerExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HardwareManagerExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Software;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Configuration.HardwareManager.Mgi;
    using Gorba.Common.Configuration.HardwareManager.Vdv301;
    using Gorba.Common.Configuration.SystemManager.Application;

    /// <summary>
    /// The export controller that exports everything related to Hardware Manager.
    /// </summary>
    public class HardwareManagerExportController : ExportControllerBase
    {
        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);
            this.AddSoftwarePackageId(PackageIds.Motion.HardwareManager);
        }

        /// <summary>
        /// Asynchronously creates the part of the folder structure for which this controller is responsible.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to fill with the structure.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the completion of this method.
        /// </returns>
        public async override Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            var hardwareManager = this.GetSoftwareVersion(PackageIds.Motion.HardwareManager);
            await this.AddPackageVersionFilesAsync(rootFolders, hardwareManager);

            var hwmConfig = this.CreateHardwareManagerXml();

            await this.AddVersionedXmlConfigFileAsync(
                hwmConfig,
                hardwareManager,
                @"Config\HardwareManager\HardwareManager.xml",
                rootFolders);

            this.AddFile(
                @"Config\HardwareManager\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            var mediConfig = this.CreateMediClientConfig();
            this.AddXmlConfigFile(
                mediConfig,
                @"Config\HardwareManager\medi.config",
                rootFolders);
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            yield return
                this.CreateProcessConfig(@"Progs\HardwareManager\HardwareManager.exe", "Hardware Manager", 200, 30);
        }

        private HardwareManagerConfig CreateHardwareManagerXml()
        {
            var hwmConfig = new HardwareManagerConfig();

            hwmConfig.BroadcastTimeChanges = this.Parent.GetPart<TimeSourcePartController>().HasBroadcastEnabled();

            hwmConfig.Mgi = this.CreateMgiConfig();
            hwmConfig.Sntp = this.Parent.GetPart<SntpTimeSyncPartController>().GetConfig();
            hwmConfig.Vdv301 = this.CreateVdv301Config();
            hwmConfig.Settings = this.CreateSettings();
            return hwmConfig;
        }

        private Vdv301Config CreateVdv301Config()
        {
            var config = new Vdv301Config();
            config.Enabled = this.Parent.GetPart<IncomingPartController>().HasSelected(IncomingData.Vdv301);
            if (!config.Enabled)
            {
                return config;
            }

            config.TimeSync = this.Parent.GetPart<Vdv301TimeSyncPartController>().GetConfig();

            var general = this.Parent.GetPart<Vdv301GeneralPartController>();
            config.DeviceClass = DeviceClass.InteriorDisplay;
            config.ValidateHttpRequests = general.ValidateHttpRequests;
            config.ValidateHttpResponses = general.ValidateHttpResponses;
            config.VerifyVersion = general.VerifyVersion;

            return config;
        }

        private MgiConfig CreateMgiConfig()
        {
            var mgiConfig = new MgiConfig();

            // Luminator configuration has GPIO on the audio board.  To keep HardwareManager from restarting, disable GPIO functionality.
            mgiConfig.Enabled = false;
            return mgiConfig;

            var infoVision = this.Parent.HardwareDescriptor.Platform as InfoVisionPlatformDescriptor;
            if (infoVision == null)
            {
                mgiConfig.Enabled = false;
                return mgiConfig;
            }

            // inputs
            var inputsPart = this.Parent.GetPart<InputsPartController>();
            foreach (var input in inputsPart.GetInputs())
            {
                mgiConfig.Gpio.Pins.Add(new PinConfig { Index = input.Key, Name = input.Value });
            }

            if (infoVision.HasGenericButton)
            {
                mgiConfig.Button = inputsPart.GetButtonName();
            }

            // outputs
            var outputsPart = this.Parent.GetPart<OutputsPartController>();
            foreach (var output in outputsPart.GetOutputs())
            {
                mgiConfig.Gpio.Pins.Add(new PinConfig { Index = output.Key, Name = output.Value });
            }

            if (infoVision.HasGenericLed)
            {
                mgiConfig.UpdateLed = outputsPart.GetLedName();
            }

            // sort all pins
            mgiConfig.Gpio.Pins.Sort((a, b) => a.Index.CompareTo(b.Index));

            // RS-485 interface
            if (infoVision.HasSharedRs485Port)
            {
                var rs485Part = this.Parent.GetPart<Rs485ModePartController>();
                mgiConfig.Rs485Interface = rs485Part.GetCompactRs485Switch();
            }

            // DVI level shifters
            var dviPart = this.Parent.GetPart<DviLevelShiftersPartController>();
            mgiConfig.DviLevelShifters = dviPart.GetLevelShifterConfigs().ToList();

            // multi-protocol transceivers
            var transPart = this.Parent.GetPart<TransceiversPartController>();
            mgiConfig.Transceivers = transPart.GetTransceiverConfigs().ToList();

            // default backlight value
            var backlight = this.Parent.GetPart<DisplayBrightnessPartController>();
            mgiConfig.DefaultBacklightValue = backlight.DisplayBrightness;
            mgiConfig.BacklightControlRate = backlight.GetBacklightControlRate();

            mgiConfig.Enabled = true;
            return mgiConfig;
        }

        private List<HardwareManagerSetting> CreateSettings()
        {
            var mode = this.Parent.GetPart<ConfigModePartController>();
            var settings = new List<HardwareManagerSetting>();
            if (!mode.ShouldUseMultiConfig())
            {
                var single = this.Parent.GetPart<SingleSystemConfigPartController>();
                settings.Add(this.CreateSetting(mode, null, single));
                return settings;
            }

            var global = this.Parent.GetPart<GlobalSystemConfigPartController>();
            for (int i = 0; i < mode.GetIoConfigCount(); i++)
            {
                var key = string.Format(UnitConfigKeys.SystemConfig.IoFormat, i);
                var specific = this.Parent.GetPart<IOSystemConfigPartController>(key);
                var setting = this.CreateSetting(mode, specific, global);
                foreach (var kvp in specific.GetInputConditions().OrderBy(p => p.Key))
                {
                    setting.Conditions.Add(new IOCondition { Name = kvp.Key, Value = kvp.Value ? 1 : 0 });
                }

                settings.Add(setting);
            }

            return settings;
        }

        private HardwareManagerSetting CreateSetting(
            ConfigModePartController mode,
            SystemConfigPartControllerBase regular,
            SystemConfigPartControllerBase fallback)
        {
            var setting = new HardwareManagerSetting();
            setting.HostnameSource = HostnameSource.MacAddress;
            setting.Display = this.CreateDisplayConfig();

            if (mode.GetIpAddressUsage() == SettingItemMode.UseSpecific && regular != null)
            {
                if (regular.UseDhcp)
                {
                    setting.UseDhcp = true;
                }
                else
                {
                    setting.IpAddress = new XmlIpAddress(regular.IpAddress);
                    setting.UseDhcp = false;
                }
            }
            else if (mode.GetIpAddressUsage() == SettingItemMode.UseGlobal && fallback != null)
            {
                if (fallback.UseDhcp)
                {
                    setting.UseDhcp = true;
                }
                else
                {
                    setting.IpAddress = new XmlIpAddress(fallback.IpAddress);
                    setting.UseDhcp = false;
                }
            }

            if (!setting.UseDhcp)
            {
                if (mode.GetNetworkMaskUsage() == SettingItemMode.UseSpecific && regular != null)
                {
                    setting.SubnetMask = new XmlIpAddress(regular.NetworkMask);
                }
                else if (mode.GetNetworkMaskUsage() == SettingItemMode.UseGlobal && fallback != null)
                {
                    setting.SubnetMask = new XmlIpAddress(fallback.NetworkMask);
                }

                if (mode.GetIpGatewayUsage() == SettingItemMode.UseSpecific && regular != null)
                {
                    setting.Gateway = new XmlIpAddress(regular.IpGateway);
                }
                else if (mode.GetIpGatewayUsage() == SettingItemMode.UseGlobal && fallback != null)
                {
                    setting.Gateway = new XmlIpAddress(fallback.IpGateway);
                }
            }

            if (mode.GetDnsServersUsage() == SettingItemMode.UseSpecific && regular != null)
            {
                setting.DnsServers = regular.DnsServers.ConvertAll(ip => new XmlIpAddress(ip));
            }
            else if (mode.GetDnsServersUsage() == SettingItemMode.UseGlobal && fallback != null)
            {
                setting.DnsServers = fallback.DnsServers.ConvertAll(ip => new XmlIpAddress(ip));
            }

            if (mode.GetTimeZoneUsage() == SettingItemMode.UseSpecific && regular != null)
            {
                setting.TimeZone = regular.TimeZone;
            }
            else if (mode.GetTimeZoneUsage() == SettingItemMode.UseGlobal && fallback != null)
            {
                setting.TimeZone = fallback.TimeZone;
            }

            return setting;
        }

        private DisplayConfig CreateDisplayConfig()
        {
            var orientation = this.Parent.GetPart<DisplayOrientationPartController>();
            var resolutions = this.Parent.GetPart<ScreenResolutionsPartController>();
            var multiScreen = this.Parent.GetPart<MultiScreenPartController>();
            var primary = multiScreen.GetPrimaryScreen();
            var config = new DisplayConfig();
            config.Screens.Add(this.CreateScreenConfig(resolutions.PrimaryResolution, primary == 1));
            if (resolutions.SecondaryResolution != null)
            {
                config.Screens.Add(this.CreateScreenConfig(resolutions.SecondaryResolution, primary == 2));
                config.Mode = multiScreen.GetDisplayMode();
            }

            config.Screens[0].Orientation = orientation.ScreenOrientation.Mode;
            return config;
        }

        private ScreenConfig CreateScreenConfig(ScreenResolution screenResolution, bool isMainDisplay)
        {
            return new ScreenConfig
                       {
                           Width = screenResolution.PhysicalWidth,
                           Height = screenResolution.PhysicalHeight,
                           Orientation = OrientationMode.Landscape,
                           IsMainDisplay = isMainDisplay
                       };
        }
    }
}