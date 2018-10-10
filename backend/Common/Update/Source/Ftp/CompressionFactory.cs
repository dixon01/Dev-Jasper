// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompressionFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Ftp
{
    using System;
    using System.IO;

    using Gorba.Common.Configuration.Update.Common;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core.IO;

    using ICSharpCode.SharpZipLib.GZip;

    /// <summary>
    /// Factory for creating streams that compress and decompress data.
    /// </summary>
    public class CompressionFactory
    {
        private readonly string temporaryDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionFactory"/> class.
        /// </summary>
        /// <param name="temporaryDirectory">
        /// The directory where this factory can create temporary files.
        /// </param>
        public CompressionFactory(string temporaryDirectory)
        {
            this.temporaryDirectory = temporaryDirectory;
        }

        /// <summary>
        /// Creates a stream that compresses everything before writing to <see cref="output"/> using the
        /// <see cref="compression"/> algorithm.
        /// </summary>
        /// <param name="output">Stream to write to.</param>
        /// <param name="compression">Compression algorithm.</param>
        /// <returns>a <see cref="Stream"/>.</returns>
        public Stream CreateCompressionStream(Stream output, CompressionAlgorithm compression)
        {
            switch (compression)
            {
                case CompressionAlgorithm.None:
                    return output;
                case CompressionAlgorithm.GZIP:
                    return new GZipOutputStream(output);
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        /// <summary>
        /// Creates a stream that compresses everything read from <see cref="input"/> using the
        /// <see cref="compression"/> algorithm.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="compression">Compression algorithm.</param>
        /// <returns>a <see cref="Stream"/>.</returns>
        public Stream CreateCompressedStream(Stream input, CompressionAlgorithm compression)
        {
            switch (compression)
            {
                case CompressionAlgorithm.None:
                    return input;
                case CompressionAlgorithm.GZIP:
                    return this.CreateGzipCompressedStream(input);
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        /// <summary>
        /// Creates a stream that decompresses everything when reading from <see cref="input"/> using the
        /// <see cref="compression"/> algorithm.
        /// </summary>
        /// <param name="input">Stream to read from.</param>
        /// <param name="compression">Compression algorithm.</param>
        /// <returns>a <see cref="Stream"/>.</returns>
        public Stream CreateDecompressionStream(Stream input, CompressionAlgorithm compression)
        {
            switch (compression)
            {
                case CompressionAlgorithm.None:
                    return input;
                case CompressionAlgorithm.GZIP:
                    return new GZipInputStream(input);
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        /// <summary>
        /// Decompresses the given file with the given algorithm and
        /// deletes the original file.
        /// The original file name is returned (and the file of course not deleted)
        /// if the algorithm is <see cref="CompressionAlgorithm.None"/>
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="compression">
        /// The compression algorithm.
        /// </param>
        /// <returns>
        /// The path to a temporary file that has to be deleted by the callee.
        /// </returns>
        public string GetDecompressedFile(string fileName, CompressionAlgorithm compression)
        {
            switch (compression)
            {
                case CompressionAlgorithm.None:
                    return fileName;
                case CompressionAlgorithm.GZIP:
                    var tempFile = Path.Combine(
                        this.temporaryDirectory, Guid.NewGuid() + FileDefinitions.TempFileExtension);
                    try
                    {
                        using (var input = new GZipInputStream(File.OpenRead(fileName)))
                        {
                            using (var output = File.Create(tempFile))
                            {
                                StreamCopy.Copy(input, output);
                            }
                        }

                        File.Delete(fileName);
                        return tempFile;
                    }
                    catch
                    {
                        File.Delete(tempFile);
                        throw;
                    }

                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        private Stream CreateGzipCompressedStream(Stream input)
        {
            var tempFile = Path.Combine(this.temporaryDirectory, Guid.NewGuid() + FileDefinitions.TempFileExtension);
            try
            {
                using (input)
                {
                    using (var fileStream = File.Create(tempFile))
                    {
                        using (var tempOutput = new GZipOutputStream(fileStream))
                        {
                            StreamCopy.Copy(input, tempOutput);
                        }
                    }
                }

                return new TemporaryFileStream(tempFile);
            }
            catch
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (IOException)
                {
                }

                throw;
            }
        }
    }
}