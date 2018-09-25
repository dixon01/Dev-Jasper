// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility;

    /// <summary>
    /// Defines a file resource.
    /// </summary>
    public class FileResource : ResourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileResource"/> class.
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
        public FileResource(AccountInfo accountInfo, string hash, string originalFileName, long length)
            : base(accountInfo, hash, originalFileName, length)
        {
        }

        /// <summary>
        /// Downloads a resource.
        /// </summary>
        /// <param name="open">
        /// The open.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task DownloadResourceAsync(bool open = false)
        {
            var client = this.AccountInfo.GetCloudFileShare();
            var file = client.GetFileReference(string.Format("{0}.rx", this.Hash));
            if (file == null)
            {
                MessageBox.Show("File doesn't exist");
                return;
            }

            var path = this.GetFullTempPath();
            await file.DownloadToFileAsync(path, FileMode.Create);
            Process.Start(path);
        }
    }
}