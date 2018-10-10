// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dictionary.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Generic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// The container of the defined tables and columns for the generic 3d (table, column, row) approach.
    /// </summary>
    [Serializable]
    public class Dictionary
    {
        private static readonly Version CurrentVersion = new Version(2, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        public Dictionary()
        {
            this.Version = CurrentVersion;
            this.Languages = new List<Language>();
            this.Tables = new List<Table>();
        }

        /// <summary>
        /// Gets the XSD schema for this config structure.
        /// </summary>
        public static XmlSchema Schema
        {
            get
            {
                using (
                    var input = typeof(Dictionary).Assembly.GetManifestResourceStream(
                        typeof(Dictionary),
                        "dictionary.xsd"))
                {
                    if (input == null)
                    {
                        throw new FileNotFoundException("Couldn't find dictionary.xsd resource");
                    }

                    return XmlSchema.Read(input, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the version of the dictionary.
        /// </summary>
        [XmlIgnore]
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the version of the dictionary as an XML serializable string.
        /// </summary>
        [XmlAttribute("Version")]
        public string VersionString
        {
            get
            {
                return this.Version.ToString();
            }

            set
            {
                this.Version = value == null ? CurrentVersion : new Version(value);
            }
        }

        /// <summary>
        /// Gets or sets the list of all <see cref="Table"/>s of the dictionary.
        /// </summary>
        public List<Language> Languages { get; set; }

        /// <summary>
        /// Gets or sets the list of all <see cref="Table"/>s of the dictionary.
        /// </summary>
        public List<Table> Tables { get; set; }

        /// <summary>
        /// Gets a generic view language from this dictionary for a given language name or number.
        /// This checks first if a language exists with the given string in the name
        /// and then searches for a language with the given number (assuming languageName
        /// is a number).
        /// </summary>
        /// <param name="languageName">
        /// The language name. Can be part of the complete name.
        /// </param>
        /// <returns>
        /// The language. Returns null if the language can't be found.
        /// </returns>
        public Language GetLanguageForNameOrNumber(string languageName)
        {
            return GetForNameOrNumber(this.Languages, languageName);
        }

        /// <summary>
        /// Gets a table from the dictionary for a given table name or number.
        /// This checks first if a table exists with the given string in the name
        /// and then searches for a table with the given number (assuming tableName
        /// is a number).
        /// </summary>
        /// <param name="tableName">
        /// The table name. Can be part of the complete name.
        /// </param>
        /// <returns>
        /// The table. Returns null if the table can't be found.
        /// </returns>
        public Table GetTableForNameOrNumber(string tableName)
        {
            return GetForNameOrNumber(this.Tables, tableName);
        }

        /// <summary>
        /// Gets an item from a list according to its name or index.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="nameOrNumber">
        /// The name or number to search for.
        /// </param>
        /// <typeparam name="T">
        /// The type of item to search for.
        /// </typeparam>
        /// <returns>
        /// The found item or null if none was found.
        /// </returns>
        internal static T GetForNameOrNumber<T>(List<T> items, string nameOrNumber)
            where T : class, IDictionaryItem
        {
            var column = items.Find(i => i.Name.Equals(nameOrNumber, StringComparison.InvariantCultureIgnoreCase));
            if (column != null)
            {
                return column;
            }

            int index;
            if (!ParserUtil.TryParse(nameOrNumber, out index))
            {
                return null;
            }

            return items.Find(i => i.Index == index);
        }
    }
}
