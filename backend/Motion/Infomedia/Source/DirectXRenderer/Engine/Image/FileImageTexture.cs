// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileImageTexture.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileImageTexture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Image
{
    using System;
    using System.Drawing;

    using Gorba.Common.Utility.Core;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    /// <summary>
    /// Image texture that loads its data from a file and
    /// also verifies regularly whether the file has been updated.
    /// </summary>
    public class FileImageTexture : ImageTextureBase
    {
        // interval in milliseconds
        private const int CheckFileInterval = 2000;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string filename;

        private readonly FileCheck fileCheck;

        private long lastCheck;

        private bool shouldReload;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileImageTexture"/> class.
        /// </summary>
        /// <param name="filename">
        /// The image file name.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        public FileImageTexture(string filename, Device device)
            : this(new Bitmap(filename), filename, device)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileImageTexture"/> class.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap created from the given file which will be taken over by this class and disposed.
        /// </param>
        /// <param name="filename">
        /// The image file name.
        /// </param>
        /// <param name="device">
        /// The device.
        /// </param>
        internal FileImageTexture(Bitmap bitmap, string filename, Device device)
            : base(device)
        {
            using (bitmap)
            {
                this.Initialize(bitmap);
            }

            this.filename = filename;
            this.fileCheck = new FileCheck(filename);
            this.lastCheck = TimeProvider.Current.TickCount;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.filename;
        }

        /// <summary>
        /// Draws this image to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public override void DrawTo(
            Sprite sprite, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color)
        {
            this.CheckFileUpdated();
            base.DrawTo(sprite, srcRectangle, destinationSize, position, color);
        }

        /// <summary>
        /// Draws this image with the given rotation to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="rotationCenter">
        /// The rotation center.
        /// </param>
        /// <param name="rotationAngle">
        /// The rotation angle.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public override void DrawTo(
            Sprite sprite,
            Rectangle srcRectangle,
            SizeF destinationSize,
            PointF rotationCenter,
            float rotationAngle,
            PointF position,
            Color color)
        {
            this.CheckFileUpdated();
            base.DrawTo(sprite, srcRectangle, destinationSize, rotationCenter, rotationAngle, position, color);
        }

        private void CheckFileUpdated()
        {
            var now = TimeProvider.Current.TickCount;
            if (this.lastCheck + CheckFileInterval >= now)
            {
                return;
            }

            this.lastCheck = now;
            if (!this.shouldReload && !this.fileCheck.CheckChanged())
            {
                return;
            }

            if (!this.fileCheck.Exists)
            {
                return;
            }

            Logger.Debug("Reloading {0}", this.filename);
            Bitmap bitmap;
            try
            {
                bitmap = new Bitmap(this.filename);
                this.shouldReload = false;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't reload File " + this.filename);
                this.shouldReload = true;
                return;
            }

            using (bitmap)
            {
                this.Initialize(bitmap);
            }
        }
    }
}