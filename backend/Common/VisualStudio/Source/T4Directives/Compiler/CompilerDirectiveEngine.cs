// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompilerDirectiveEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompilerDirectiveEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.Compiler
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.CSharp;
    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Engine for the compiler directive.
    /// </summary>
    public class CompilerDirectiveEngine : DirectiveEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilerDirectiveEngine"/> class.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public CompilerDirectiveEngine(ITextTemplatingEngineHost host)
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
                return "Compiler";
            }
        }

        /// <summary>
        /// Executes the directive.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The result.</returns>
        public BuildResult Execute(IDictionary<string, string> arguments)
        {
            this.WriteStart();
            string items;
            if (!arguments.TryGetValue("Items", out items))
            {
                throw new DirectiveProcessorException("Required argument 'Items' not specified.");
            }

            if (string.IsNullOrEmpty(items))
            {
                throw new ArgumentException("The arguments must contain a valid 'Items' string");
            }

            string assemblies;
            if (!arguments.TryGetValue("Assemblies", out assemblies))
            {
                assemblies = "System.dll";
            }

            var outputFile = Path.GetTempFileName();
            var fileInfo = new FileInfo(outputFile);

            var provider = new CSharpCodeProvider();
            var compilerparams = new CompilerParameters
                                     {
                                         GenerateExecutable = false,
                                         GenerateInMemory = false,
                                         OutputAssembly = outputFile
                                     };
            assemblies.Split(';').ToList().ForEach(assembly => compilerparams.ReferencedAssemblies.Add(assembly));
            var results = provider.CompileAssemblyFromFile(
                compilerparams,
                items.Split(';').Select(this.Host.ResolvePath).ToArray());
            if (results.Errors.Count > 0)
            {
                var errors =
                    results.Errors.OfType<CompilerError>()
                        .Select(error => error.ErrorText)
                        .Aggregate((s, s1) => s + "," + s1);
                throw new Exception(errors);
            }

            fileInfo.Refresh();

            return new BuildResult { Succeeded = true, Log = string.Empty, Output = new[] { outputFile } };
        }

        /// <summary>
        /// Results of the engine.
        /// </summary>
        public class BuildResult
        {
            /// <summary>
            /// Gets or sets a value indicating whether the compilation succeeded.
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
            /// <returns>
            /// The produced assemblies.
            /// </returns>
            public IEnumerable<Assembly> LoadAssemblies()
            {
                foreach (var assembly in this.Output)
                {
                    var assemblyBytes = File.ReadAllBytes(assembly);
                    yield return Assembly.Load(assemblyBytes);
                }
            }
        }
    }
}