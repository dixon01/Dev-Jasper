// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PasteConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The paste configuration.
    /// </summary>
    public class PasteConfiguration
    {
        /// <summary>
        /// The default.
        /// </summary>
        public static readonly PasteConfiguration Default = new PasteConfiguration(PhysicalScreenType.Unknown);

        /// <summary>
        /// The TFT configuration.
        /// </summary>
        public static readonly PasteConfiguration Tft = new PasteConfiguration(
            PhysicalScreenType.TFT,
            DefaultOffsetTft);

        /// <summary>
        /// The configuration for LED screens.
        /// </summary>
        public static readonly PasteConfiguration Led = new PasteConfiguration(
            PhysicalScreenType.LED,
            DefaultOffsetLed);

        /// <summary>
        /// The configuration for Audio screens.
        /// </summary>
        public static readonly PasteConfiguration Audio = new PasteConfiguration(PhysicalScreenType.Audio);

        private const int DefaultOffsetTft = 10;

        private const int DefaultOffsetLed = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasteConfiguration"/> class.
        /// </summary>
        /// <param name="screenType">
        /// The screen type.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        public PasteConfiguration(PhysicalScreenType screenType, int offset = 0)
        {
            this.Offset = offset;
            this.ScreenType = screenType;
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Gets the screen type.
        /// </summary>
        public PhysicalScreenType ScreenType { get; private set; }
    }
}
