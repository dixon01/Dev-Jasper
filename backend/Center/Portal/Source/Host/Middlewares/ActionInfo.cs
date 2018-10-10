// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActionInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.Utility.Files;

    /// <summary>
    /// The action info.
    /// </summary>
    [DebuggerDisplay("{Name}, {IsRazorTemplate}")]
    internal class ActionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <param name="isRazorTemplate">
        /// The is razor.
        /// </param>
        public ActionInfo(string name, IFileInfo fileInfo, bool isRazorTemplate)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name can't be null or empty", "name");
            }

            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }

            this.IsRazorTemplate = isRazorTemplate;
            this.FileInfo = fileInfo;
            this.Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the file info.
        /// </summary>
        public IFileInfo FileInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is a razor template.
        /// </summary>
        public bool IsRazorTemplate { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "{{ActionInfo {0}, {1}, {2}}}",
                this.Name,
                this.FileInfo.FullName,
                this.IsRazorTemplate);
        }
    }
}