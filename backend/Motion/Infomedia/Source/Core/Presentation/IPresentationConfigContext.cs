// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentationConfigContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentationConfigContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Context for presentation that provides also the possibility to
    /// find files related to the default IM2 file.
    /// </summary>
    public interface IPresentationConfigContext
    {
        /// <summary>
        /// Gets the configuration of the currently running presentation.
        /// </summary>
        InfomediaConfig Config { get; }

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
        string GetAbsolutePathRelatedToConfig(string filename);
    }
}