// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionConverterOptions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionConverterOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using CommandLineParser.Arguments;

    /// <summary>
    /// The command line options for the solution converter.
    /// </summary>
    public class SolutionConverterOptions
    {
        /// <summary>
        /// Gets or sets the target .NET framework.
        /// </summary>
        [EnumeratedValueArgument(typeof(string), 't', "Target", AllowedValues = "CF20;CF35", DefaultValue = "CF35",
            Description = "Target .NET framework and version")]
        public string TargetFramework { get; set; }

        /// <summary>
        /// Gets or sets the target Visual Studio version.
        /// </summary>
        [EnumeratedValueArgument(typeof(string), 'v', "VisualStudio", AllowedValues = "2005;2008;2010;2012",
            DefaultValue = "2008", Description = "Target Visual Studio version")]
        public string VisualStudioVersion { get; set; }
    }
}
