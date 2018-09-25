// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Information about a font file.
    /// </summary>
    internal class FontInfo
    {
        private readonly FileCheck fileCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontInfo"/> class.
        /// </summary>
        /// <param name="filename">
        /// The font file name.
        /// </param>
        public FontInfo(string filename)
            : this(new FontFile(filename))
        {
            this.Filename = filename;
            this.fileCheck = new FileCheck(filename);
        }

        private FontInfo(FontFile font)
        {
            this.Font = font;
        }

        /// <summary>
        /// Gets the Alpha NT font file.
        /// </summary>
        public FontFile Font { get; private set; }

        /// <summary>
        /// Gets the name of the font.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Font.Name;
            }
        }

        /// <summary>
        /// Gets the file name of the font.
        /// This can be null if the font was obtained from <see cref="FromResource"/>.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Creates a <see cref="FontInfo"/> from the given resource name.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="FontInfo"/> or null if the resource was not found.
        /// </returns>
        public static FontInfo FromResource(string resourceName)
        {
            var stream = typeof(FontInfo).Assembly.GetManifestResourceStream(typeof(FontInfo), resourceName);
            return stream == null ? null : new FontInfo(new FontFile(stream, true));
        }

        /// <summary>
        /// Checks if the font file has changed since this object was created or the last
        /// time <see cref="CheckChanged"/> was called.
        /// </summary>
        /// <returns>
        /// True if the font file has changed and is available, otherwise false.
        /// </returns>
        public bool CheckChanged()
        {
            if (this.fileCheck == null || !this.fileCheck.CheckChanged() || !this.fileCheck.Exists)
            {
                return false;
            }

            this.Font = new FontFile(this.Filename);
            return true;
        }
    }
}