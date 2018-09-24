// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITextPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITextPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities;

    /// <summary>
    /// Interface for a part of a formatted text that shows a text.
    /// </summary>
    public interface ITextPart : IPart
    {
        /// <summary>
        /// Gets the text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the font.
        /// </summary>
        Font Font { get; }
    }
}