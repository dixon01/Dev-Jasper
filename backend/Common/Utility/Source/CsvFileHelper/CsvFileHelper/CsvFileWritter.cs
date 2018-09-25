// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvFileWritter.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;
    using System.IO;

    using CsvHelper;
    using CsvHelper.Configuration;

    /// <summary>The csv file writer.</summary>
    public class CsvFileWriter : CsvFileInfo
    {
        /// <summary>Initializes a new instance of the <see cref="CsvFileWriter"/> class.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="csvWriter">The csv writer.</param>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="csvClassMapType">The csv class map type.</param>
        public CsvFileWriter(
            string fileName,
            FileStream fileStream,
            CsvWriter csvWriter,
            Configuration csvConfiguration,
            Type csvClassMapType = null)
            : base(fileName, fileStream, csvConfiguration, csvClassMapType)
        {
            this.CsvWriter = csvWriter;
        }

        /// <summary>Gets or sets the csv writer.</summary>
        public CsvWriter CsvWriter { get; set; }
    }
}