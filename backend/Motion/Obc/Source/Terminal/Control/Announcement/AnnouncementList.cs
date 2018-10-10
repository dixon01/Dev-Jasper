// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnouncementList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Announcement
{
    using System.Collections.Generic;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Csv;

    /// <summary>
    /// Represents the announcement.csv file
    /// </summary>
    internal class AnnouncementList
    {
        private readonly List<string[]> entries;
        private readonly int posDriverText;
        private readonly int posIdValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementList"/> class.
        /// </summary>
        /// <param name="file">
        /// The file name.
        /// </param>
        /// <param name="posIdValue">
        /// The position of the id value.
        /// </param>
        /// <param name="posDriverText">
        /// The position of the driver text.
        /// </param>
        public AnnouncementList(string file, int posIdValue, int posDriverText)
        {
            this.posDriverText = posDriverText;
            this.posIdValue = posIdValue;

            this.entries = new List<string[]>();
            var csvReader = new CsvReader(PathManager.Instance.GetPath(FileType.Database, file));
            string[] entry;
            while ((entry = csvReader.GetCsvLine()) != null)
            {
                this.entries.Add(entry);
            }
        }

        /// <summary>
        /// Gets the driver text for the given id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The driver text.
        /// </returns>
        public string GetDriverText(string id)
        {
            foreach (var str in this.entries)
            {
                if (str[this.posIdValue].Equals(id))
                {
                    return str[this.posDriverText];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the value for the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The announcement text.
        /// </returns>
        public string GetIdValue(int index)
        {
            return this.entries[index][this.posIdValue];
        }

        /// <summary>
        /// Gets the id value for the given driver text.
        /// </summary>
        /// <param name="driverText">
        /// The driver text.
        /// </param>
        /// <returns>
        /// The announcement text.
        /// </returns>
        public string GetIdValue(string driverText)
        {
            foreach (var str in this.entries)
            {
                if (str[this.posDriverText].Equals(driverText))
                {
                    return str[this.posIdValue];
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the list of all announcements.
        /// </summary>
        /// <returns>
        /// A new list containing all announcement driver texts.
        /// </returns>
        public List<string> GetAllAnnouncements()
        {
            var tmp = new List<string>();
            foreach (var str in this.entries)
            {
                tmp.Add(str[this.posDriverText]);
            }

            return tmp;
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <returns>
        /// The length.
        /// </returns>
        public int GetLength()
        {
            return this.entries.Count;
        }
    }
}