// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using System;
    using System.IO;

    using CommandLineParser;
    using CommandLineParser.Exceptions;

    using NLog;

    /// <summary>
    /// The main application.
    /// </summary>
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The exit code.
        /// </returns>
        public static int Main(string[] args)
        {
            var options = new SolutionConverterOptions();
            var parser = new CommandLineParser();
            try
            {
                parser.AdditionalArgumentsSettings.AcceptAdditionalArguments = true;
                parser.AdditionalArgumentsSettings.RequestedAdditionalArgumentsCount = 1;
                parser.ExtractArgumentAttributes(options);
                parser.ParseCommandLine(args);
            }
            catch (CommandLineException ex)
            {
                Console.WriteLine(ex.Message);
                parser.ShowUsage();
                return -1;
            }

            if (parser.AdditionalArgumentsSettings.AdditionalArguments.Length != 1)
            {
                Console.WriteLine("Provide solution name as argument");
                return -1;
            }

            FrameworkVersion frameworkVersion;
            switch (options.TargetFramework)
            {
                case "CF20":
                    frameworkVersion = FrameworkVersion.CompactFramework20;
                    break;
                case "CF35":
                    frameworkVersion = FrameworkVersion.CompactFramework35;
                    break;
                default:
                    Console.WriteLine("Invalid target framework: " + options.TargetFramework);
                    parser.ShowUsage();
                    return -1;
            }

            VisualStudioVersion visualStudioVersion;
            switch (options.VisualStudioVersion)
            {
                case "2005":
                    visualStudioVersion = VisualStudioVersion.VisualStudio2005;
                    break;
                case "2008":
                    visualStudioVersion = VisualStudioVersion.VisualStudio2008;
                    break;
                case "2010":
                    visualStudioVersion = VisualStudioVersion.VisualStudio2010;
                    break;
                case "2012":
                    visualStudioVersion = VisualStudioVersion.VisualStudio2012;
                    break;
                default:
                    Console.WriteLine("Invalid target Visual Studio version: " + options.VisualStudioVersion);
                    parser.ShowUsage();
                    return -1;
            }

            var originalSolutionFile = Path.GetFullPath(parser.AdditionalArgumentsSettings.AdditionalArguments[0]);

            var converter = SolutionConverter.Create(visualStudioVersion, frameworkVersion);
            var context = new ConversionContext();
            try
            {
                converter.Convert(originalSolutionFile, context);
                return 0;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't convert solution", ex);
                return -2;
            }
        }
    }
}
