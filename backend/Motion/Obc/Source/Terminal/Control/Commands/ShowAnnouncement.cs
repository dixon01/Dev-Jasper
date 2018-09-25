// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowAnnouncement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Shows the announcement screen
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    ///   Shows the announcement screen
    /// </summary>
    internal class ShowAnnouncement : MainFieldCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowAnnouncement"/> class.
        /// </summary>
        public ShowAnnouncement()
            : base(MainFieldKey.Announcement)
        {
        }
    }
}
