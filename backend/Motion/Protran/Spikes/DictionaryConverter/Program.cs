// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DictionaryConverter
{
    using System.IO;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.Core.Dictionary;

    using Column = Gorba.Common.Protocols.Ximple.Generic.Column;
    using Table = Gorba.Common.Protocols.Ximple.Generic.Table;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var filename = args[0];
            var legacyMgr = new ConfigManager<GenViewDictionary> { FileName = args[0] };

            var newMgr = new ConfigManager<Dictionary> { FileName = Path.ChangeExtension(filename, ".new.xml") };
            newMgr.CreateConfig();

            ConvertDictionary(legacyMgr.Config, newMgr.Config);

            newMgr.SaveConfig();
        }

        private static void ConvertDictionary(GenViewDictionary legacy, Dictionary dictionary)
        {
            dictionary.Languages.Add(new Language { Index = 0, Name = "Default" });
            dictionary.Languages.Add(new Language { Index = 1, Name = "Secondary" });

            foreach (var table in legacy.Tables)
            {
                dictionary.Tables.Add(ConvertTable(table));
            }
        }

        private static Table ConvertTable(Gorba.Motion.Protran.Core.Dictionary.Table legacy)
        {
            var table = new Table
                            {
                                Name = legacy.Name,
                                Index = int.Parse(legacy.Number),
                                Description = legacy.Description
                            };
            foreach (var column in legacy.Columns)
            {
                table.Columns.Add(ConvertColumn(column));
            }

            return table;
        }

        private static Column ConvertColumn(Gorba.Motion.Protran.Core.Dictionary.Column legacy)
        {
            var column = new Column
                             {
                                 Name = legacy.Name,
                                 Index = int.Parse(legacy.Index),
                                 Description = legacy.Description
                             };
            return column;
        }
    }
}
