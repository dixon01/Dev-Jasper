// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenshotEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Event arguments for taking a screenshot.
    /// </summary>
    public class ScreenshotEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the encoder.
        /// </summary>
        public BitmapEncoder Encoder { get; set; }
    }
}
