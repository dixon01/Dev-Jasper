// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneBook.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhoneBook type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Communication
{
    using System.Collections.Generic;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The phone book.
    /// </summary>
    public class PhoneBook
    {
        private static readonly object Locker = new object();

        /// <summary>
        ///   Static because if the phone book is read once, save some resources. otherwise
        ///   when new instance is loaded the xml file has to be loaded and parsed again...
        /// </summary>
        private static List<string[]> entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneBook"/> class.
        /// </summary>
        public PhoneBook()
        {
            lock (this)
            {
                if (entries != null)
                {
                    return;
                }

                entries = new List<string[]>();
                using (
                    var csvReader =
                        new CsvReader(PathManager.Instance.GetPath(FileType.Database, ConfigPaths.PhoneBookCsv)))
                {
                    string[] entry;
                    while ((entry = csvReader.GetCsvLine()) != null)
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return entries.Count;
            }
        }

        /// <summary>
        ///   If no number is found to this name, NULL will be returned
        /// </summary>
        /// <param name = "name">The name</param>
        /// <returns>Null if no entry is found</returns>
        public string GetNumber(string name)
        {
            int langIndex = LanguageManager.Instance.CurrentLanguage.Number;
            lock (Locker)
            {
                foreach (var entry in entries)
                {
                    if (entry.Length > langIndex)
                    {
                        // avoid index out of range ex
                        if (entry[langIndex].Equals(name))
                        {
                            return entry[0];
                        }
                    }
                    else
                    {
                        if (entry[1].Equals(name))
                        {
                            return entry[0];
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the number at the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The number.
        /// </returns>
        public string GetNumber(int index)
        {
            return entries[index][0];
        }

        /// <summary>
        /// Gets the name for the given number.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The name or null if not found.
        /// </returns>
        public string GetName(string number)
        {
            int langIndex = LanguageManager.Instance.CurrentLanguage.Number;
            foreach (var entry in entries)
            {
                if (!entry[0].Equals(number))
                {
                    continue;
                }

                return entry.Length <= langIndex ? entry[1] : entry[langIndex];
            }

            return null;
        }

        /// <summary>
        /// Gets all names from the phone book.
        /// </summary>
        /// <returns>
        /// The list of names.
        /// </returns>
        public List<string> GetAllNames()
        {
            int langIndex = LanguageManager.Instance.CurrentLanguage.Number;
            var names = new List<string>();
            foreach (var entry in entries)
            {
                names.Add(entry.Length > langIndex ? entry[langIndex] : entry[1]);
            }

            return names;
        }
    }
}