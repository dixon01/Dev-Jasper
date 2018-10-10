// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="CsvFileHelperFactory.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;
    using System.IO;

    using CsvHelper;
    using CsvHelper.Configuration;

    using NLog;

    /// <summary>
    ///     The csv file helper factory.
    /// </summary>
    public static class CsvFileHelperFactory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>The create.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="cvsClassMap">The cvs class map type.</param>
        /// <typeparam name="T">Class type to use for file input output.</typeparam>
        /// <returns>The <see cref="CsvFileHelper"/>.</returns>
        public static CsvFileHelper<T> Create<T>(string fileName, Type cvsClassMap = null)
            where T : class
        {
            var writer = CreateCsvWriter(fileName, new MyDefaultCsvConfiguration(), FileMode.Open, FileAccess.ReadWrite, cvsClassMap);
            return new CsvFileHelper<T>(writer);
        }

        /// <summary>The create csv reader.</summary>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="csvClassMapType">The csv class map type.</param>
        /// <returns>The <see cref="CsvFileReader"/>.</returns>
        public static CsvFileReader CreateCsvReader(
            Configuration csvConfiguration,
            string fileName,
            FileStream stream,
            Type csvClassMapType = null)
        {
            Logger.Info("Open File Stream {0}", stream.Name);
            var csvReader = new CsvReader(new StreamReader(stream), csvConfiguration ?? new MyDefaultCsvConfiguration());
            var csvFileWritter = new CsvFileReader(fileName, stream, csvReader, csvConfiguration, csvClassMapType);
            return csvFileWritter;
        }

        /// <summary>The create csv writer.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">File Access</param>
        /// <param name="csvClassMapType">The csv Class Map Type.</param>
        /// <returns>The <see cref="CsvHelper.CsvWriter"/>.</returns>
        public static CsvFileWriter CreateCsvWriter(
            string fileName,
            Configuration csvConfiguration = null,
            FileMode fileMode = FileMode.Open,
            FileAccess fileAccess = FileAccess.ReadWrite,
            Type csvClassMapType = null)
        {
            if (File.Exists(fileName))
            {
                fileAccess = FileAccess.ReadWrite;
            }

            var directoryName = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directoryName))
            {
                if (!Directory.Exists(directoryName))
                {
                    Logger.Warn("Directory Not Found {0}, Creating..,", directoryName);
                    Directory.CreateDirectory(directoryName);
                }
            }

            var fileSteam = new FileStream(fileName, fileMode, fileAccess, FileShare.ReadWrite);
            return CreateCsvWriter(csvConfiguration, fileName, fileSteam, csvClassMapType);
        }

        /// <summary>The create csv writer.</summary>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="csvClassMapType">The csv class map type.</param>
        /// <returns>The <see cref="CsvFileWriter"/>.</returns>
        public static CsvFileWriter CreateCsvWriter(
            Configuration csvConfiguration,
            string fileName,
            FileStream stream,
            Type csvClassMapType = null)
        {
            Logger.Info("Open File Stream {0}", stream.Name);
            var streamWriter = new StreamWriter(stream);
            streamWriter.AutoFlush = true;
            var csvWriter = new CsvWriter(streamWriter, csvConfiguration ?? new MyDefaultCsvConfiguration());
            var csvFileWritter = new CsvFileWriter(fileName, stream, csvWriter, csvConfiguration, csvClassMapType);
            return csvFileWritter;
        }
    }
}