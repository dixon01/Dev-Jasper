// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System;
    using System.IO;

    using NLog;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main function.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The exit value of the application.
        /// </returns>
        public static int Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    Logger.Error("Expected exactly one command line argument");
                    return -1;
                }

                var docxFile = Path.GetFullPath(args[0]);
                if (!File.Exists(docxFile))
                {
                    Logger.Error("Couldn't find file {0}", docxFile);
                    return -2;
                }

                var outputFile = Path.ChangeExtension(docxFile, ".g.docx");

                using (var documentor = new Documentor(docxFile))
                {
                    documentor.Generate(outputFile);
                }

                Logger.Info("Successfully generated {0}", outputFile);

                return 0;
            }
            catch (Exception ex)
            {
                Logger.FatalException("Couldn't generate XML documentation", ex);
                return int.MinValue;
            }
        }
    }
}
