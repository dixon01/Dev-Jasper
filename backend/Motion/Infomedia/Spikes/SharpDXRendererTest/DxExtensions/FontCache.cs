// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FontCache.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FontCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions
{
    using System.Collections.Generic;

    using SharpDX.Direct3D9;

    /// <summary>
    /// Cache for <see cref="FontEx"/> objects.
    /// </summary>
    public class FontCache
    {
        private readonly Dictionary<FontKey, FontEx> fonts = new Dictionary<FontKey, FontEx>();

        static FontCache()
        {
            Instance = new FontCache();
        }

        private FontCache()
        {
        }

        /// <summary>
        /// Gets the single instance of this cache.
        /// </summary>
        public static FontCache Instance { get; private set; }

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="weight">
        /// The weight.
        /// </param>
        /// <param name="mipLevels">
        /// The number of MIP levels.
        /// </param>
        /// <param name="italic">
        /// The italic.
        /// </param>
        /// <param name="charSet">
        /// The char set.
        /// </param>
        /// <param name="outputPrecision">
        /// The output precision.
        /// </param>
        /// <param name="quality">
        /// The quality.
        /// </param>
        /// <param name="pitchAndFamily">
        /// The pitch and family.
        /// </param>
        /// <param name="fontName">
        /// The font name.
        /// </param>
        /// <returns>
        /// The <see cref="FontEx"/>.
        /// </returns>
        public FontEx CreateFont(
            Device device,
            int height,
            int width,
            FontWeight weight,
            int mipLevels,
            bool italic,
            FontCharacterSet charSet,
            FontPrecision outputPrecision,
            FontQuality quality,
            FontPitchAndFamily pitchAndFamily,
            string fontName)
        {
            // Create the font description
            var desc = new FontDescription
            {
                Height = height,
                Width = width,
                Weight = weight,
                MipLevels = mipLevels,
                Italic = italic,
                CharacterSet = charSet,
                OutputPrecision = outputPrecision,
                Quality = quality,
                PitchAndFamily = pitchAndFamily,
                FaceName = fontName
            };

            // return the font
            return this.CreateFont(device, desc);
        }

        /// <summary>
        /// Creates a new font or takes one from the cache.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="FontEx"/>.
        /// </returns>
        public FontEx CreateFont(Device device, FontDescription description)
        {
            FontEx font;
            var key = new FontKey(device, description);
            if (!this.fonts.TryGetValue(key, out font))
            {
                font = new FontEx(device, description);
                font.Disposing += (s, e) => this.fonts.Remove(key);
                this.fonts.Add(key, font);
            }

            return font;
        }

        private sealed class FontKey
        {
            private readonly Device device;

            private readonly FontDescription description;

            public FontKey(Device device, FontDescription description)
            {
                this.device = device;
                this.description = description;
            }

            public override bool Equals(object obj)
            {
                var other = obj as FontKey;
                if (other == null)
                {
                    return false;
                }

                return this.device == other.device 
                    && this.description.Height == other.description.Height
                    && this.description.Width == other.description.Width
                    && this.description.MipLevels == other.description.MipLevels
                    && this.description.Italic == other.description.Italic
                    && this.description.CharacterSet == other.description.CharacterSet
                    && this.description.OutputPrecision == other.description.OutputPrecision
                    && this.description.Quality == other.description.Quality
                    && this.description.PitchAndFamily == other.description.PitchAndFamily
                    && this.description.FaceName == other.description.FaceName;
            }

            public override int GetHashCode()
            {
                return this.device.GetHashCode() ^ this.description.GetHashCode();
            }
        }
    }
}
