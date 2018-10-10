// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBbParserContext.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBbParserContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    /// <summary>
    /// Interface to be implemented by the context in which
    /// the BBCode is being parsed.
    /// </summary>
    public interface IBbParserContext
    {
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