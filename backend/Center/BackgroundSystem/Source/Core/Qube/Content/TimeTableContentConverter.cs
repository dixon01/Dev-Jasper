// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeTableContentConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeTableContentConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Content
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading.Tasks;

    using Accord.Imaging.Converters;
    using Accord.MachineLearning;
    using Accord.Math;

    using ImageMagick;

    /// <summary>
    /// Defines an image conversion for a time table.
    /// </summary>
    public class TimeTableContentConverter : ImageContentConverter
    {
        private TimeTableContentConverter(EPaperFormat paperFormat)
        {
            this.PaperFormat = paperFormat;
        }

        /// <summary>
        /// Defines the possible rotations.
        /// </summary>
        public enum Rotation
        {
            /// <summary>
            /// The left rotation.
            /// </summary>
            Left = 0,

            /// <summary>
            /// The right rotation.
            /// </summary>
            Right = 1
        }

        /// <summary>
        /// Gets the paper format.
        /// </summary>
        public EPaperFormat PaperFormat { get; private set; }

        /// <summary>
        /// Creates the image conversion object for the specified format.
        /// </summary>
        /// <param name="paperFormat">
        ///     The paper format.
        /// </param>
        /// <returns>
        /// The <see cref="ImageContentConverter"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The specified format is not supported.
        /// </exception>
        public static ImageContentConverter Create(EPaperFormat paperFormat)
        {
            if (paperFormat != EPaperFormat.TimeTable)
            {
                throw new NotSupportedException("Only TimeTable is supported at the moment");
            }

            return new TimeTableContentConverter(paperFormat);
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
        public override async Task<TableConversionResult> ProcessAsync(Stream stream)
        {
            var image = new MagickImage(stream)
                          {
                              ColorSpace = ColorSpace.sRGB,
                              CompressionMethod = CompressionMethod.NoCompression,
                              Density = new PointD(175),
                              Format = MagickFormat.Png,
                              Quality = 100
                          };
            image.Alpha(AlphaOption.Off);
            var bitmap = image.ToBitmap();
            var top = new MagickImage((Bitmap)bitmap.Clone())
                          {
                              ColorSpace = ColorSpace.Gray,
                              CompressionMethod = CompressionMethod.NoCompression,
                              Format = MagickFormat.Png,
                              Quality = 100
                          };
            top.Crop(image.Width, image.Height / 2, Gravity.North);
            var topPart = await this.ProcessAsync(top, "Top", Rotation.Left);
            var bottomBitmap = (Bitmap)bitmap.Clone();
            var bottomImage = new MagickImage(bottomBitmap)
                                  {
                                      ColorSpace = ColorSpace.Gray,
                                      CompressionMethod = CompressionMethod.NoCompression,
                                      Format = MagickFormat.Png,
                                      Quality = 100
                                  };
            bottomImage.Crop(bottomImage.Width, bottomImage.Height / 2, Gravity.South);
            var bottomPart = await this.ProcessAsync(bottomImage, "Bottom", Rotation.Right);
            var output = await this.ProcessOutputAsync(topPart, bottomPart);
            var input = await this.SaveInternalAsync(bitmap, "Input", "TimeTableInput.png");
            return new TableConversionResult(input.Resource, output, topPart, bottomPart);
        }

        /// <summary>
        /// Gets the format for image conversion.
        /// </summary>
        /// <returns>
        /// The <see cref="ImageContentConversionFormat"/>.
        /// </returns>
        protected override ImageContentConversionFormat GetFormat()
        {
            return ImageContentConversionFormat.TimeTable;
        }

        /// <summary>
        /// Runs the k-means algorithm on the given image.
        /// </summary>
        /// <param name="image">
        /// The image to process.
        /// </param>
        /// <param name="k">
        /// The parameter required by the algorithm.
        /// </param>
        /// <returns>
        /// The <see cref="Bitmap"/> after processing.
        /// </returns>
        /// <remarks>The algorithm is not currently used.</remarks>
        private Bitmap RunKMeans(Bitmap image, int k)
        {
            // Retrieve the number of clusters

            // Load original image

            // Create converters
            var imageToArray = new ImageToArray(min: -1, max: +1);
            var arrayToImage = new ArrayToImage(image.Width, image.Height, min: -1, max: +1);

            var input = image;

            // Transform the image into an array of pixel values
            double[][] pixels;
            imageToArray.Convert(input, out pixels);

            // Create a K-Means algorithm using given k and a
            //  square Euclidean distance as distance metric.
            var kmeans = new KMeans(k, Distance.SquareEuclidean)
                             {
                                 Tolerance = 0.05
                             };

            // Compute the K-Means algorithm until the difference in
            //  cluster centroids between two iterations is below 0.05
            var idx = kmeans.Compute(pixels);

            // Replace every pixel with its corresponding centroid
            pixels.ApplyInPlace((x, i) => kmeans.Clusters.Centroids[idx[i]]);

            // Show resulting image in the picture box
            Bitmap result;
            arrayToImage.Convert(pixels, out result);
            return result;
        }

        private async Task<TableConversionPart> ProcessOutputAsync(
            TableConversionPart topPart,
            TableConversionPart bottomPart)
        {
            var outputStream = new MemoryStream();
            await topPart.Epd.Content.CopyToAsync(outputStream);
            topPart.Epd.Content.Position = 0;
            await bottomPart.Epd.Content.CopyToAsync(outputStream);
            bottomPart.Epd.Content.Position = 0;

            outputStream.Position = 0;
            var contentResource = await this.StoreAsync(outputStream, "output.epd");
            outputStream.Position = 0;
            return new TableConversionPart { Epd = new StreamContentResourceWrapper(outputStream, contentResource) };
        }

        private async Task<TableConversionPart> ProcessAsync(MagickImage top, string name, Rotation rotation)
        {
            double rotationValue;
            switch (rotation)
            {
                case Rotation.Left:
                    rotationValue = -90;
                    break;
                case Rotation.Right:
                    rotationValue = 90;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("rotation", rotation, null);
            }

            top.Rotate(rotationValue);
            var part = new TableConversionPart();
            var bitmap = top.ToBitmap();
            var input =
                await
                this.SaveInternalAsync(bitmap, name, string.Format("TimeTable{0}Input.Png", name));
            var size = new MagickGeometry(1024, 1280);
            top.Scale(size);
            top.Extent(size, Gravity.Southeast, new MagickColor(Color.White));
            bitmap = top.ToBitmap();
            this.ToBlackAndWhiteInternal(bitmap);
            part.BlackWhite = new StreamContentResourceWrapper(this.GetStream(bitmap));
            var paperFile = await this.CreatePaperFileInternalAsync(bitmap, string.Format("{0}Epd", name));
            part.Epd = paperFile;
            return part;
        }
    }
}