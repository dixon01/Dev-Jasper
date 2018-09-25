// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Parts
{
    using System;

    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// An image part of a text, used for layout and format handling.
    /// </summary>
    public class ImagePart : PartBase
    {
        private readonly string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePart"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename of the image.
        /// </param>
        public ImagePart(string filename)
            : this(filename, BitmapFactory.CreateBitmap(filename))
        {
        }

        private ImagePart(string filename, IBitmap bitmap)
        {
            this.filename = filename;
            this.Bitmap = bitmap;
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        public IBitmap Bitmap { get; private set; }

        /// <summary>
        /// Sets the scaling factor of this item.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public override void SetScaling(double factor)
        {
            if (factor < 1.0)
            {
                throw new ArgumentOutOfRangeException("factor", "Only factor of 1.0 allowed");
            }

            this.Width = this.Bitmap.Width;
            this.Height = this.Ascent = this.Bitmap.Height;
        }

        /// <summary>
        /// Tries to split the item into two parts at the given offset.
        /// The last possible split point in this item has to be found (meaning where the width of
        /// the returned <see cref="left"/> item is less than or equal to the given <see cref="offset"/>).
        /// If this item can't be split, the method must return false and <see cref="right"/> must be null.
        /// If the first possible split point is past <see cref="offset"/>, this method should split
        /// at that point and return true.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left item of the split operation. This is never null.
        /// If the item couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right item of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public override bool Split(int offset, out PartBase left, out PartBase right)
        {
            // images are not splittable
            left = this;
            right = null;
            return false;
        }

        /// <summary>
        /// Duplicates this part if necessary.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// The same or an equal <see cref="IPart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new ImagePart(this.filename, this.Bitmap);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public override bool Equals(PartBase other)
        {
            var image = other as ImagePart;
            return image != null && this.filename == image.filename;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            var disposable = this.Bitmap as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}