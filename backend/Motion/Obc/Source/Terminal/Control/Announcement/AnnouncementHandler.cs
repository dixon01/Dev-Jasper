// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnouncementHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Announcement
{
    using System;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The announcement handler.
    /// </summary>
    internal class AnnouncementHandler
    {
        private readonly AnnouncementType announcementType = AnnouncementType.None;
        private AnnouncementList announcementList;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementHandler"/> class.
        /// </summary>
        /// <param name="announcementType">
        /// The announcement type.
        /// </param>
        public AnnouncementHandler(AnnouncementType announcementType)
        {
            this.announcementType = announcementType;
            this.LoadAnnouncementList();

            LanguageManager.Instance.CurrentLanguageChanged += this.CurrentLanguageChanged;
        }

        /// <summary>
        /// Gets the announcement list.
        /// </summary>
        /// <returns>
        /// The <see cref="AnnouncementList"/>.
        /// </returns>
        public AnnouncementList GetAnnouncementList()
        {
            return this.announcementList;
        }

        private void CurrentLanguageChanged(object sender, EventArgs e)
        {
            this.LoadAnnouncementList();
        }

        private void LoadAnnouncementList()
        {
            var lang = LanguageManager.Instance.CurrentLanguage;
            switch (this.announcementType)
            {
                case AnnouncementType.Mp3:
                    this.announcementList = new AnnouncementList(
                        ConfigPaths.AnnouncementCsv, 0, lang.Number);
                    break;
                case AnnouncementType.Tts:
                    string tmpPath = string.Format(ConfigPaths.AnnouncementCsvLangFormat, lang.Name);
                    this.announcementList = new AnnouncementList(tmpPath, 1, 0);
                    break;
                default:
                    throw new NotSupportedException("Announcement value is not supported: " + this.announcementType);
            }
        }
    }
}