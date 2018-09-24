// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Documentor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Documentor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    using NLog;

    /// <summary>
    /// Class that updates all chapters of a word document that contain the @XmlDoc macro.
    /// The macro has to be placed in a heading and must have the following structure:
    /// <code>
    /// @XmlDoc(xsd=[relative path to XSD file])
    /// </code>
    /// or
    /// <code>
    /// @XmlDoc(xsd=[relative path to XSD file],xml=[relative path to example XML file])
    /// </code>
    /// </summary>
    public class Documentor : IDisposable
    {
        // ReSharper disable PossiblyMistakenUseOfParamsMethod
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string inputFilename;

        private WordprocessingDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="Documentor"/> class.
        /// </summary>
        /// <param name="inputFilename">
        /// The input filename. This should point to a valid Word document.
        /// </param>
        public Documentor(string inputFilename)
        {
            this.inputFilename = inputFilename;
        }

        /// <summary>
        /// Generates a new word document from given input file to the given output file.
        /// </summary>
        /// <param name="outputFilename">
        /// The output filename. This filename should end in <c>.docx</c>.
        /// </param>
        public void Generate(string outputFilename)
        {
            RemoveReadonlyAttribute(outputFilename);

            File.Copy(this.inputFilename, outputFilename, true);

            RemoveReadonlyAttribute(outputFilename);

            this.document = WordprocessingDocument.Open(outputFilename, true);

            this.UpdateDocumentSettings();

            var regex = new Regex(@"@\s*XmlDoc\s*\(([^)]*)");

            foreach (
                var paragraph in
                    this.document.MainDocumentPart.Document.Descendants<Paragraph>()
                        .Where(p => p.InnerText.Contains("XmlDoc")))
            {
                var match = regex.Match(paragraph.InnerText);
                if (!match.Success)
                {
                    Logger.Debug("Found invalid XmlDoc tag: {0}", paragraph.InnerText);
                    continue;
                }

                var args =
                    match.Groups[1].Value.Split(';')
                                   .Select(s => s.Split('='))
                                   .Where(s => s.Length == 2)
                                   .ToDictionary(
                                       s => s[0].Trim(), s => s[1].Trim(), StringComparer.InvariantCultureIgnoreCase);
                this.GenerateForParagraph(paragraph, args);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.document != null)
            {
                this.document.Close();
                this.document.Dispose();
            }
        }

        private static void RemoveReadonlyAttribute(string filename)
        {
            var outputFileInfo = new FileInfo(filename);
            if (outputFileInfo.Exists)
            {
                outputFileInfo.Attributes = outputFileInfo.Attributes & ~FileAttributes.ReadOnly;
            }
        }

        private string GetAbsolutePath(string filename)
        {
            var baseDirectory = Path.GetDirectoryName(this.inputFilename);
            return Path.GetFullPath(baseDirectory == null ? filename : Path.Combine(baseDirectory, filename));
        }

        private void UpdateDocumentSettings()
        {
            var settings = this.document.MainDocumentPart.DocumentSettingsPart;
            settings.Settings.Append(new UpdateFieldsOnOpen { Val = true });

            var randomPassword = new StringBuilder(15);
            var random = new Random();
            for (int i = 0; i < randomPassword.Capacity; i++)
            {
                randomPassword.Append((char)random.Next(32, 126));
            }

            new DocumentProtectionProvider(this.document).MakeReadOnly(randomPassword.ToString());
        }

        private void GenerateForParagraph(Paragraph paragraph, Dictionary<string, string> args)
        {
            string xsdFile;
            if (!args.TryGetValue("xsd", out xsdFile))
            {
                Logger.Warn("Missing xsd parameter in {0}", paragraph.InnerText);
                return;
            }

            xsdFile = this.GetAbsolutePath(xsdFile);
            if (!File.Exists(xsdFile))
            {
                Logger.Warn("xsd parameter is not pointing to an existing file: {0}", xsdFile);
                return;
            }

            string xmlFile;
            if (args.TryGetValue("xml", out xmlFile))
            {
                xmlFile = this.GetAbsolutePath(xmlFile);
            }

            var generator = new DocumentationGenerator(xsdFile, xmlFile);
            generator.GenerateChapters(paragraph);
        }

        // ReSharper restore PossiblyMistakenUseOfParamsMethod
    }
}