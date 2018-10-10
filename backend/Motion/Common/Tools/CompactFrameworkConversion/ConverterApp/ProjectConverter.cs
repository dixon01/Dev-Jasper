// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    using Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.Config;

    using NLog;

    /// <summary>
    /// Converter for C# projects.
    /// </summary>
    internal class ProjectConverter
    {
        private const string DefaultNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<string> FrameworkDlls = new List<string>
                                                                 {
                                                                     "System",
                                                                     "System.Data",
                                                                     "System.Drawing",
                                                                     "System.Windows.Forms",
                                                                     "System.Xml"
                                                                 };

        private static readonly Regex PlatformSpecificExtensionRegex = new Regex(@"\.(CF|FX)\d\d\.");

        private readonly string filePostfix;

        private readonly Version frameworkVersion;

        private ProjectConversionConfig config;

        private XmlDocument document;

        private XmlNamespaceManager nsmgr;

        private string projectDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectConverter"/> class.
        /// </summary>
        /// <param name="filePostfix">
        /// The file postfix including the leading dot.
        /// </param>
        /// <param name="frameworkVersion">
        /// The framework version.
        /// </param>
        public ProjectConverter(string filePostfix, Version frameworkVersion)
        {
            this.filePostfix = filePostfix;
            this.frameworkVersion = frameworkVersion;

            this.AdditionalProjects = new Dictionary<Guid, string>();
        }

        /// <summary>
        /// Gets the list of additional projects added by this converter and their GUID.
        /// </summary>
        public IDictionary<Guid, string> AdditionalProjects { get; private set; }

        /// <summary>
        /// Converts the given file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="context">
        /// The conversion context.
        /// </param>
        /// <returns>
        /// True if the project was converted, otherwise false.
        /// </returns>
        public bool Convert(string fileName, IConversionContext context)
        {
            if (!fileName.EndsWith(".csproj"))
            {
                throw new NotSupportedException("Project file extension not supported: " + fileName);
            }

            Logger.Info("Converting project {0} to version {1}", fileName, this.frameworkVersion);
            this.config = context.GetProjectConfig(fileName, this.filePostfix);
            if (this.config.IgnoreProject)
            {
                return false;
            }

            this.projectDirectory = Path.GetDirectoryName(fileName);
            if (this.projectDirectory == null)
            {
                throw new DirectoryNotFoundException("Couldn't find project directory");
            }

            this.document = new XmlDocument();
            this.document.Load(fileName);

            this.nsmgr = new XmlNamespaceManager(this.document.NameTable);
            this.nsmgr.AddNamespace("m", DefaultNamespace);

            this.ConvertXml(context);

            var newFileName = Path.ChangeExtension(fileName, this.filePostfix + Definitions.CSharpProjectExtension);
            Logger.Info("Saving new project file: {0}", newFileName);
            this.document.Save(newFileName);
            return true;
        }

        private static void CleanNode(XmlNode node)
        {
            for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
            {
                var childNode = node.ChildNodes[i];
                CleanNode(childNode);
                if (node.Name != "PropertyGroup")
                {
                    continue;
                }

                // all source control and Code Contracts  tags can be removed from <PropertyGroup>s
                if (childNode.Name.StartsWith("Scc") || childNode.Name.StartsWith("CodeContracts"))
                {
                    node.RemoveChild(childNode);
                }
            }
        }

        private void ConvertXml(IConversionContext context)
        {
            // remove all unused tags throughout the entire document (recursive)
            CleanNode(this.document);

            var projectNode = this.document.SelectSingleNode("/m:Project", this.nsmgr) as XmlElement;
            if (projectNode == null)
            {
                throw new XPathException("Couldn't find root Project node");
            }

            // update the tool version to the new CF version
            projectNode.SetAttribute("ToolsVersion", "3.5");

            this.UpdateDefaultPropertyGroup();

            this.UpdatePlatformPropertyGroups();

            var referencesItemGroup = this.UpdateDllReferences();

            var compileItemGroup = this.RemoveSpecificSourceFiles();
            if (compileItemGroup == null)
            {
                compileItemGroup = this.document.CreateElement("ItemGroup");
                projectNode.InsertAfter(compileItemGroup, referencesItemGroup);
            }

            this.AddSpecificSourceFiles(this.projectDirectory, string.Empty, compileItemGroup);

            var projectReferencesItemGroup =
                this.document.SelectSingleNode("/m:Project/m:ItemGroup[m:ProjectReference]", this.nsmgr) as XmlElement;
            if (projectReferencesItemGroup == null)
            {
                projectReferencesItemGroup = this.document.CreateElement("ItemGroup", DefaultNamespace);
                projectNode.InsertAfter(projectReferencesItemGroup, compileItemGroup);
            }
            else
            {
                this.UpdateProjectReferences(projectReferencesItemGroup, context);
            }

            foreach (var reference in this.config.AdditionalProjectReferences)
            {
                this.AddProjectReference(Path.Combine(this.projectDirectory, reference), projectReferencesItemGroup);
            }

            var nodes = this.document.SelectNodes("/m:Project/m:Import", this.nsmgr);
            if (nodes == null)
            {
                return;
            }

            foreach (var import in nodes.OfType<XmlElement>().Where(
                i => i.GetAttribute("Project").EndsWith("Microsoft.CSharp.targets")))
            {
                // replace the target file used
                import.SetAttribute("Project", @"$(MSBuildBinPath)\Microsoft.CompactFramework.CSharp.targets");
            }
        }

        private void AddProjectReference(string projectPath, XmlElement projectReferencesItemGroup)
        {
            var originalProject = projectPath;
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            if (projectName == null)
            {
                return;
            }

            projectName += this.filePostfix;
            projectPath = projectPath.Replace(
                Definitions.CSharpProjectExtension, this.filePostfix + Definitions.CSharpProjectExtension);

            foreach (XmlElement child in projectReferencesItemGroup.ChildNodes)
            {
                // update the Include attribute to point to the .CF35 version of the project file
                var name = child.SelectSingleNode("m:Name/text()", this.nsmgr) as XmlText;
                if (name != null && projectName.StartsWith(name.Value))
                {
                    // the reference was found, let's not add it again
                    return;
                }
            }

            Logger.Debug("Adding reference to project '{0}': {1}", projectName, projectPath);
            var xml = new XmlDocument();
            xml.Load(originalProject);

            var guidText = xml.SelectSingleNode("//m:ProjectGuid/text()", this.nsmgr) as XmlText;
            if (guidText == null)
            {
                throw new NotSupportedException("Couldn't get project GUID of " + projectPath);
            }

            Logger.Trace("GUID for project '{0}' is {1}", projectName, guidText.Value);

            this.AdditionalProjects[new Guid(guidText.Value)] = originalProject;

            // add a new <ProjectReference> tag pointing to the compatibility project
            var projectReference = this.document.CreateElement("ProjectReference", DefaultNamespace);
            projectReference.SetAttribute("Include", projectPath);
            projectReference.AppendChild(this.CreateTextElement("Project", guidText.Value));
            projectReference.AppendChild(this.CreateTextElement("Name", projectName));
            projectReferencesItemGroup.InsertBefore(projectReference, projectReferencesItemGroup.FirstChild);
        }

        private void UpdateProjectReferences(XmlElement projectReferencesItemGroup, IConversionContext context)
        {
            var extension = this.filePostfix + Definitions.CSharpProjectExtension;
            for (int i = projectReferencesItemGroup.ChildNodes.Count - 1; i >= 0; i--)
            {
                // update the Include attribute to point to the .CF35 version of the project file
                var reference = (XmlElement)projectReferencesItemGroup.ChildNodes[i];
                var include = reference.GetAttribute("Include");
                var refConfig = context.GetProjectConfig(
                    Path.Combine(this.projectDirectory, include), this.filePostfix);
                if (refConfig.IgnoreProject)
                {
                    // remove the project reference if it should be ignored
                    Logger.Debug("Removing project reference {0}", include);
                    projectReferencesItemGroup.RemoveChild(reference);
                    continue;
                }

                reference.SetAttribute("Include", include.Replace(Definitions.CSharpProjectExtension, extension));
                Logger.Trace("Updated project reference {0}", reference.GetAttribute("Include"));

                var name = reference.SelectSingleNode("m:Name/text()", this.nsmgr) as XmlText;
                if (name == null)
                {
                    continue;
                }

                name.Value += this.filePostfix;
            }
        }

        private XmlNode UpdateDllReferences()
        {
            var referencesItemGroup =
                this.document.SelectSingleNode("/m:Project/m:ItemGroup[m:Reference]", this.nsmgr) as XmlElement;
            if (referencesItemGroup == null)
            {
                return null;
            }

            // update hint paths and remove references that don't exist in CF
            for (int i = referencesItemGroup.ChildNodes.Count - 1; i >= 0; i--)
            {
                var reference = referencesItemGroup.ChildNodes[i] as XmlElement;
                if (reference == null)
                {
                    continue;
                }

                var hintPath = reference.SelectSingleNode("m:HintPath/text()", this.nsmgr);
                if (hintPath != null)
                {
                    // update hint path
                    var newPath = this.UpdateDllReference(hintPath.Value);
                    if (newPath != null)
                    {
                        hintPath.Value = newPath;
                        reference.SetAttribute("Include", Path.GetFileNameWithoutExtension(newPath));
                        Logger.Debug("Updated reference to {0}", newPath);
                    }
                    else
                    {
                        referencesItemGroup.RemoveChild(reference);
                        Logger.Debug("Removed reference to {0}", hintPath.Value);
                    }
                }
                else
                {
                    // check if we know this DLL
                    var include = reference.GetAttribute("Include");
                    if (!FrameworkDlls.Contains(include))
                    {
                        referencesItemGroup.RemoveChild(reference);
                        Logger.Debug("Removed framework reference: {0}", include);
                    }
                }
            }

            foreach (var fileName in this.config.AdditionalAssemblyReferences)
            {
                var reference = this.document.CreateElement("Reference", DefaultNamespace);
                reference.SetAttribute("Include", Path.GetFileNameWithoutExtension(fileName));

                if (!string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
                {
                    var hintPath = this.document.CreateElement("HintPath", DefaultNamespace);
                    hintPath.AppendChild(this.document.CreateTextNode(fileName));
                    reference.AppendChild(hintPath);
                }

                referencesItemGroup.InsertBefore(reference, referencesItemGroup.FirstChild);
                Logger.Debug("Added reference to {0}", fileName);
            }

            return referencesItemGroup;
        }

        private XmlElement RemoveSpecificSourceFiles()
        {
            var compiles = this.document.SelectNodes("/m:Project/m:ItemGroup/m:Compile", this.nsmgr);
            if (compiles == null)
            {
                return null;
            }

            XmlElement compileItemGroup = null;
            foreach (XmlElement compile in compiles)
            {
                if (!compile.HasAttribute("Include"))
                {
                    continue;
                }

                if (compileItemGroup == null)
                {
                    compileItemGroup = compile.ParentNode as XmlElement;
                }

                var include = compile.GetAttribute("Include");
                if (compile.ParentNode != null && PlatformSpecificExtensionRegex.IsMatch(include))
                {
                    // if the file name contains a target specific file extension, we have to remove it
                    compile.ParentNode.RemoveChild(compile);
                    Logger.Debug("Removed source file {0}", include);
                }
            }

            return compileItemGroup;
        }

        private void UpdateDefaultPropertyGroup()
        {
            var defaultPropertyGroup = this.document.SelectSingleNode(
                "/m:Project/m:PropertyGroup[not(@*)]", this.nsmgr);
            if (defaultPropertyGroup == null)
            {
                return;
            }

            var productVersion = defaultPropertyGroup.SelectSingleNode("m:ProductVersion/text()", this.nsmgr);
            if (productVersion != null)
            {
                // new product version for VS2008
                productVersion.Value = "9.0.30729";
            }

            var assemblyName = defaultPropertyGroup.SelectSingleNode("m:AssemblyName/text()", this.nsmgr);
            if (assemblyName != null)
            {
                // the assembly name needs the .CF35 extension
                assemblyName.Value += this.filePostfix;

                // we can use the original assembly name directly as the deployment directory suffix
                defaultPropertyGroup.AppendChild(this.CreateTextElement("DeployDirSuffix", assemblyName.Value));
            }

            var targetFramework = defaultPropertyGroup.SelectSingleNode("m:TargetFrameworkVersion/text()", this.nsmgr);
            if (targetFramework != null)
            {
                if (targetFramework.Value != "v2.0")
                {
                    throw new NotSupportedException(
                        "Original target framework not supported: " + targetFramework.Value);
                }

                // change the target framework version
                targetFramework.Value = "v" + this.frameworkVersion;
            }

            // add some elements needed for CF 3.5 to the default property group
            defaultPropertyGroup.AppendChild(
                this.CreateTextElement(
                    "ProjectTypeGuids",
                    "{4D628B5B-2FBC-4AA6-8C16-197242AEB884};" + Definitions.CSharpProjectTypeGuid));
            defaultPropertyGroup.AppendChild(this.CreateTextElement("PlatformFamilyName", "WindowsCE"));
            defaultPropertyGroup.AppendChild(this.CreateTextElement("NativePlatformName", "Windows CE"));
            defaultPropertyGroup.AppendChild(
                this.CreateTextElement("PlatformID", "E2BECB1F-8C8C-41ba-B736-9BE7D946A398"));
            defaultPropertyGroup.AppendChild(this.CreateTextElement("OSVersion", "5.0"));
            defaultPropertyGroup.AppendChild(this.CreateTextElement("FormFactorID", string.Empty));
        }

        private void UpdatePlatformPropertyGroups()
        {
            var platformPropertyGroups = this.document.SelectNodes("/m:Project/m:PropertyGroup[@*]", this.nsmgr);
            if (platformPropertyGroups == null)
            {
                return;
            }

            var outputPathReplace = string.Format("bin\\{0}\\", this.filePostfix).Replace(".", string.Empty);
            foreach (XmlNode platformPropertyGroup in platformPropertyGroups)
            {
                var defConsts = platformPropertyGroup.SelectSingleNode("m:DefineConstants/text()", this.nsmgr);
                if (defConsts != null)
                {
                    // the constants should also contain the platform family name ("WindowsCE")
                    defConsts.Value += ";$(PlatformFamilyName)";
                }

                var outputPath = platformPropertyGroup.SelectSingleNode("m:OutputPath/text()", this.nsmgr);
                if (outputPath != null)
                {
                    outputPath.Value = outputPath.Value.Replace("bin\\", outputPathReplace);
                }

                // set some platform specific properties
                platformPropertyGroup.AppendChild(this.CreateTextElement("NoStdLib", "true"));
                platformPropertyGroup.AppendChild(this.CreateTextElement("NoConfig", "true"));
                platformPropertyGroup.AppendChild(this.CreateTextElement("GenerateSerializationAssemblies", "Off"));
            }
        }

        private void AddSpecificSourceFiles(string dir, string relativePath, XmlElement compleItemGroup)
        {
            foreach (var file in Directory.GetFiles(dir, "*" + this.filePostfix + ".*"))
            {
                // TODO: support more extensions
                if (!file.EndsWith(".cs"))
                {
                    continue;
                }

                // add this target specific file with the right path
                var filePath = relativePath + Path.GetFileName(file);
                Logger.Debug("Adding source file {0}", filePath);

                var compile = this.document.CreateElement("Compile", DefaultNamespace);
                compile.SetAttribute("Include", filePath);
                compleItemGroup.AppendChild(compile);
                var dependentName = filePath.Replace(this.filePostfix + ".", ".");
                var dependent = compleItemGroup.SelectSingleNode(
                    string.Format("m:Compile[@Include='{0}']", dependentName), this.nsmgr);
                if (dependent != null)
                {
                    var dependentUpon = this.document.CreateElement("DependentUpon", DefaultNamespace);
                    dependentUpon.AppendChild(this.document.CreateTextNode(Path.GetFileName(dependentName)));
                    compile.AppendChild(dependentUpon);
                }
            }

            foreach (var directory in Directory.GetDirectories(dir))
            {
                // do the same for all subdirectories
                this.AddSpecificSourceFiles(
                    directory, relativePath + Path.GetFileName(directory) + "\\", compleItemGroup);
            }
        }

        private string UpdateDllReference(string reference)
        {
            if (reference.Contains(".netfx20\\"))
            {
                return reference.Replace(".netfx20\\", ".netcf35\\");
            }

            if (reference.Contains("\\net-20\\"))
            {
                return reference.Replace("\\net-20\\", "\\netcf-35\\");
            }

            if (reference.EndsWith("\\Microsoft.Practices.ServiceLocation.dll"))
            {
                return reference.Replace(
                    "\\Microsoft.Practices.ServiceLocation.dll", "-cf35\\Microsoft.Practices.ServiceLocation.dll");
            }

            if (reference.EndsWith("\\Unity\\v1.2\\Microsoft.Practices.Unity.dll"))
            {
                return reference.Replace(
                    "\\Unity\\v1.2\\Microsoft.Practices.Unity.dll",
                    "\\CompactContainer\\1.0\\CompactContainer.dll");
            }

            if (reference.Contains("\\Unity\\v1.2\\"))
            {
                return null;
            }

            if (reference.EndsWith("\\NAudio.dll"))
            {
                return null;
            }

            if (reference.Contains("\\Interop."))
            {
                return null;
            }

            // TODO: support more references
            throw new NotSupportedException("Couldn't find Compact Framework version of " + reference);
        }

        private XmlElement CreateTextElement(string elementName, string text)
        {
            var element = this.document.CreateElement(elementName, DefaultNamespace);
            element.AppendChild(this.document.CreateTextNode(text));
            return element;
        }
    }
}