// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileCompression.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileCompression type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FileCompressionTest
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using ICSharpCode.SharpZipLib.Zip;

    using NLog;

    /// <summary>
    /// The file compression.
    /// </summary>
    public class FileCompression
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCompression"/> class.
        /// </summary>
        public FileCompression()
        {
            this.ZipCompression();
            this.GzCompression();
        }

        private void GzCompression()
        {
            var dir = new DirectoryInfo("D:\\develop\\Main\\Motion\\Protran\\Spikes\\FileCompressionTest\\FileCompressionTest\\FilesForCompression");
            var files = dir.GetFiles();
            var outFile = File.Create("D:\\develop\\Main\\Motion\\Protran\\Spikes\\FileCompressionTest\\FileCompressionTest\\Outputfile");

            try
            {
                var compress = new GZipStream(outFile, CompressionMode.Compress, false);

                foreach (var file in files)
                {
                    var infile = File.OpenRead(file.FullName);
                    this.buffer = new byte[infile.Length];

                    var count = infile.Read(this.buffer, 0, this.buffer.Length);
                    infile.Close();

                    if (count <= 0)
                    {
                        continue;
                    }

                    compress.Write(this.buffer, 0, count);
                    Console.WriteLine(
                        "Original size: {0}, Compressed size: {1}", this.buffer.Length, outFile.Length);
                }

                compress.Close();
            }
            catch (Exception e)
            {
                Logger.Error("Error in compressing file using Gz due to: {0}", e);
            }
        }

        private void ZipCompression()
        {
            var dir = new DirectoryInfo("D:\\develop\\Main\\Motion\\Protran\\Spikes\\FileCompressionTest\\FileCompressionTest\\FilesForCompression");

            var zip = new FastZip();
            zip.CreateZip("ZippedLogs.zip", dir.FullName, true, string.Empty);
        }
    }
}
