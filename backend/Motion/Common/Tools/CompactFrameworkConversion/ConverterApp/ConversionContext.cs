// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConversionContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConversionContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.Config;

    using NLog;

    /// <summary>
    /// The conversion context.
    /// </summary>
    public class ConversionContext : IConversionContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, ProjectConversionConfig> configs =
            new Dictionary<string, ProjectConversionConfig>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Gets the project specific conversion config for the given project file.
        /// </summary>
        /// <param name="projectFile">
        /// The full path to the project file.
        /// </param>
        /// <param name="postfix">
        /// The file postfix used during the conversion.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectConversionConfig"/>.
        /// </returns>
        public ProjectConversionConfig GetProjectConfig(string projectFile, string postfix)
        {
            projectFile = Path.GetFullPath(projectFile);
            var key = projectFile + "?" + postfix;
            ProjectConversionConfig config;
            if (this.configs.TryGetValue(key, out config))
            {
                return config;
            }

            var dir = Path.GetDirectoryName(projectFile);
            if (dir != null)
            {
                var configFile = Path.Combine(dir, "ProjectConversion" + postfix + ".xml");
                if (File.Exists(configFile))
                {
                    Logger.Debug("Loading config from {0}", configFile);
                    var serializer = new XmlSerializer(typeof(ProjectConversionConfig));
                    using (var input = File.OpenRead(configFile))
                    {
                        config = (ProjectConversionConfig)serializer.Deserialize(input);
                    }
                }
            }

            if (config == null)
            {
                config = new ProjectConversionConfig();
            }

            this.AddDefaultConfig(config, projectFile);
            this.configs.Add(key, config);
            return config;
        }

        private void AddDefaultConfig(ProjectConversionConfig config, string projectFile)
        {
            if (!projectFile.Contains("Utility.Compatibility"))
            {
                var projectDir = Path.GetDirectoryName(projectFile);
                if (projectDir == null)
                {
                    throw new DirectoryNotFoundException("Couldn't find directory of " + projectFile);
                }

                var compatibilityPath = @"Common\Utility\Source\Compatibility\Utility.Compatibility"
                                        + Definitions.CSharpProjectExtension;
                for (int i = 0; i < 10 && !File.Exists(Path.Combine(projectDir, compatibilityPath)); i++)
                {
                    compatibilityPath = "..\\" + compatibilityPath;
                }

                config.AdditionalProjectReferences.Add(compatibilityPath);
            }

            // add a reference to mscorlib (needed because we have NoStdLib set)
            config.AdditionalAssemblyReferences.Add("mscorlib");
        }
    }
}
