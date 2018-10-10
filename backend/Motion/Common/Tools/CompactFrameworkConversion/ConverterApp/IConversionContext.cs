// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConversionContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IConversionContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp
{
    using Gorba.Motion.Common.Tools.CompactFrameworkConversion.ConverterApp.Config;

    /// <summary>
    /// The context of a solution conversion.
    /// </summary>
    public interface IConversionContext
    {
        /// <summary>
        /// Gets the project specific conversion config for the given project file.
        /// </summary>
        /// <param name="projectFile">
        /// The full path to the project file.
        /// </param>
        /// <param name="postfix">
        /// The file postfix used during the conversion.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectConversionConfig"/>.
        /// </returns>
        ProjectConversionConfig GetProjectConfig(string projectFile, string postfix);
    }
}