// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericAlarms.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericAlarms type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Alarm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Csv;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    /// The generic alarms.
    /// </summary>
    internal class GenericAlarms
    {
        /// <summary>
        /// Generic alarm but unknown type.
        /// </summary>
        public const int GenericAlarm = 15;

        /// <summary>
        /// The false alarm.
        /// </summary>
        public const int FalseAlarm = 14;

        /// <summary>
        /// The distress alarm.
        /// </summary>
        public const int DistressAlarm = 0;

        private static readonly Logger Logger = LogHelper.GetLogger<GenericAlarms>();

        private static readonly object Locker = new object();

        /// <summary>
        ///   Static because if the Alarms is read once, save some resources. otherwise
        ///   when new instance is loaded the xml file has to be loaded and parsed again...
        /// </summary>
        private static List<string[]> entries;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAlarms"/> class.
        /// </summary>
        public GenericAlarms()
        {
            lock (Locker)
            {
                if (entries != null)
                {
                    return;
                }

                entries = new List<string[]>();
                var path = PathManager.Instance.GetPath(FileType.Database, ConfigPaths.AlarmCsv);
                var csvReader = new CsvReader(path);
                {
                    try
                    {
                        string[] entry;
                        while ((entry = csvReader.GetCsvLine()) != null)
                        {
                            int alarmId = int.Parse(entry[0]);
                            if ((alarmId > 0) && (alarmId < 14))
                            {
                                entries.Add(entry);
                            }
                            else
                            {
                                throw new ArgumentException("Invalid AlarmID"); // MLHIDE
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorException("Invalid alarmID in File" + ConfigPaths.AlarmCsv, ex);
                        entries.Clear();
                    }
                }

                var falseAlarm = new string[4];

                // get all language text
                string[] strFalseAlarm = ml.GetMenuStrings(131, ml.ml_string(131, "False alarm"));
                falseAlarm[0] = FalseAlarm.ToString(CultureInfo.InvariantCulture);
                falseAlarm[1] = strFalseAlarm[0]; // German text
                falseAlarm[2] = strFalseAlarm[1]; // French text
                falseAlarm[3] = strFalseAlarm[2]; // English text
                entries.Add(falseAlarm);
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
        /// Gets the alarm id for the given index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The alarm ID or 0 if not found.
        /// </returns>
        public int GetAlarmId(int index)
        {
            if (entries.Count > index)
            {
                return int.Parse(entries[index][0]);
            }

            return 0;
        }

        /// <summary>
        ///   If no alarmID is found to this name, NULL will be returned
        /// </summary>
        /// <param name = "name">The name</param>
        /// <returns>0 if no entry is found</returns>
        public int GetAlarmId(string name)
        {
            int langIndex = LanguageManager.Instance.CurrentLanguage.Number;
            foreach (var entry in entries)
            {
                // avoid index out of range ex
                if (entry.Length > langIndex)
                {
                    if (entry[langIndex].Equals(name))
                    {
                        return int.Parse(entry[0]);
                    }
                }
                else
                {
                    if (entry[1].Equals(name))
                    {
                        return int.Parse(entry[0]);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// The get all alarm names.
        /// </summary>
        /// <returns>
        /// The list of all alarm names.
        /// </returns>
        public List<string> GetAllAlarmNames()
        {
            int langIndex = LanguageManager.Instance.CurrentLanguage.Number;
            var names = new List<string>();
            foreach (var entry in entries)
            {
                if (entry.Length > langIndex)
                {
                    names.Add(entry[langIndex]);
                }
                else
                {
                    names.Add(entry[1]);
                }
            }

            return names;
        }
    }
}