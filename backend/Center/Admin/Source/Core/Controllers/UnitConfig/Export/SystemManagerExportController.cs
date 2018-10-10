// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.SplashScreens;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.SystemManager;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Configuration.SystemManager.SplashScreen;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Items;
    using Gorba.Common.Configuration.SystemManager.SplashScreen.Trigger;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.SystemManagement.ServiceModel;

    using IResourceService = Gorba.Center.Common.ServiceModel.IResourceService;

    /// <summary>
    /// The export controller that exports everything related to System Manager.
    /// </summary>
    public class SystemManagerExportController : ExportControllerBase
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
            this.AddSoftwarePackageId(PackageIds.Motion.SystemManager);
        }

        /// <summary>
        /// Asynchronously creates the part of the folder structure for which this controller is responsible.
        /// </summary>
        /// <param name="rootFolders">
        /// The root folders to fill with the structure.
        /// Implementers should only add files and folder, but never remove anything created by another controller.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the completion of this method.
        /// </returns>
        public async override Task CreateExportStructureAsync(List<ExportFolder> rootFolders)
        {
            var systemManagerApplication = this.GetSoftwareVersion(PackageIds.Motion.SystemManager);
            await this.AddPackageVersionFilesAsync(rootFolders, systemManagerApplication);

            var systemManagerConfig = await this.CreateSystemManagerXmlAsync(rootFolders);

            await this.AddVersionedXmlConfigFileAsync(
                systemManagerConfig,
                systemManagerApplication,
                @"Config\SystemManager\SystemManager.xml",
                rootFolders);

            this.AddFile(
                @"Config\SystemManager\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("SystemManagerNLog.config")));

            var mediConfig = this.CreateMediServerConfig();
            mediConfig.Services.Add(
                new RemoteResourceServiceConfig { ResourceDirectory = @"..\..\Data\SystemManager\Medi\Resources" });

            var incoming = this.Parent.GetPart<IncomingPartController>();
            if (incoming.HasSelected(IncomingData.Medi))
            {
                var mediSlave = this.Parent.GetPart<MediSlavePartController>();
                mediConfig.Peers.Add(this.CreateMediClientPeerConfig(mediSlave.MasterAddress));
            }

            if (new Version(systemManagerApplication.SoftwareVersion)
                >= SoftwareVersions.SystemManager.MediSupportsGateways)
            {
                if (this.Parent.GetPart<BackgroundSystemConnectionPartController>().ShouldConnect)
                {
                    var client = this.CreateBackgroundSystemClientPeerConfig();
                    mediConfig.Peers.Add(client);
                }
            }

            if (ContainsFile(rootFolders, "Gorba.Common.Medi.Ports.dll"))
            {
                mediConfig.Services.Add(new PortForwardingServiceConfig());
            }

            this.AddXmlConfigFile(mediConfig, @"Config\SystemManager\medi.config", rootFolders);
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            // System Manager doesn't need to start itself, therefore no application config is required
            yield break;
        }

        private static string ConvertColor(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        private static bool ContainsFile(IEnumerable<ExportFolder> folders, string filename)
        {
            foreach (var folder in folders)
            {
                if (ContainsFile(folder.ChildItems.OfType<ExportFolder>().ToList(), filename))
                {
                    return true;
                }

                if (folder.ChildItems.OfType<ExportFileBase>()
                    .Select(f => f.Name)
                    .Any(n => n.Equals(filename, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<SystemManagerConfig> CreateSystemManagerXmlAsync(List<ExportFolder> rootFolders)
        {
            var resolutions = this.Parent.GetPart<ScreenResolutionsPartController>();
            var resolution = resolutions.PrimaryResolution ?? resolutions.SecondaryResolution;

            var outgoing = this.Parent.GetPart<OutgoingPartController>();

            var config = new SystemManagerConfig();
            config.Defaults.Process = new ProcessConfig
                                          {
                                              LaunchDelay = TimeSpan.FromSeconds(0),
                                              RelaunchDelay = TimeSpan.FromSeconds(10),
                                              WindowMode = ProcessWindowStyle.Minimized,
                                              Priority = ProcessPriorityClass.Normal,
                                              KillIfRunning = true
                                          };

            config.SplashScreens.X = 0;
            config.SplashScreens.Y = 0;
            config.SplashScreens.Width = resolution.VisibleWidth;
            config.SplashScreens.Height = resolution.VisibleHeight;

            config.SplashScreens.Items.Add(await this.CreateBootScreenAsync(outgoing, rootFolders));
            config.SplashScreens.Items.Add(this.CreateAnnounceScreen());
            config.SplashScreens.Items.Add(await this.CreateHotKeyScreenAsync(rootFolders));

            var pc = this.Parent.HardwareDescriptor.Platform as PcPlatformDescriptorBase;
            if (pc != null)
            {
                config.SplashScreens.Items.Add(await this.CreateButtonScreenAsync(pc, rootFolders));
            }

            config.System = this.CreateSystemConfig();

            foreach (var appConfig in this.Parent.GetExportControllers().SelectMany(c => c.CreateApplicationConfigs()))
            {
                // These aren't applications to be managed by SystemManager but are being used as a mechanism
                // by which files can be moved to software systems on other Luminator-specific hardware.
                if ((appConfig.Name != "MCU") && (appConfig.Name != "LAM"))
                {
                    config.Applications.Add(appConfig);
                }
            }

            return config;
        }

        private SystemConfig CreateSystemConfig()
        {
            var systemConfig = new SystemConfig();

            var isInform = this.Parent.HardwareDescriptor.Platform is InformPlatformDescriptor;
            var renderersCount =
                this.Parent.GetExportControllers().Count(c => c.IsRenderer && c.SoftwarePackageIds.Count != 0);
            var reboot = this.Parent.GetPart<RebootPartController>();
            systemConfig.RebootAt = reboot.RebootAt;
            systemConfig.RebootAfter = reboot.RebootAfter;

            systemConfig.KickWatchdog = true;
            systemConfig.RamLimit = new SystemRamLimitConfig
                                        {
                                            Enabled = !isInform,
                                            FreeRamMb = 100,
                                            Actions = { new RebootLimitActionConfig() }
                                        };
            systemConfig.CpuLimit = new CpuLimitConfig
            {
                                            Enabled = false,     // For some reason Luminator's hardware can run above this value.  Disable it for now.
                                            //Enabled = !isInform && renderersCount <= 1,
                                            MaxCpuPercentage = 98,
                                            Actions = { new RebootLimitActionConfig() }
                                        };
            systemConfig.DiskLimits = new DiskLimitConfigList
                                          {
                                              Enabled = true,
                                              Disks =
                                                  {
                                                      new DiskLimitConfig
                                                          {
                                                              Enabled = true,
                                                              Path = "C:\\",
                                                              FreeSpaceMb = 5,
                                                              Actions = { new RebootLimitActionConfig() }
                                                          },
                                                      new DiskLimitConfig
                                                          {
                                                              Enabled = true,
                                                              Path = "D:\\",
                                                              FreeSpaceMb = 10,
                                                              FreeSpacePercentage = 5,
                                                              Actions =
                                                              {
                                                                  new PurgeLimitActionConfig
                                                                              {
                                                                                  Path = @"D:\log\archives\"
                                                                              }
                                                              }
                                                          }
                                                  }
                                          };
            systemConfig.PreventPopups = new PreventPopupsConfig
                                             {
                                                 Enabled = true,
                                                 CheckInterval = TimeSpan.FromSeconds(10)
                                             };
            return systemConfig;
        }

        private async Task<SplashScreenConfig> CreateBootScreenAsync(
            OutgoingPartController outgoing, List<ExportFolder> rootFolders)
        {
            var startUp = this.Parent.GetPart<StartUpSplashScreenPartController>();
            var bootScreen = await this.CreateSplashScreenAsync("Boot", startUp, rootFolders);
            bootScreen.ShowOn.Add(new SystemBootTriggerConfig());
            if (outgoing.HasSelected(OutgoingData.DirectX))
            {
                bootScreen.ShowOn.Add(new SystemShutdownTriggerConfig());
                bootScreen.HideOn.Add(
                    new ApplicationStateChangeTriggerConfig
                        {
                            Application = DirectXRendererExportController.AppName,
                            State = ApplicationState.Running
                        });
            }
            else
            {
                bootScreen.HideOn.Add(new SystemShutdownTriggerConfig());
            }

            return bootScreen;
        }

        private async Task<SplashScreenConfig> CreateHotKeyScreenAsync(List<ExportFolder> rootFolders)
        {
            var hotKey = this.Parent.GetPart<HotKeySplashScreenPartController>();
            var hotKeyScreen = await this.CreateSplashScreenAsync("HotKey", hotKey, rootFolders);
            hotKeyScreen.ShowOn.Add(new HotKeyTriggerConfig { Key = "S" });
            hotKeyScreen.HideOn.Add(new HotKeyTriggerConfig { Key = "S" });

            return hotKeyScreen;
        }

        private async Task<SplashScreenConfig> CreateButtonScreenAsync(
            PcPlatformDescriptorBase platform, List<ExportFolder> rootFolders)
        {
            var button = this.Parent.GetPart<ButtonSplashScreenPartController>();
            var buttonScreen = await this.CreateSplashScreenAsync("Button", button, rootFolders);
            buttonScreen.HideOn.Add(new TimeoutTriggerConfig { Delay = TimeSpan.FromSeconds(30) });

            if (platform.HasGenericButton)
            {
                var inputs = this.Parent.GetPart<InputsPartController>();
                buttonScreen.ShowOn.Add(new InputTriggerConfig { Name = inputs.GetButtonName(), Value = 1 });
            }
            else
            {
                buttonScreen.ShowOn.Add(new InputTriggerConfig { Name = "Button", Unit = "*", Value = 1 });
            }

            return buttonScreen;
        }

        private async Task<SplashScreenConfig> CreateSplashScreenAsync(
            string name, SplashScreenPartControllerBase controller, List<ExportFolder> rootFolders)
        {
            var config = new SplashScreenConfig { Name = name, Enabled = controller.IsEnabled };

            config.Foreground = ConvertColor(controller.Foreground);
            config.Background = ConvertColor(controller.Background);

            string logoFileName = null;
            if (!controller.UseDefaultLogo)
            {
                using (
                    var resourceService =
                        this.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
                {
                    var resource = await resourceService.Channel.GetAsync(controller.LogoResourceHash);
                    if (resource != null)
                    {
                        logoFileName = name + "." + resource.MimeType.Substring(resource.MimeType.LastIndexOf('/') + 1);
                        this.AddFile(
                            @"Config\SystemManager\" + logoFileName,
                            rootFolders,
                            n => new ExportResourceFile(n, resource));
                    }
                }
            }

            config.Items.Add(new LogoSplashScreenItem { Filename = logoFileName });
            config.Items.AddRange(controller.GetSelectedItems());

            return config;
        }

        private SplashScreenConfig CreateAnnounceScreen()
        {
            const string UdcpAnnounce = "UDCPAnnounce";
            var announceScreen = new SplashScreenConfig
                                     {
                                         Name = "Announcement",
                                         Enabled = true,
                                         Foreground = "Black",
                                         Background = "Red",
                                         ShowOn = { new InputTriggerConfig { Name = UdcpAnnounce, Value = 1 } },
                                         HideOn = { new InputTriggerConfig { Name = UdcpAnnounce, Value = 0 } },
                                         Items =
                                             {
                                                 new LogoSplashScreenItem(),
                                                 new SystemSplashScreenItem { MachineName = true, Serial = true },
                                                 new NetworkSplashScreenItem { Ip = true, StatusFilter = "Up" }
                                             }
                                     };
            return announceScreen;
        }

        private MediConfig CreateMediServerConfig()
        {
            var mediConfig = new MediConfig
                                 {
                                     InterceptLocalLogs = true,
                                     Peers =
                                         {
                                             new ServerPeerConfig
                                                 {
                                                     Codec =
                                                         new BecCodecConfig
                                                             {
                                                                 Serializer = BecCodecConfig.SerializerType.Default
                                                             },
                                                     Transport =
                                                         new TcpTransportServerConfig
                                                             {
                                                                 LocalPort = TcpTransportServerConfig.DefaultPort
                                                             }
                                                 }
                                         },
                                 };
            return mediConfig;
        }

        private ClientPeerConfig CreateBackgroundSystemClientPeerConfig()
        {
            var connectionString =
                this.Parent.DataController.ConnectionController.BackgroundSystemConfiguration
                    .NotificationsConnectionString;

            var uriBuilder = new UriBuilder(connectionString);
            var transportClientConfig = new TcpTransportClientConfig
                                            {
                                                RemoteHost = uriBuilder.Host,
                                                IdleKeepAliveWait = 120 * 1000,
                                            };
            if (uriBuilder.Port != -1)
            {
                transportClientConfig.RemotePort = uriBuilder.Port;
            }

            var client = new ClientPeerConfig
                             {
                                 Codec = new BecCodecConfig { Serializer = BecCodecConfig.SerializerType.Default },
                                 Transport = transportClientConfig,
                                 IsGateway = true
                             };
            return client;
        }
    }
}
