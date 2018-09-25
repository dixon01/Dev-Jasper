// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranExportController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtranExportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Hardware;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.IO;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Controllers.UnitConfig.TimeSync;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;
    using Gorba.Common.Configuration.Protran.GorbaProtocol;
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Configuration.Protran.VDV301;
    using Gorba.Common.Configuration.SystemManager.Application;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using IbisSerialPortConfig = Gorba.Common.Configuration.Protran.Ibis.SerialPortConfig;
    using IoSerialPortConfig = Gorba.Common.Configuration.Protran.IO.SerialPortConfig;

    /// <summary>
    /// The Protran export controller.
    /// </summary>
    public class ProtranExportController : ExportControllerBase
    {
        /// <summary>
        /// The simulation file location.
        /// </summary>
        public static readonly string SimulationFileLocation = @"D:\Data\Protran\recording.log";
        private const string SimulationFilePath = @"..\..\Data\Protran\recording.log";

        private const string DefaultTransformationId = "_default";

        private const string IntegerTransformationPostfix = "-int";

        private IncomingPartController incoming;

        /// <summary>
        /// Initializes this controller.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        public override void Initialize(UnitConfiguratorController parentController)
        {
            base.Initialize(parentController);

            this.incoming = this.Parent.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateSoftwarePackages();
            this.UpdateSoftwarePackages();
        }

        /// <summary>
        /// Creates the application configurations required for System Manager.
        /// </summary>
        /// <returns>
        /// The list of application configurations. The list can be empty, but never null.
        /// </returns>
        public override IEnumerable<ApplicationConfigBase> CreateApplicationConfigs()
        {
            if (this.SoftwarePackageIds.Count == 0)
            {
                yield break;
            }

            yield return
                this.CreateProcessConfig(@"Progs\Protran\Protran.exe", "Protran", 300);
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
            if (this.SoftwarePackageIds.Count == 0)
            {
                return;
            }

            var application = this.GetSoftwareVersion(PackageIds.Motion.Protran);
            await this.AddPackageVersionFilesAsync(rootFolders, application);

            await this.AddDictionaryAsync(rootFolders);

            var protranConfig = this.CreateProtranXml();
            await this.AddVersionedXmlConfigFileAsync(
                protranConfig,
                application,
                @"Config\Protran\protran.xml",
                rootFolders);

            // Only try to create the gorba.xml if the Protran version supports it.
            Version version;
            if (ParserUtil.TryParse(application.SoftwareVersion, out version) && version.Build >= 1532)
            {
                    var gorbaConfig = new GorbaConfig();
                    await
                        this.AddVersionedXmlConfigFileAsync(
                        gorbaConfig,
                        application,
                        @"Config\Protran\gorba.xml",
                        rootFolders);
            }

            await this.AddVersionedXmlConfigFileAsync(
                this.CreateIoXml(),
                application,
                @"Config\Protran\io.xml",
                rootFolders);

            if (this.incoming.HasSelected(IncomingData.Ibis))
            {
                var ibisConfig = this.CreateIbisXml();
                await this.AddVersionedXmlConfigFileAsync(
                    ibisConfig,
                    application,
                    @"Config\Protran\ibis.xml",
                    rootFolders);

                var hpw074 =
                    this.Parent.GetPart<HPW074TelegramPartController>(
                        string.Format(UnitConfigKeys.IbisProtocol.TelegramFormat, "HPW074"));
                if (hpw074.ViewModel.IsVisible)
                {
                    this.AddFile(
                        @"Config\Protran\" + HPW074TelegramPartController.SpecialTextFile,
                        rootFolders,
                        name => new ExportTextConfigFile(name, hpw074.SpecialText, "text/csv"));
                }
            }

            if (this.incoming.HasSelected(IncomingData.Vdv301))
            {
                var vdv301Config = this.CreateVdv301Xml();
                await this.AddVersionedXmlConfigFileAsync(
                    vdv301Config,
                    application,
                    @"Config\Protran\vdv301.xml",
                    rootFolders);
            }

            this.AddFile(
                @"Config\Protran\NLog.config",
                rootFolders,
                name => new ExportXmlConfigFile(name, this.LoadFileResource("DefaultNLog.config")));

            this.AddXmlConfigFile(this.CreateMediClientConfig(), @"Config\Protran\medi.config", rootFolders);
        }

        private async Task AddDictionaryAsync(List<ExportFolder> rootFolders)
        {
            var dictionaryPath = Path.Combine(
                    Path.GetDirectoryName(ApplicationHelper.GetEntryAssemblyLocation()) ?? Environment.CurrentDirectory,
                    "dictionary.xml");
            if (!File.Exists(dictionaryPath))
            {
                this.Logger.Warn("Couldn't find dictionary: {0}", dictionaryPath);
                return;
            }

            var hash = ResourceHash.Create(dictionaryPath);
            using (
                var resourceService =
                    this.Parent.DataController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                var resource = await resourceService.Channel.GetAsync(hash);
                if (resource == null)
                {
                    this.Logger.Debug(
                        "Dictionary.xml is not yet available as resource on server, creating it ({0})",
                        hash);
                    var applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
                    using (var input = File.OpenRead(dictionaryPath))
                    {
                        resource = new Resource
                                       {
                                           Hash = hash,
                                           Length = input.Length,
                                           UploadingUser = applicationState.CurrentUser,
                                           OriginalFilename = Path.GetFileName(dictionaryPath),
                                           MimeType = "text/xml"
                                       };
                        var result =
                            await
                            resourceService.Channel.UploadAsync(
                                new ResourceUploadRequest { Content = input, Resource = resource });
                        resource = result.Resource;
                    }
                }

                this.AddFile(
                    @"Config\Protran\dictionary.xml",
                    rootFolders,
                    name => new ExportResourceFile(name, resource));
            }
        }

        private ProtranConfig CreateProtranXml()
        {
            var config = new ProtranConfig();

            if (this.incoming.HasSelected(IncomingData.LamXimple))
            {
                if (this.incoming.HasSelected(IncomingData.Ximple))
                {
                    config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "Gorba" });
                }

                config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "XimpleProtocol" });
            }
            else
            {               
                config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "IOProtocol" });
                config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "Gorba" });

                if (this.incoming.HasSelected(IncomingData.Ibis))
                {
                    config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "IbisProtocol" });
                }

                if (this.incoming.HasSelected(IncomingData.Vdv301))
                {
                    config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "VDV301" });
                }
            }

            if (this.incoming.HasSelected(IncomingData.AudioPeripheral))
            {
                config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "PeripheralProtocol" });
            }

            if (this.incoming.HasSelected(IncomingData.AdHoc))
            {
                config.Protocols.Add(new ProtocolConfig { Enabled = true, Name = "AdHocMessagingProtocol" });
            }

            var persistence = this.Parent.GetPart<PersistencePartController>();
            config.Persistence.IsEnabled = persistence.IsEnabled;
            config.Persistence.DefaultValidity = persistence.DefaultValidity;
            return config;
        }

        private IOProtocolConfig CreateIoXml()
        {
            var config = new IOProtocolConfig();

            var inform = this.Parent.HardwareDescriptor.Platform as InformPlatformDescriptor;
            if (inform != null)
            {
                var inputs = this.Parent.GetPart<InputsPartController>().GetInputs();
                var outputs = this.Parent.GetPart<OutputsPartController>().GetOutputs();

                foreach (var serialPort in inform.SerialPorts)
                {
                    var port = new IoSerialPortConfig
                                   {
                                       Name = serialPort.Name,
                                       Rts = this.GetIoName(serialPort.Name, "RTS", outputs, inform.Outputs),
                                       Cts = this.GetIoName(serialPort.Name, "CTS", inputs, inform.Inputs),
                                       Dtr = this.GetIoName(serialPort.Name, "DTR", outputs, inform.Outputs),
                                       Dsr = this.GetIoName(serialPort.Name, "DSR", inputs, inform.Inputs)
                                   };
                    config.SerialPorts.Add(port);
                }
            }

            var transformations = new List<string>();
            var general = this.Parent.GetPart<IoProtocolGeneralPartController>();
            for (int i = 1; i <= general.InputsCount; i++)
            {
                var key = string.Format(UnitConfigKeys.IoProtocol.InputHandlerFormat, i);
                var part = this.Parent.GetPart<InputHandlerPartController>(key);
                var chain = string.IsNullOrEmpty(part.TransformationChainId)
                                ? DefaultTransformationId
                                : part.TransformationChainId;
                config.Inputs.Add(
                    new InputHandlingConfig
                        {
                            Enabled = true,
                            Unit = part.UnitName,
                            Application = part.ApplicationName,
                            Name = part.InputName,
                            UsedFor = part.GenericUsage,
                            TransfRef = chain
                        });
                transformations.Add(chain);
            }

            config.Transformations = this.CreateTransformations(transformations);
            return config;
        }

        private string GetIoName(
            string portName,
            string pinName,
            IDictionary<int, string> mapping,
            IEnumerable<IoDescriptorBase> ios)
        {
            var io = ios.FirstOrDefault(d => d.Name.Contains(portName) && d.Name.Contains(pinName));
            if (io == null)
            {
                return null;
            }

            string name;
            mapping.TryGetValue(io.Index, out name);
            return name;
        }

        private List<Chain> CreateTransformations(ICollection<string> transformations)
        {
            var general = this.Parent.GetPart<TransformationsGeneralPartController>();
            var category = (TransformationsCategoryController)general.Parent;
            var chains =
                category.TransformationPartControllers.Where(c => transformations.Contains(c.ViewModel.ChainId))
                    .Select(part => part.CreateChain())
                    .ToList();
            if (transformations.Contains(DefaultTransformationId))
            {
                chains.Add(new Chain { Id = DefaultTransformationId });
            }

            return chains;
        }

        private IbisConfig CreateIbisXml()
        {
            var config = new IbisConfig();

            var general = this.Parent.GetPart<IbisGeneralPartController>();
            config.Behaviour.ConnectionTimeOut = general.ConnectionTimeout;
            config.Behaviour.ConnectionStatusUsedFor = new GenericUsage("SystemStatus", "RemotePC");
            config.Behaviour.IbisAddresses = general.GetAddresses().ToList();
            config.Behaviour.CheckCrc = general.CheckCrc;
            config.Behaviour.ByteType = general.ByteType;
            config.Behaviour.ProcessPriority = this.Parent.HardwareDescriptor.Platform is InformPlatformDescriptor
                                                   ? ProcessPriorityClass.AboveNormal
                                                   : ProcessPriorityClass.Normal;

            config.Sources.Active = general.SourceType;
            config.Sources.Json = this.CreateJsonConfig();
            config.Sources.SerialPort = this.CreateSerialPortConfig(general.ByteType);
            config.Sources.Simulation = this.CreateSimulationConfig();
            config.Sources.UdpServer = this.CreateUdpServerConfig();

            config.Recording = this.CreateRecordingConfig(general);

            var timeSync = this.Parent.GetPart<IbisTimeSyncPartController>();
            config.TimeSync = timeSync.GetTimeSyncConfig();

            var transformations = new List<string>();
            var integerTransformations = new List<string>();
            var telegrams = this.Parent.GetPart<IbisTelegramSelectionPartController>();
            foreach (var telegramName in telegrams.GetSelectedTelegrams())
            {
                var part =
                    this.Parent.GetPart<IbisTelegramPartControllerBase>(
                        string.Format(UnitConfigKeys.IbisProtocol.TelegramFormat, telegramName));
                var telegramConfig = part.CreateTelegramConfig();
                config.Telegrams.Add(telegramConfig);

                if (part.TelegramType == TelegramType.Empty)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(telegramConfig.TransfRef))
                {
                    telegramConfig.TransfRef = DefaultTransformationId;
                }

                transformations.Add(telegramConfig.TransfRef);
                if (part.TelegramType == TelegramType.Integer)
                {
                    integerTransformations.Add(telegramConfig.TransfRef);
                    telegramConfig.TransfRef += IntegerTransformationPostfix;
                }
            }

            if (config.TimeSync.Enabled)
            {
                this.AddTimeSyncTelegrams(config, transformations, timeSync);
            }

            config.Transformations.AddRange(this.CreateTransformations(transformations));
            foreach (var transformationName in integerTransformations.Distinct())
            {
                var chain = new Chain { Id = transformationName + IntegerTransformationPostfix };
                if (transformationName != DefaultTransformationId)
                {
                    chain.Transformations.Add(new ChainRef { TransfRef = transformationName });
                }

                chain.Transformations.Add(new Integer());
                config.Transformations.Add(chain);
            }

            return config;
        }

        private void AddTimeSyncTelegrams(
            IbisConfig config, ICollection<string> transformations, IbisTimeSyncPartController timeSync)
        {
            if (timeSync.ShouldUseDs006A())
            {
                config.Telegrams.Add(
                    new DS006AConfig
                        {
                            Name = "DS006a",
                            Enabled = true,
                            UsedFor = new GenericUsage("SystemStatus", "Date"),
                            TransfRef = DefaultTransformationId
                        });
                transformations.Add(DefaultTransformationId);
            }
            else
            {
                const string TimeTransformation = "_time";
                const string DateTransformation = "_date";
                config.Telegrams.Add(
                    new SimpleTelegramConfig { Name = "DS005", Enabled = true, TransfRef = TimeTransformation });
                config.Telegrams.Add(
                    new DS006Config
                        {
                            Name = "DS006",
                            Enabled = true,
                            UsedFor = new GenericUsage("SystemStatus", "Date"),
                            InitialYear = TimeProvider.Current.UtcNow.Year,
                            TransfRef = DateTransformation
                        });

                config.Transformations.Add(
                    new Chain
                        {
                            Id = TimeTransformation,
                            Transformations =
                                {
                                    new RegexMapping
                                        {
                                            Mappings =
                                                {
                                                    new Mapping
                                                        {
                                                            From = @"^(\d\d)(\d\d)$",
                                                            To = "$1:$2"
                                                        }
                                                }
                                        }
                                }
                        });

                config.Transformations.Add(
                    new Chain
                    {
                        Id = DateTransformation,
                        Transformations =
                                {
                                    new RegexMapping
                                        {
                                            Mappings =
                                                {
                                                    new Mapping
                                                        {
                                                            From = @"^(\d\d)(\d\d)(\d\d)$",
                                                            To = "$1.$2.20$3"
                                                        }
                                                }
                                        }
                                }
                    });
            }

            config.Telegrams.Sort(
                (a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        private RecordingConfig CreateRecordingConfig(IbisGeneralPartController general)
        {
            return new RecordingConfig { Active = general.Recording, FileAbsPath = SimulationFilePath };
        }

        private JsonConfig CreateJsonConfig()
        {
            return this.Parent.HardwareDescriptor.Platform is InfoVisionPlatformDescriptor ? new JsonConfig() : null;
        }

        private IbisSerialPortConfig CreateSerialPortConfig(ByteType byteType)
        {
            var config = new IbisSerialPortConfig { RetryCount = 10, SerialPortReopen = SerialPortReopen.FrameOnly };
            var port = this.Parent.HardwareDescriptor.Platform.SerialPorts.FirstOrDefault();
            if (port != null)
            {
                config.ComPort = port.Name;
            }

            switch (byteType)
            {
                case ByteType.Ascii7:
                    config.BaudRate = 1200;
                    config.DataBits = 7;
                    config.StopBits = StopBits.Two;
                    config.Parity = Parity.Even;
                    break;
                case ByteType.Hengartner8:
                    config.BaudRate = 2400;
                    config.DataBits = 8;
                    config.StopBits = StopBits.One;
                    config.Parity = Parity.None;
                    break;
                case ByteType.UnicodeBigEndian:
                    config.BaudRate = 2400;
                    config.DataBits = 8;
                    config.StopBits = StopBits.Two;
                    config.Parity = Parity.None;
                    break;
            }

            return config;
        }

        private SimulationConfig CreateSimulationConfig()
        {
            var simulation = this.Parent.GetPart<IbisSimulationPartController>();
            return new SimulationConfig
                       {
                           InitialDelay = simulation.InitialDelay,
                           IntervalBetweenTelegrams = simulation.IntervalBetweenTelegrams,
                           TimesToRepeat = simulation.TimesToRepeat,
                           SimulationFile = SimulationFilePath
                       };
        }

        private UdpServerConfig CreateUdpServerConfig()
        {
            var udpServer = this.Parent.GetPart<IbisUdpServerPartController>();
            return new UdpServerConfig
                       {
                           LocalPort = udpServer.LocalPort,
                           ReceiveFormat = udpServer.ReceiveFormat,
                           SendFormat = udpServer.SendFormat
                       };
        }

        private Vdv301ProtocolConfig CreateVdv301Xml()
        {
            var general = this.Parent.GetPart<Vdv301GeneralPartController>();
            var category = (Vdv301ProtocolCategoryController)general.Parent;
            var config = category.CreateConfig();
            return config;
        }

        private void UpdateSoftwarePackages()
        {
            if (this.incoming.HasSelected(IncomingData.Ibis)
                || this.incoming.HasSelected(IncomingData.Vdv301)
                || this.incoming.HasSelected(IncomingData.LamXimple))
            {
                this.AddSoftwarePackageId(PackageIds.Motion.Protran);
            }
            else
            {
                this.RemoveSoftwarePackageId(PackageIds.Motion.Protran);
            }
        }
    }
}
