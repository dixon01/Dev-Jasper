// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportProjectDirectiveEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportProjectDirectiveEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.ImportProject
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Gorba.Common.VisualStudio.T4Directives.Utility;

    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// A directive to build projects.
    /// </summary>
    public class ImportProjectDirectiveEngine : DirectiveEngine
    {
        private const string DefaultConfiguration = "Release";

        private const string DefaultPlatform = "Any CPU";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportProjectDirectiveEngine"/> class.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public ImportProjectDirectiveEngine(ITextTemplatingEngineHost host)
            : base(host)
        {
        }

        /// <summary>
        /// Gets the processor name.
        /// </summary>
        public override string ProcessorName
        {
            get
            {
                return "MSBuild";
            }
        }

        /// <summary>
        /// Executes the directive.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="BuildResult"/>.
        /// </returns>
        /// <exception cref="DirectiveProcessorException">An internal error occurred.</exception>
        public BuildResult Execute(IDictionary<string, string> arguments)
        {
            string projects;
            if (!arguments.TryGetValue("Projects", out projects))
            {
                throw new DirectiveProcessorException("Required argument 'Projects' not specified.");
            }

            string references;
            if (!arguments.TryGetValue("References", out references))
            {
                references = string.Empty;
            }

            string configuration;
            if (!arguments.TryGetValue("Configuration", out configuration))
            {
                configuration = DefaultConfiguration;
            }

            string platform;
            if (!arguments.TryGetValue("Platform", out platform))
            {
                platform = DefaultPlatform;
            }

            string outputPath;
            if (!arguments.TryGetValue("OutputPath", out outputPath))
            {
                outputPath = GetOutputPath();
            }

            var projectCollection = new ProjectCollection();
            if (!string.IsNullOrEmpty(references))
            {
                this.LoadProjects(references, projectCollection);
            }

            var buildLogger = new BuildLogger();
            var buildParameters = new BuildParameters(projectCollection)
                                      {
                                          Loggers = new[] { buildLogger }
                                      };
            var projectToBuild = this.Host.ResolvePath(projects);
            IDictionary<string, string> globalProperties = new Dictionary<string, string>
                                                               {
                                                                   {
                                                                       "Configuration",
                                                                       configuration
                                                                   },
                                                                   { "Platform", platform },
                                                                   {
                                                                       "OutputPath", outputPath
                                                                   }
                                                               };
            var requestData = new BuildRequestData(
                projectToBuild,
                globalProperties,
                null,
                new[] { "Rebuild" },
                null);
            var buildResult = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(
                buildParameters,
                requestData);
            if (buildResult.OverallResult != BuildResultCode.Success)
            {
                return new BuildResult();
            }

            return new BuildResult
                             {
                                 Succeeded = true,
                                 Log = buildLogger.GetOutput(),
                                 Output =
                                     buildResult.ResultsByTarget["Rebuild"].Items.Select(
                                         item => item.ToString()).ToArray(),
                                 OutputPath = outputPath
                             };
        }

        private static string GetOutputPath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var outputPath = Path.Combine(appData, "Gorba AG", "ImportProjectT4Directive", "OutputPath");
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            Directory.CreateDirectory(outputPath);
            return outputPath;
        }

        private void LoadProjects(string references, ProjectCollection projectCollection)
        {
            var visualStudioProjects = references.Split(';').Select(this.GetProject).ToList();
            if (visualStudioProjects.Count <= 1)
            {
                return;
            }

            Action<string> loadProject = p => { var prj = projectCollection.LoadProject(p); };
            visualStudioProjects.Take(visualStudioProjects.Count - 1).ToList().ForEach(loadProject);
        }

        private string GetProject(string arg)
        {
            return arg.EndsWith(".csproj") ? this.Host.ResolvePath(arg) : arg;
        }

        /// <summary>
        /// Defines the result of the build.
        /// </summary>
        public class BuildResult
        {
            /// <summary>
            /// Gets or sets a value indicating whether the build was successful.
            /// </summary>
            public bool Succeeded { get; set; }

            /// <summary>
            /// Gets or sets the log.
            /// </summary>
            public string Log { get; set; }

            /// <summary>
            /// Gets or sets the output.
            /// </summary>
            public string[] Output { get; set; }

            /// <summary>
            /// Gets or sets the output path.
            /// </summary>
            public string OutputPath { get; set; }

            /// <summary>
            /// Loads the assemblies.
            /// </summary>
            public void LoadAssemblies()
            {
                foreach (var assembly in this.Output)
                {
                    var assemblyBytes = File.ReadAllBytes(assembly);
                    Assembly.Load(assemblyBytes);
                }
            }
        }
    }
}