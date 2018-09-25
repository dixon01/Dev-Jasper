// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnouncementScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnnouncementScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.Announcement;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The announcement screen.
    /// </summary>
    internal class AnnouncementScreen : SimpleListScreen
    {
        private readonly AnnouncementHandler announcementHandler;

        private readonly AnnouncementType announcementType = AnnouncementType.None;

        private AnnouncementList announcementList;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncementScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="announcementHandler">
        /// The announcement handler.
        /// </param>
        public AnnouncementScreen(IList mainField, IContext context, AnnouncementHandler announcementHandler)
            : base(mainField, context)
        {
            this.announcementType = context.ConfigHandler.GetConfig().Announcement.Value;
            this.announcementHandler = announcementHandler;
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                this.announcementList = this.announcementHandler.GetAnnouncementList();
                return this.announcementList.GetAllAnnouncements();
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(1, "Announcements");
            }
        }

        /// <summary>
        ///   This method will be called when the user has selected an entry.
        ///   Implement your action here. The index is the selected item from the GetList() method
        /// </summary>
        /// <param name = "index">
        /// The selected index.
        /// </param>
        protected override void ItemSelected(int index)
        {
            string idToSend = this.announcementList.GetIdValue(index);
            switch (this.announcementType)
            {
                case AnnouncementType.Mp3:
                    MessageDispatcher.Instance.Broadcast(new evAnnouncement(int.Parse(idToSend)));
                    break;

                case AnnouncementType.Tts:
                    MessageDispatcher.Instance.Broadcast(
                        new evTTSFrame(0, evTTSFrame.OutDest.SpeakerAndDisplay, 60, idToSend, string.Empty, 0, 0));
                    break;
            }

            this.Context.ShowRootScreen();
        }
    }
}