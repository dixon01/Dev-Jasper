// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Software
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A file inside a software package version.
    /// </summary>
    public class FileItem : FileSystemItemBase
    {
        private string originalFileName;

        private string hash;

        /// <summary>
        /// Gets or sets the original file name.
        /// This property is set when an item is added by the user.
        /// In this case <see cref="Hash"/> needs to be null.
        /// </summary>
        public string OriginalFileName
        {
            get
            {
                return this.originalFileName;
            }

            set
            {
                if (!this.SetProperty(ref this.originalFileName, value, () => this.OriginalFileName))
                {
                    return;
                }

                this.Name = Path.GetFileName(value);
            }
        }

        /// <summary>
        /// Gets or sets the hash of the file.
        /// This property is set when an item is already in the database.
        /// In this case <see cref="OriginalFileName"/> needs to be null.
        /// </summary>
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.SetProperty(ref this.hash, value, () => this.Hash);
            }
        }

        /// <summary>
        /// Gets the items inside this item.
        /// This will always return an empty list.
        /// </summary>
        public override IEnumerable<FileSystemItemBase> Items
        {
            get
            {
                return new FileSystemItemBase[0];
            }
        }
    }
}