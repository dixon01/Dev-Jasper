// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.XmlSolution;

    using NLog;

    /// <summary>
    /// Converter for Visual Studio solutions.
    /// </summary>
    public class SolutionConverter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string solutionFileVersion;
        private readonly string filePostfix;
        private readonly Version frameworkVersion;

        private XmlDocument solution;

        private SolutionConverter(string solutionFileVersion, string filePostfix, Version frameworkVersion)
        {
            this.solutionFileVersion = solutionFileVersion;
            this.filePostfix = filePostfix;
            this.frameworkVersion = frameworkVersion;
        }

        /// <summary>
        /// Creates a new <seealso cref="SolutionConverter"/>.
        /// </summary>
        /// <param name="visualStudioVersion">
        /// The Visual Studio version.
        /// </param>
        /// <param name="version">
        /// The framework version.
        /// </param>
        /// <returns>
        /// The <see cref="SolutionConverter"/>.
        /// </returns>
        public static SolutionConverter Create(VisualStudioVersion visualStudioVersion, FrameworkVersion version)
        {
            string solutionFileVersion;
            switch (visualStudioVersion)
            {
                case VisualStudioVersion.VisualStudio2008:
                    solutionFileVersion = "10.00";
                    break;
                default:
                    throw new NotSupportedException("Can't convert to " + visualStudioVersion);
            }

            switch (version)
            {
                case FrameworkVersion.CompactFramework20:
                    return new SolutionConverter(solutionFileVersion, ".CF20", new Version(2, 0));
                case FrameworkVersion.CompactFramework35:
                    return new SolutionConverter(solutionFileVersion, ".CF35", new Version(3, 5));
                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }

        /// <summary>
        /// Converts the given solution file.
        /// </summary>
        /// <param name="originalSolutionFile">
        /// The original solution file.
        /// </param>
        /// <param name="context">
        /// The conversion context.
        /// </param>
        public void Convert(string originalSolutionFile, IConversionContext context)
        {
            Logger.Info(
                "Converting {0} to version {1} for Visual Studio {2}",
                originalSolutionFile,
                this.frameworkVersion,
                this.solutionFileVersion);

            var solutionDir = Path.GetDirectoryName(originalSolutionFile);
            if (solutionDir == null)
            {
                throw new DirectoryNotFoundException("Couldn't find solution directory of " + originalSolutionFile);
            }

            this.solution = XmlSolutionConverter.CreateXml(originalSolutionFile);
            this.UpdateGlobalSolutionProperites();

            this.ConvertProjects(solutionDir, context);

            var updatedSolutionFile = Path.ChangeExtension(originalSolutionFile, this.filePostfix + ".sln");
            var outputFileInfo = new FileInfo(updatedSolutionFile);
            if (outputFileInfo.Exists)
            {
                outputFileInfo.Attributes = FileAttributes.Normal;
            }

            Logger.Info("Saving new solution file: {0}", updatedSolutionFile);
            using (var output = File.Create(updatedSolutionFile))
            {
                XmlSolutionConverter.CreateSolution(this.solution, output);
            }
        }

        private void ConvertProjects(string solutionDir, IConversionContext context)
        {
            var projects = this.solution.SelectNodes(
                string.Format("/Solution/Project[@type='{0}']", Definitions.CSharpProjectTypeGuid));
            if (projects == null)
            {
                return;
            }

            foreach (var project in projects.OfType<XmlElement>().ToList())
            {
                var projectFileName = project.GetAttribute("file");
                project.SetAttribute("name", project.GetAttribute("name") + this.filePostfix);

                var extension = string.Format("{0}{1}", this.filePostfix, Definitions.CSharpProjectExtension);
                var newProjectFileName = projectFileName.Replace(Definitions.CSharpProjectExtension, extension);
                project.SetAttribute("file", newProjectFileName);

                try
                {
                    var projectFilePath = Path.GetFullPath(Path.Combine(solutionDir, projectFileName));
                    if (this.ConvertProject(projectFilePath, context))
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WarnException("Couldn't convert " + projectFileName, ex);
                }

                this.RemoveProject(project);
            }
        }

        private bool ConvertProject(string projectFilePath, IConversionContext context)
        {
            var converter = new ProjectConverter(this.filePostfix, this.frameworkVersion);
            if (!converter.Convert(projectFilePath, context))
            {
                return false;
            }

            foreach (var project in converter.AdditionalProjects)
            {
                var guid = project.Key;
                var originalProject = project.Value;
                var convertedProject = originalProject.Replace(
                    Definitions.CSharpProjectExtension, this.filePostfix + Definitions.CSharpProjectExtension);
                if (this.AddProject(convertedProject, guid))
                {
                    this.ConvertProject(originalProject, context);
                }
            }

            return true;
        }

        private void UpdateGlobalSolutionProperites()
        {
            var root = this.solution.LastChild as XmlElement;
            if (root == null)
            {
                throw new XmlException("Couldn't find root element");
            }

            root.SetAttribute("version", this.solutionFileVersion);

            // remove entire <TeamFoundationVersionControl>
            var tfsVc = root.SelectSingleNode("//TeamFoundationVersionControl");
            if (tfsVc != null && tfsVc.ParentNode != null)
            {
                tfsVc.ParentNode.RemoveChild(tfsVc);
            }
        }

        private bool AddProject(string filePath, Guid guid)
        {
            var projectName = Path.GetFileNameWithoutExtension(filePath);
            var projectNode = this.solution.SelectSingleNode("//Project[@name='" + projectName + "']") as XmlElement;
            if (projectName == null || projectNode != null)
            {
                return false;
            }

            if (projectName.EndsWith(this.filePostfix))
            {
                var originalProject = projectName.Substring(0, projectName.Length - this.filePostfix.Length);
                projectNode = this.solution.SelectSingleNode(
                    "//Project[@name='" + originalProject + "']") as XmlElement;
                if (projectNode != null)
                {
                    return false;
                }
            }
            else
            {
                projectName += this.filePostfix;
            }

            var guidStr = guid.ToString("B").ToUpper();

            projectNode = this.solution.CreateElement("Project");
            projectNode.SetAttribute("version", Definitions.CSharpProjectTypeGuid);
            projectNode.SetAttribute("guid", guidStr);
            projectNode.SetAttribute("type", Definitions.CSharpProjectTypeGuid);
            projectNode.SetAttribute("name", projectName);
            projectNode.SetAttribute("file", filePath);

            var root = this.solution.SelectSingleNode("/Solution");
            var global = this.solution.SelectSingleNode("/Solution/Global");
            if (root == null)
            {
                throw new NotSupportedException("Couldn't find the root node");
            }

            root.InsertBefore(projectNode, global);

            var nestedProjects = this.solution.SelectSingleNode("//NestedProjects");
            var referencesXpath = string.Format(
                "//Project[@file='References' and @type='{0}']", Definitions.SolutionFolderTypeGuid);
            var references = this.solution.SelectSingleNode(referencesXpath) as XmlElement;
            if (nestedProjects != null && references != null)
            {
                var nestedProject = this.solution.CreateElement("NestedProject");
                nestedProject.SetAttribute("guid", guidStr);
                nestedProject.SetAttribute("parent", references.GetAttribute("guid"));
                nestedProjects.AppendChild(nestedProject);
            }

            var platformConfigs = this.solution.SelectNodes("//SolutionConfigurationPlatforms/SolutionConfiguration");
            var projectConfigs = this.solution.SelectSingleNode("//ProjectConfigurationPlatforms");
            if (platformConfigs == null || projectConfigs == null)
            {
                return true;
            }

            foreach (XmlElement platformConfig in platformConfigs)
            {
                this.AddProjectPlatform(platformConfig, guidStr, projectConfigs);
            }

            return true;
        }

        private void AddProjectPlatform(XmlElement platformConfig, string guid, XmlNode projectConfigs)
        {
            var type = platformConfig.GetAttribute("name");
            var value = platformConfig.GetAttribute("value");
            var parts = value.Split('|');
            if (parts.Length != 2)
            {
                return;
            }

            var projectConfig = this.solution.CreateElement("ProjectConfiguration");
            projectConfig.SetAttribute("guid", guid);
            projectConfig.SetAttribute("type", type);
            projectConfig.SetAttribute("extension", "ActiveCfg");
            projectConfig.SetAttribute("value", parts[0] + "|Any CPU");
            projectConfigs.AppendChild(projectConfig);

            if (parts[1] != "Any CPU" && parts[1] != "Mixed Platforms")
            {
                return;
            }

            projectConfig = this.solution.CreateElement("ProjectConfiguration");
            projectConfig.SetAttribute("guid", guid);
            projectConfig.SetAttribute("type", type);
            projectConfig.SetAttribute("extension", "Build.0");
            projectConfig.SetAttribute("value", parts[0] + "|Any CPU");
            projectConfigs.AppendChild(projectConfig);
        }

        private void RemoveProject(XmlElement project)
        {
            Logger.Info("Removing project {0}", project.GetAttribute("file"));
            if (project.ParentNode != null)
            {
                project.ParentNode.RemoveChild(project);
            }

            var guid = project.GetAttribute("guid");
            var configurations = this.solution.SelectNodes(string.Format("//ProjectConfiguration[@guid='{0}']", guid));
            if (configurations != null)
            {
                foreach (XmlElement configuration in configurations)
                {
                    if (configuration.ParentNode != null)
                    {
                        configuration.ParentNode.RemoveChild(configuration);
                    }
                }
            }

            var nested = this.solution.SelectSingleNode(string.Format("//NestedProject[@guid='{0}']", guid));
            if (nested != null && nested.ParentNode != null)
            {
                nested.ParentNode.RemoveChild(nested);
            }
        }
    }
}