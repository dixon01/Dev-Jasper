// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFile.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Logging.NLogExtensions.Targets
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Timers;

    /// <summary>
    /// Class representing a log file to which messages can be written.
    /// This class also provides a configurable caching.
    /// </summary>
    internal class LogFile : IDisposable
    {
        /// <summary>
        /// Timestamp for <see cref="LastWriteTimeStamp"/> if the file was never written to.
        /// </summary>
        public static readonly DateTime InvalidTimeStamp = DateTime.MinValue;

        private readonly MemoryStream cache;

        private readonly Timer cacheTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFile"/> class.
        /// </summary>
        /// <param name="fullName">
        /// The full name of the log file, this must be an absolute path.
        /// </param>
        /// <param name="cacheSize">
        /// The cache size in bytes. If this is zero, caching is disabled.
        /// </param>
        /// <param name="maximumCacheTime">
        /// The maximum cache time after which the cache is flushed automatically.
        /// </param>
        public LogFile(string fullName, int cacheSize, TimeSpan maximumCacheTime)
        {
            this.FullName = fullName;
            var fileInfo = new FileInfo(fullName);
            this.LastWriteTimeStamp = fileInfo.Exists ? fileInfo.LastWriteTime : InvalidTimeStamp;

            if (cacheSize <= 0)
            {
                return;
            }

            this.cache = new MemoryStream(new byte[cacheSize], 0, cacheSize, true, true);
            this.cache.SetLength(0);

            if (maximumCacheTime <= TimeSpan.Zero)
            {
                return;
            }

            this.cacheTimeout = new Timer(maximumCacheTime.TotalMilliseconds);
            this.cacheTimeout.Elapsed += (s, e) => this.FlushCache();
            this.cacheTimeout.AutoReset = false;
            this.cacheTimeout.Enabled = true;
        }

        /// <summary>
        /// Gets the full file name, this is an absolute path.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// Gets or sets the last write time stamp.
        /// This property is never set by the class, but has to be updated by the handling class.
        /// </summary>
        public DateTime LastWriteTimeStamp { get; set; }

        /// <summary>
        /// Checks if this log file requires a header (i.e. doesn't contain anything yet).
        /// </summary>
        /// <returns>
        /// True if the file is empty (or doesn't exist), otherwise false.
        /// </returns>
        public bool RequiresHeader()
        {
            if (this.cache != null && this.cache.Length > 0)
            {
                return false;
            }

            var fileInfo = new FileInfo(this.FullName);
            return !fileInfo.Exists || fileInfo.Length == 0;
        }

        /// <summary>
        /// Opens this log file for appending text.
        /// This method can only be used if caching is disabled.
        /// </summary>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <returns>
        /// A new <see cref="StreamWriter"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// if caching is enabled.
        /// </exception>
        public StreamWriter AppendText(Encoding encoding)
        {
            if (this.cache != null)
            {
                throw new NotSupportedException("Caching is enabled");
            }

            return new StreamWriter(this.FullName, true, encoding);
        }

        /// <summary>
        /// Writes the given bytes to the cache and flushes the file if necessary.
        /// This method can only be used if caching is enabled.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset into the <see cref="data"/>.
        /// </param>
        /// <param name="length">
        /// The number of bytes from the <see cref="offset"/> in the <see cref="data"/>.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// if caching is disabled.
        /// </exception>
        public void WriteBytesToCache(byte[] data, int offset, int length)
        {
            if (this.cache == null)
            {
                throw new NotSupportedException("Caching is disabled");
            }

            lock (this.cache)
            {
                if (this.cache.Length + length > this.cache.Capacity)
                {
                    this.FlushCache();
                }

                if (length > this.cache.Capacity)
                {
                    // special case: if the log entry is bigger than the cache, write directly to the file
                    this.WriteBytesToFile(data, offset, length);
                    return;
                }

                this.cache.Write(data, offset, length);
            }
        }

        /// <summary>
        /// Flushes the underlying cache.
        /// If caching is disabled or the cache is empty, this method does nothing.
        /// </summary>
        public void FlushCache()
        {
            if (this.cache == null)
            {
                return;
            }

            this.cacheTimeout.Enabled = false;
            try
            {
                if (this.cache.Length == 0)
                {
                    return;
                }

                lock (this.cache)
                {
                    this.WriteBytesToFile(this.cache.GetBuffer(), 0, (int)this.cache.Length);
                    this.cache.SetLength(0);
                }
            }
            finally
            {
                this.cacheTimeout.Enabled = true;
            }
        }

        /// <summary>
        /// Archives this file to the given location and removes this file;
        /// subsequent calls to other methods will automatically create a new file when needed.
        /// </summary>
        /// <param name="archiveFile">
        /// The archive file location; this has to be a full path.
        /// </param>
        /// <param name="compression">
        /// The compression type used when compressing.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// if <see cref="compression"/> is unknown.
        /// </exception>
        public void Archive(string archiveFile, CompressionType compression)
        {
            this.FlushCache();

            Stream archiveOutput;
            switch (compression)
            {
                case CompressionType.None:
                    File.Move(this.FullName, archiveFile);
                    return;
                case CompressionType.GZIP:
                    archiveOutput = new GZipStream(File.Create(archiveFile), CompressionMode.Compress);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }

            try
            {
                using (archiveOutput)
                {
                    using (var input = File.OpenRead(this.FullName))
                    {
                        // little trick to save big memory allocations:
                        // reuse the cache buffer if we have one (it will be unused while archiving)
                        var buffer = this.cache != null ? this.cache.GetBuffer() : new byte[64 * 1024];
                        int read;
                        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            archiveOutput.Write(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                File.Delete(this.FullName);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.FlushCache();
            if (this.cacheTimeout != null)
            {
                this.cacheTimeout.Enabled = false;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var other = obj as LogFile;
            return other != null
                   && other.FullName.Equals(this.FullName, StringComparison.InvariantCultureIgnoreCase);
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
            return this.FullName.GetHashCode();
        }

        private void WriteBytesToFile(byte[] data, int offset, int length)
        {
            using (var output = new FileStream(this.FullName, FileMode.Append, FileAccess.Write))
            {
                output.Write(data, offset, length);
            }
        }
    }
}