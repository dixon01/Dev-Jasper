// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageComponent.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageComponent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The image component.
    /// </summary>
    public class ImageComponent : ComponentBase
    {
        private IBitmap bitmap;

        private FileCheck fileCheck;

        /// <summary>
        /// Gets or sets the filename of the image.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets the <see cref="IBitmap"/> for this image.
        /// This method caches the generated bitmap for faster access.
        /// </summary>
        /// <returns>
        /// The <see cref="IBitmap"/>.
        /// </returns>
        public virtual IBitmap GetBitmap()
        {
            if (this.fileCheck == null)
            {
                this.fileCheck = new FileCheck(this.Filename);
            }

            if ((!this.fileCheck.CheckChanged() || !this.fileCheck.Exists) && this.bitmap != null)
            {
                return this.bitmap;
            }

            this.bitmap = BitmapFactory.CreateBitmap(this.Filename);
            return this.bitmap;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.
        /// </param>
        public override bool Equals(object obj)
        {
            var other = obj as ImageComponent;
            if (other == null)
            {
                return false;
            }

            return other.Filename == this.Filename && base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Filename.GetHashCode() ^ base.GetHashCode();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            var disposable = this.bitmap as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}