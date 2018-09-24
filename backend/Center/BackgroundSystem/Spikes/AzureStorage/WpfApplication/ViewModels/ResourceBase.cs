// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility;

    /// <summary>
    /// The base class for resource view models.
    /// </summary>
    public abstract class ResourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceBase"/> class.
        /// </summary>
        /// <param name="accountInfo">
        /// The account info.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        protected ResourceBase(AccountInfo accountInfo, string hash, string originalFileName, long length)
        {
            this.AccountInfo = accountInfo;
            this.Hash = hash;
            this.OriginalFileName = originalFileName;
            this.Length = length;
            this.DownloadCommand = new AwaitableDelegateCommand(this.DownloadAsync);
        }

        /// <summary>
        /// Gets the account info.
        /// </summary>
        public AccountInfo AccountInfo { get; private set; }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// Gets the original file name.
        /// </summary>
        public string OriginalFileName { get; private set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets the download command.
        /// </summary>
        public ICommand DownloadCommand { get; private set; }

        /// <summary>
        /// Downloads a resource.
        /// </summary>
        /// <param name="open">
        /// The open.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected abstract Task DownloadResourceAsync(bool open = false);

        /// <summary>
        /// Gets a full path to a temp file.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected virtual string GetFullTempPath()
        {
            var originalFileInfo = new FileInfo(this.OriginalFileName);
            var newFileName = string.Format("{0}{1}", Guid.NewGuid(), originalFileInfo.Extension);
            var tempPath = Path.GetTempPath();
            var fullPath = Path.Combine(tempPath, newFileName);
            return fullPath;
        }

        private async Task DownloadAsync()
        {
            await this.DownloadResourceAsync(true);
        }
    }
}