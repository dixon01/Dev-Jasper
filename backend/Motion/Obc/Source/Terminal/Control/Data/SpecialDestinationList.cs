// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialDestinationList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SpecialDestinationList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The special destination list.
    /// </summary>
    internal class SpecialDestinationList
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SpecialDestinationList>();

        private readonly List<string[]> entries = new List<string[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialDestinationList"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public SpecialDestinationList(string filename)
        {
            var path = PathManager.Instance.GetPath(FileType.Database, filename);
            if (path == null)
            {
                Logger.Warn("Couldn't find {0}", filename);
                return;
            }

            var csvReader = new CsvReader(path);

            string[] entry;
            while ((entry = csvReader.GetCsvLine()) != null)
            {
                this.entries.Add(entry);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.entries.Count;
            }
        }

        private static int LangIndex
        {
            get
            {
                return LanguageManager.Instance.CurrentLanguage.Number;
            }
        }

        /// <summary>
        /// Gets all destination texts.
        /// </summary>
        /// <returns>
        /// The list of destination texts.
        /// </returns>
        public List<string> GetAllDestinationText()
        {
            var destEntries = new List<string>();
            foreach (var entry in this.entries)
            {
                destEntries.Add(entry[LangIndex + 1]);
            }

            return destEntries;
        }

        /// <summary>
        /// Gets the block number for the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The block number.
        /// </returns>
        public int GetBlockNumber(int index)
        {
            try
            {
                return int.Parse(this.entries[index][0]);
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Block number invalid", ex); // MLHIDE
                return -1;
            }
        }

        /// <summary>
        /// Gets the destination code for the given block number.
        /// </summary>
        /// <param name="blockNbr">
        /// The block number.
        /// </param>
        /// <returns>
        /// The destination code.
        /// </returns>
        public int GetDestinationCode(int blockNbr)
        {
            return this.GetDestinationCode(blockNbr.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the destination code for the given block number.
        /// </summary>
        /// <param name="blockNbr">
        /// The block number.
        /// </param>
        /// <returns>
        /// The destination code.
        /// </returns>
        public int GetDestinationCode(string blockNbr)
        {
            foreach (var entry in this.entries)
            {
                if (entry[0].Equals(blockNbr))
                {
                    return int.Parse(entry[1]);
                }
            }

            return 0;
        }

        /// <summary>
        /// Checks if the given service number exists.
        /// </summary>
        /// <param name="blockNbr">
        /// The block number.
        /// </param>
        /// <returns>
        /// True if found.
        /// </returns>
        public bool ExistServiceNumber(string blockNbr)
        {
            foreach (var entry in this.entries)
            {
                if (entry[0].Equals(blockNbr))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the text for the given block number.
        /// </summary>
        /// <param name="blockNbr">
        /// The block number.
        /// </param>
        /// <returns>
        /// The text.
        /// </returns>
        public string GetText(int blockNbr)
        {
            return this.GetText(blockNbr.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the text for the given block number.
        /// </summary>
        /// <param name="block">
        /// The block number.
        /// </param>
        /// <returns>
        /// The text.
        /// </returns>
        public string GetText(string block)
        {
            foreach (var entry in this.entries)
            {
                if (entry[0].Equals(block))
                {
                    return entry[LangIndex + 1];
                }
            }

            return string.Empty;
        }
    }
}