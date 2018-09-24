// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigPaths.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigPaths type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Config
{
    /// <summary>
    /// The config paths.
    /// </summary>
    internal static class ConfigPaths
    {
        /// <summary>
        ///   Main TerminalControl Config path
        /// </summary>
        public const string Config = @"Config.xml";

        /// <summary>
        ///   Path for the TerminalControl Short keys
        /// </summary>
        public const string ConfigShortKey = @"ConfigShortKey.xml";

        /// <summary>
        ///   Path for the TerminalControl menu configuration
        /// </summary>
        public const string ConfigMenu = @"ConfigMenu.xml";

        /// <summary>
        ///   Path for the TerminalControl status field configuration
        /// </summary>
        public const string StatusConfig = @"ConfigStatusField.xml";

        /// <summary>
        ///   Path for the temporary saved drive (block/special destination) status
        /// </summary>
        public const string SavedTmpStatus = @"SavedStatus.tmp.xml";

        /// <summary>
        ///   Path for the temporary saved in messages
        /// </summary>
        public const string SavedTmpMessages = @"SavedMessages.tmp.xml";

        /// <summary>
        /// Path for the alarm.csv
        /// </summary>
        public const string AlarmCsv = @"alarm.csv";

        /// <summary>
        /// Path for the announcement.csv.
        /// </summary>
        public const string AnnouncementCsv = @"announcement.csv";

        /// <summary>
        /// Path format for the announcement.csv language file.
        /// </summary>
        public const string AnnouncementCsvLangFormat = @"{0}.announcement.csv";

        /// <summary>
        /// Path for the extra service CSV file.
        /// </summary>
        public const string ExtraServiceCsv = @"extraservice.csv";

        /// <summary>
        ///   CSV File
        ///   Number;Text German;Text French;Text English
        /// </summary>
        public const string PhoneBookCsv = @"phoneBook.csv";
    }
}