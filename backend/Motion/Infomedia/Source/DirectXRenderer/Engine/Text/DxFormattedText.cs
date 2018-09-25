// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxFormattedText.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxFormattedText type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.RendererBase.Text;

    /// <summary>
    /// A <see cref="FormattedText{TPart}"/> for DirectX.
    /// It contains additional methods to help rendering text.
    /// </summary>
    public class DxFormattedText : FormattedText<DxPart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DxFormattedText"/> class.
        /// </summary>
        public DxFormattedText()
        {
            this.RenderParts = new List<DxPart>();
        }

        /// <summary>
        /// Gets all parts.
        /// </summary>
        public List<DxPart> RenderParts { get; private set; }

        /// <summary>
        /// Updates the content of all parts of this text.
        /// See also: <see cref="DxPart.UpdateContent"/>
        /// </summary>
        /// <returns>
        /// True if any of the parts have been updated
        /// (i.e. <see cref="DxPart.UpdateContent"/> returned true).
        /// </returns>
        public bool UpdateContent()
        {
            var shouldUpdate = false;
            foreach (var line in this.Lines)
            {
                foreach (var part in line.Parts)
                {
                    shouldUpdate |= part.UpdateContent();
                }
            }

            return shouldUpdate;
        }

        /// <summary>
        /// Clears the <see cref="RenderParts"/> list and disposes of all
        /// objects that are no longer required (i.e. are not in <see cref="FormattedText{TPart}.Parts"/>).
        /// </summary>
        public void ClearRenderParts()
        {
            foreach (var part in this.RenderParts)
            {
                var found = false;
                foreach (var line in this.Lines)
                {
                    if (line.Parts.Contains(part))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    part.Dispose();
                }
            }

            this.RenderParts.Clear();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            foreach (var line in this.Lines)
            {
                foreach (var part in line.Parts)
                {
                    this.RenderParts.Remove(part);
                    part.Dispose();
                }
            }

            foreach (var part in this.RenderParts)
            {
                part.Dispose();
            }
        }
    }
}