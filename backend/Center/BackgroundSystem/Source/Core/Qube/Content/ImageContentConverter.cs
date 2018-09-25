// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageContentConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageContentConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;

    /// <summary>
    /// Defines a component used for image conversions.
    /// </summary>
    public abstract class ImageContentConverter
    {
        private const string WhiteString = "ffffffff";

        private const int BitArrayLength = 8;

        /// <summary>
        /// Create an e-paper file.
        /// </summary>
        /// <param name="image">
        /// The image to convert.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <returns>
        /// The <see cref="StreamContentResourceWrapper"/> relative to the input image.
        /// </returns>
        public async Task<StreamContentResourceWrapper> CreatePaperFileInternalAsync(Bitmap image, string resourceName)
        {
            var stream = new MemoryStream();
            var buffer = new List<bool>(8);
            for (var i = 0; i < 1280; i++)
            {
                for (var j = 0; j < 1024; j++)
                {
                    var name = j < image.Width && i < image.Height ? image.GetPixel(j, i).Name : WhiteString;
                    buffer.Add(!string.Equals(name, WhiteString, StringComparison.InvariantCultureIgnoreCase));
                    if (buffer.Count == BitArrayLength)
                    {
                        this.Write(stream, buffer);
                        buffer.Clear();
                    }
                }
            }

            this.Write(stream, buffer);
            stream.Position = 0;
            var contentResource = await this.StoreAsync(stream, "epd");
            stream.Position = 0;
            return new StreamContentResourceWrapper(stream, contentResource);
        }

        /// <summary>
        /// Processes the given stream and converts it to a format valid for the e-paper.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="TableConversionResult"/>.
        /// </returns>
        public abstract Task<TableConversionResult> ProcessAsync(Stream stream);

        /// <summary>
        /// Converts the given image to black and white.
        /// </summary>
        /// <param name="img">
        /// The image to convert.
        /// </param>
        protected virtual void ToBlackAndWhiteInternal(Bitmap img)
        {
            using (var gr = Graphics.FromImage(img))
            {
                var matrix = new[]
                                 {
                                     new[] { 0.299f, 0.299f, 0.299f, 0, 0 }, new[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                                     new[] { 0.114f, 0.114f, 0.114f, 0, 0 }, new[] { 0f, 0, 0, 1, 0 },
                                     new[] { 0f, 0, 0, 0, 1 }
                                 };

                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(new ColorMatrix(matrix));
                attributes.SetThreshold(0.8f); // Change this threshold as needed
                var rc = new Rectangle(0, 0, img.Width, img.Height);
                gr.DrawImage(img, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
            }
        }

        /// <summary>
        /// Stores the given stream.
        /// </summary>
        /// <param name="stream">
        /// The stream to store.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        /// <returns>
        /// The created <see cref="ContentResource"/>.
        /// </returns>
        protected async Task<ContentResource> StoreAsync(Stream stream, string originalFileName)
        {
            var resourceService = DependencyResolver.Current.Get<IContentResourceService>();
            var hash = ContentResourceHash.Create(stream, HashAlgorithmTypes.xxHash64);
            stream.Position = 0;
            var contentResourceRequest = new ContentResourceUploadRequest
                                             {
                                                 Content = stream,
                                                 Resource =
                                                     new ContentResource
                                                         {
                                                             Hash = hash,
                                                             HashAlgorithmType
                                                                 =
                                                                 HashAlgorithmTypes
                                                                 .xxHash64,
                                                             MimeType =
                                                                 "application/octet-stream",
                                                             OriginalFilename =
                                                                 originalFileName
                                                         }
                                             };
            var result = await resourceService.UploadAsync(contentResourceRequest);
            if (result == null)
            {
                return await resourceService.GetAsync(hash, HashAlgorithmTypes.xxHash64);
            }

            return result.Resource;
        }

        /// <summary>
        /// Gets a stream out of given image.
        /// The stream is seekable and set to Position 0.
        /// </summary>
        /// <param name="image">
        /// The image.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        protected Stream GetStream(Bitmap image)
        {
            var outputStream = new MemoryStream();
            image.Save(outputStream, ImageFormat.Png);
            outputStream.Position = 0;

            return outputStream;
        }

        /// <summary>
        /// Gets the format for image conversion.
        /// </summary>
        /// <returns>
        /// The <see cref="ImageContentConversionFormat"/>.
        /// </returns>
        protected abstract ImageContentConversionFormat GetFormat();

        /// <summary>
        /// Saves the image internally.
        /// </summary>
        /// <param name="img">The image to save.</param>
        /// <param name="name">The name of the image.</param>
        /// <param name="originalFileName">The original file name.</param>
        /// <returns>The created <see cref="StreamContentResourceWrapper"/>.</returns>
        protected async Task<StreamContentResourceWrapper> SaveInternalAsync(
            Bitmap img,
            string name,
            string originalFileName)
        {
            var outputStream = new MemoryStream();
            img.Save(outputStream, ImageFormat.Png);
            outputStream.Position = 0;
            var contentResource = await this.StoreAsync(outputStream, originalFileName);
            outputStream.Position = 0;
            return new StreamContentResourceWrapper(outputStream, contentResource);
        }

        private void Write(Stream stream, IList<bool> buffer)
        {
            if (!buffer.Any())
            {
                return;
            }

            var b = new BitArray(buffer.Reverse().ToArray());
            var bytes = new byte[b.Length];
            b.CopyTo(bytes, 0);
            stream.WriteByte(bytes[0]);
        }
    }
}