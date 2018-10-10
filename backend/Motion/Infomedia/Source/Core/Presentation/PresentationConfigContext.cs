// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationConfigContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationConfigContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Implementation of <see cref="IPresentationConfigContext"/>.
    /// </summary>
    internal class PresentationConfigContext : IPresentationConfigContext
    {
        private readonly ConfigManager<InfomediaConfig> configManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationConfigContext"/> class.
        /// </summary>
        /// <param name="filename">
        /// The config filename.
        /// </param>
        public PresentationConfigContext(string filename)
        {
            this.configManager = new ConfigManager<InfomediaConfig> { FileName = filename, EnableCaching = true };
        }

        /// <summary>
        /// Gets the configuration of the currently running presentation.
        /// </summary>
        public InfomediaConfig Config
        {
            get
            {
                return this.configManager.Config;
            }
        }

        /// <summary>
        /// Converts the given file name into an absolute name
        /// relative to the presentation config file.
        /// </summary>
        /// <param name="filename">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        public string GetAbsolutePathRelatedToConfig(string filename)
        {
            return this.configManager.GetAbsolutePathRelatedToConfig(filename);
        }
    }
}