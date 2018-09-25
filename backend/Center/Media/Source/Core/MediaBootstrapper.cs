// -----------------------------------------------------------------------
// <copyright file="MediaBootstrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaBootstrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Windows.Threading;
    using System.Xml.Schema;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Startup;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Dictionary;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Protocols.Ximple.Generic;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using NLog;

    /// <summary>
    /// Defines the bootstrapper specific for the Media application.
    /// </summary>
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.ReadabilityRules",
        "SA1100:DoNotPrefixCallsWithBaseUnlessLocalImplementationExists",
        Justification = "It seems a bug in the current version of StyleCop.")]
    public class MediaBootstrapper : ClientApplicationBootstrapperBase
    {
        private const long DefaultMaxUsedSpace = 10000000000;

        private const long DefaultMinAvailableSpace = 100000000;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ResolutionConfiguration> defaultAvailableResolutions;

        private readonly TimeSpan defaultKeepResourceDuration = TimeSpan.FromDays(30);

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaBootstrapper"/> class.
        /// </summary>
        /// <param name="compositionBatch">The composition batch.</param>
        /// <param name="assemblies">The assemblies.</param>
        public MediaBootstrapper(CompositionBatch compositionBatch, params Assembly[] assemblies)
            : base(compositionBatch, assemblies)
        {
            this.defaultAvailableResolutions = new List<ResolutionConfiguration>
                                                   {
                                                       new ResolutionConfiguration
                                                           {
                                                               Height = 768,
                                                               Width = 1368,
                                                           }
                                                   };
        }

        /// <summary>
        /// Checks if the operating system meets the requirements of the application.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the application meets the requirements; <c>false</c> otherwise.
        /// </returns>
        public override bool CheckOsRequirements()
        {
            try
            {
                var operatingSystem = this.GetOsVersionInfo();
                Logger.Info("Operating system: {0}", operatingSystem);
            }
            catch (Exception exception)
            {
                Logger.WarnException("Can't retrieve the information about the Operating System", exception);
            }

            var dotNetVersion = this.GetClrVersion();
            Logger.Info("Current CLR: {0}", dotNetVersion);
            if (dotNetVersion.Major >= 4)
            {
                return true;
            }

            Logger.Error(".NET framework 4 or higher is required.");
            return false;
        }

        /// <summary>
        /// Invoked after composition.
        /// </summary>
        /// <param name="container">
        /// The composition container.
        /// </param>
        protected override void OnComposed(CompositionContainer container)
        {
            base.OnComposed(container);
            var mediaShell = container.GetExportedValue<IMediaShell>();
            var unityContainer = ServiceLocator.Current.GetInstance<IUnityContainer>();
            unityContainer.RegisterInstance(mediaShell);
        }

        /// <summary>
        /// Initializes the service locator.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="result">The result.</param>
        protected override void AddRegistrationsToServiceLocator<TController, TState>(
            BootstrapperResult<TController, TState> result)
        {
            base.AddRegistrationsToServiceLocator(result);
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var dispatcher = new WpfDispatcher(Dispatcher.CurrentDispatcher);
            container.RegisterInstance<IDispatcher>(dispatcher);
            DictionaryDataViewModel dictionary;
            if (this.LoadDictionary(out dictionary))
            {
                container.RegisterInstance(dictionary);
            }

            MediaConfiguration configuration;
            this.LoadMediaConfiguration(out configuration);
            container.RegisterInstance(configuration);

            // configure Medi with no peers (just used within the application)
            MessageDispatcher.Instance.Configure(new ObjectConfigurator(new MediConfig()));

            var resourceManager = new ResourceManager(configuration.ResourceSettings);
            container.RegisterInstance<IResourceManager>(resourceManager);
        }

        private static void OnValidationEventHandler(object sender, ValidationEventArgs e)
        {
            Logger.Error(e.Message);
        }

        /// <summary>
        /// Loads the dictionary
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>
        ///  <c>true</c> if the dictionary was correctly loaded; otherwise, <c>false</c>.
        /// </returns>
        private bool LoadDictionary(out DictionaryDataViewModel dictionary)
        {
            try
            {
                var dictionaryLoader = new ConfigManager<Dictionary>();
                dictionaryLoader.XmlSchema = Dictionary.Schema;

                dictionaryLoader.FileName = "dictionary.xml";
                dictionaryLoader.EnableCaching = true;
                dictionary = new DictionaryDataViewModel(dictionaryLoader.Config);
                return true;
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while loading the dictionary", exception);
            }

            dictionary = null;
            return false;
        }

        private void LoadMediaConfiguration(out MediaConfiguration configuration)
        {
            try
            {
                var xmlSchemaSet = this.GetXmlSchemaSet();
                var mediaConfigurationLoader = new ConfigManager<MediaConfiguration>
                                                   {
                                                       XmlSchemaSet = xmlSchemaSet,
                                                       FileName =
                                                           @"MediaConfiguration.xml",
                                                       EnableCaching = true
                                                   };
                configuration = mediaConfigurationLoader.Config;
                if (string.IsNullOrWhiteSpace(configuration.ResourceSettings.LocalResourcePath))
                {
                    configuration.ResourceSettings.LocalResourcePath =
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }
                else
                {
                    if (!Directory.Exists(configuration.ResourceSettings.LocalResourcePath))
                    {
                        var message = string.Format(
                            "The path '{0}' for the local resource storage could not be found.",
                            configuration.ResourceSettings.LocalResourcePath);
                        throw new DirectoryNotFoundException(message);
                    }

                    if (!Path.IsPathRooted(configuration.ResourceSettings.LocalResourcePath))
                    {
                        var message = string.Format(
                            "The format of path '{0}' is invalid. It must be an absolute path.",
                            configuration.ResourceSettings.LocalResourcePath);
                        throw new FormatException(message);
                    }
                }

                return;
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while loading the media configuration. Using default values.", exception);
            }

            var resourceSettings = new ResourceSettings
                                   {
                                       LocalResourcePath =
                                           Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       MaxUsedDiskSpace = DefaultMaxUsedSpace,
                                       MinRemainingDiskSpace = DefaultMinAvailableSpace,
                                       RemoveLocalResourceAfter = this.defaultKeepResourceDuration,
                                   };
            var physicalScreenSettings = new PhysicalScreenSettings();
            physicalScreenSettings.PhysicalScreenTypes = new List<PhysicalScreenTypeConfig>
                                                {
                                                    new PhysicalScreenTypeConfig
                                                        {
                                                            Name = "TFT",
                                                            AvailableResolutions = this.defaultAvailableResolutions
                                                        }
                                                };
            configuration = new MediaConfiguration
                                {
                                    PhysicalScreenSettings = physicalScreenSettings,
                                    ResourceSettings = resourceSettings
                                };
        }

        private XmlSchemaSet GetXmlSchemaSet()
        {
            var schema =
                this.GetType()
                    .Assembly.GetManifestResourceStream("Gorba.Center.Media.Core.Configuration.MediaConfiguration.xsd");
            if (schema == null)
            {
                throw new FileNotFoundException("Couldn't find schema for media configuration");
            }

            var directXRendererSchema =
                this.GetType().Assembly
                .GetManifestResourceStream("Gorba.Center.Media.Core.Configuration.DirectXRenderer.xsd");
            if (directXRendererSchema == null)
            {
                throw new FileNotFoundException("Couldn't find schema for directx renderer");
            }

            var xmlSchemaSet = new XmlSchemaSet();
            using (schema)
            {
                xmlSchemaSet.Add(XmlSchema.Read(schema, OnValidationEventHandler));
            }

            using (directXRendererSchema)
            {
                xmlSchemaSet.Add(XmlSchema.Read(directXRendererSchema, OnValidationEventHandler));
            }

            return xmlSchemaSet;
        }

        private MasterLayout CreateDefaultFullscreenConfiguration()
        {
            var layout = new MasterLayout
                             {
                                 Name = "Normal",
                                 Columns = "*",
                                 HorizontalGaps = "0",
                                 Rows = "*",
                                 VerticalGaps = "0"
                             };
            return layout;
        }

        private void ValidateResolutionFormat(string resolution)
        {
            if (!resolution.Contains("x"))
            {
                var message =
                    string.Format(
                        "The format of resolution '{0}' is invalid. " + "It must be an something like this: '800x600'.",
                        resolution);

                throw new FormatException(message);
            }

            var resolutionParts = resolution.Split('x');
            if (resolutionParts.Length < 2)
            {
                var message =
                    string.Format(
                        "The format of resolution '{0}' is invalid. " + "It must be an something like this: '800x600'.",
                        resolution);
                throw new FormatException(message);
            }

            int i;
            if (!int.TryParse(resolutionParts[0], out i) || !int.TryParse(resolutionParts[1], out i))
            {
                var message =
                    string.Format(
                        "The format of resolution '{0}' is invalid. " + "It must be an something like this: '800x600'.",
                        resolution);
                throw new FormatException(message);
            }
        }
    }
}
