// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitUploadFileManager.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitUploadFileManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// The manager that handles storage of uploaded files
    /// </summary>
    public class UnitUploadFileManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IWritableFileSystem fileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitUploadFileManager"/> class.
        /// </summary>
        /// <param name="fileSystem">The File System to use when saving the files</param>
        public UnitUploadFileManager(IWritableFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        /// <summary>
        /// Saves the given file and its contents asynchronously
        /// </summary>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <param name="contents">
        /// The file contents
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveUploadFileAsync(string fileName, Stream contents)
        {
            if (contents == null || contents.Length == 0)
            {
                Logger.Debug($"File {fileName} has no data, ignoring.");
                return;
            }

            Logger.Debug($"Writing file {fileName} length {contents.Length}");
            try
            {
                var file = this.fileSystem.CreateFile(fileName);
                using (var stream = file.OpenWrite())
                {
                    await contents.CopyToAsync(stream);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Unable to create or write {fileName}. Exception: {e.Message}");
            }
        }
    }
}