// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator LTG" file="CsvFileReader.cs">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using System;
    using System.IO;

    using CsvHelper;
    using CsvHelper.Configuration;

    /// <summary>The csv file reader.</summary>
    public class CsvFileReader : CsvFileInfo
    {
        /// <summary>Initializes a new instance of the <see cref="CsvFileReader"/> class.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="csvReader">The csv reader.</param>
        /// <param name="csvConfiguration">The csv configuration.</param>
        /// <param name="csvClassMapType">The csv class map type.</param>
        public CsvFileReader(
            string fileName,
            FileStream fileStream,
            CsvReader csvReader,
            Configuration csvConfiguration,
            Type csvClassMapType = null)
            : base(fileName, fileStream, csvConfiguration, csvClassMapType)
        {
            this.CsvReader = csvReader;
        }

        /// <summary>Gets or sets the csv reader.</summary>
        public CsvReader CsvReader { get; set; }
    }
}