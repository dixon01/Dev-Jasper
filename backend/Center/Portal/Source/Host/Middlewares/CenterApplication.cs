// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CenterApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CenterApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    /// <summary>
    /// The center application.
    /// </summary>
    public class CenterApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CenterApplication"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        public CenterApplication(string key, string path, string displayName)
        {
            this.Key = key;
            this.DisplayName = displayName;
            this.Path = path;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; private set; }
    }
}