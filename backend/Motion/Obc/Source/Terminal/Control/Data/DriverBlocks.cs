// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverBlocks.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DriverBlocks type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Data
{
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Csv;

    /// <summary>
    /// The driver blocks DA.
    /// </summary>
    internal static class DriverBlocks
    {
        /// <summary>
        /// Loads all driver blocks by day kind.
        /// </summary>
        /// <param name="dayKind">
        /// The day kind.
        /// </param>
        /// <returns>
        /// The list of driver blocks.
        /// </returns>
        public static List<string> LoadAllByDayKind(int dayKind)
        {
            var ret = new List<string>();

            var sfn = PathManager.Instance.GetPath(FileType.Database, "ser_agent.csv");
            if (sfn == null)
            {
                return ret;
            }

            using (var csvReader = new CsvReader(sfn))
            {
                string[] entry;
                while ((entry = csvReader.GetCsvLine()) != null)
                {
                    if (entry[1].Trim() == dayKind.ToString(CultureInfo.InvariantCulture))
                    {
                        string bd = entry[0].Trim();
                        if (!ret.Contains(bd))
                        {
                            ret.Add(bd);
                        }
                    }
                }
            }

            return ret;
        }
    }
}