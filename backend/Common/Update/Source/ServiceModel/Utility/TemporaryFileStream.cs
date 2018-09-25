// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFileStream.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemporaryFileStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Utility
{
    using System;
    using System.IO;

    using Gorba.Common.Utility.Core.IO;

    /// <summary>
    /// Stream that deletes a temporary file when being closed.
    /// </summary>
    public sealed class TemporaryFileStream : WrapperStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFileStream"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public TemporaryFileStream(string fileName)
        {
            this.Name = fileName;
            try
            {
                this.Open(File.OpenRead(this.Name));
            }
            catch (Exception)
            {
                this.DeleteTemporaryFile();
                throw;
            }
        }

        /// <summary>
        /// Gets the file name passed to the constructor.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Closes the current stream and releases any resources
        /// (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            base.Close();
            this.DeleteTemporaryFile();
        }

        private void DeleteTemporaryFile()
        {
            try
            {
                File.Delete(this.Name);
            }
            catch (IOException)
            {
            }
        }
    }
}