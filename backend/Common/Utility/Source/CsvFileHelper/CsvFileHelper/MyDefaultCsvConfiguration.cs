// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyDefaultCsvConfiguration.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Utility.CsvFileHelper
{
    using CsvHelper.Configuration;

    /// <summary>The default csv configuration.</summary>
    internal sealed class MyDefaultCsvConfiguration : Configuration
    {
        /// <summary>Initializes a new instance of the <see cref="MyDefaultCsvConfiguration" /> class.</summary>
        public MyDefaultCsvConfiguration()
        {
            this.HasHeaderRecord = true;
            this.IncludePrivateMembers = false;
            this.Delimiter = ",";
        }
    }
}