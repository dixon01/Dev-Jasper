// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogDirectoryTypes.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;

    /// <summary>
    /// The directory types used to set the initial directory when showing a file dialog.
    /// </summary>
    public static class DialogDirectoryTypes
    {
        /// <summary>
        /// The project directory.
        /// </summary>
        public static readonly DialogDirectoryType Project = new DialogDirectoryType("Project");

        /// <summary>
        /// The image directory.
        /// </summary>
        public static readonly DialogDirectoryType Image = new DialogDirectoryType("Image");

        /// <summary>
        /// The video directory.
        /// </summary>
        public static readonly DialogDirectoryType Video = new DialogDirectoryType("Video");

        /// <summary>
        /// The symbols directory.
        /// </summary>
        public static readonly DialogDirectoryType Symbol = new DialogDirectoryType("Symbol");

        /// <summary>
        /// The audio directory.
        /// </summary>
        public static readonly DialogDirectoryType Audio = new DialogDirectoryType("Audio");

        /// <summary>
        /// The fonts directory.
        /// </summary>
        public static readonly DialogDirectoryType Font = new DialogDirectoryType("Font");
    }
}
