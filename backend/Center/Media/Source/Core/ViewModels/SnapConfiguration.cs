// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SnapConfiguration.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SnapConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    /// <summary>
    /// Defines the configuration for the snapping functionality.
    /// </summary>
    public class SnapConfiguration
    {
        /// <summary>
        /// The TFT configuration.
        /// </summary>
        public static readonly SnapConfiguration Tft = new SnapConfiguration(true, TftSnapTolerance);

        /// <summary>
        /// The led editor snap configuration
        /// </summary>
        public static readonly SnapConfiguration Led = new SnapConfiguration(false, LedSnapTolerance);

        /// <summary>
        /// The configuration to disable the feature.
        /// </summary>
        public static readonly SnapConfiguration Disabled = new SnapConfiguration(false);

        private const int LedSnapTolerance = 1;

        private const int TftSnapTolerance = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnapConfiguration"/> class.
        /// </summary>
        /// <param name="isAvailable">
        /// The is available.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance.
        /// </param>
        public SnapConfiguration(bool isAvailable, double tolerance = 0)
        {
            this.IsAvailable = isAvailable;
            this.Tolerance = tolerance;
        }

        /// <summary>
        /// Gets a value indicating whether snapping is available.
        /// </summary>
        public bool IsAvailable { get; private set; }

        /// <summary>
        /// Gets the tolerance value.
        /// </summary>
        public double Tolerance { get; private set; }
    }
}