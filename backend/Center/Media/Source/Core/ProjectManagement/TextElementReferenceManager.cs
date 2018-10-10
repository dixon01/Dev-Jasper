// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextElementReferenceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The text element reference manager.
    /// </summary>
    public class TextElementReferenceManager : ElementReferenceManagerBase<TextElementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(TextElementDataViewModel item)
        {
            this.SetReferenceFontFace(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(TextElementDataViewModel item)
        {
            this.UnsetReferenceFontFace(item);
        }

        /// <summary>
        /// Adds the reference for the FontFace property.
        /// </summary>
        /// <param name="text">
        /// The text element.
        /// </param>
        private void SetReferenceFontFace(TextElementDataViewModel text)
        {
            if (text.FontFace == null || string.IsNullOrEmpty(text.FontFace.Value))
            {
                return;
            }

            var faces = text.FontFace.Value.Split(';');
            foreach (var resource in faces.Select(this.GetResource))
            {
                if (resource == null)
                {
                    return;
                }

                resource.SetReference(text, "FontFace");
                resource.UpdateIsUsedVisible();
            }
        }

        /// <summary>
        /// Removes the reference for the FontFace property.
        /// </summary>
        /// <param name="text">
        /// The text element.
        /// </param>
        private void UnsetReferenceFontFace(TextElementDataViewModel text)
        {
            if (text.FontFace == null || string.IsNullOrEmpty(text.FontFace.Value))
            {
                return;
            }

            var faces = text.FontFace.Value.Split(';');
            foreach (var resource in faces.Select(this.GetResource))
            {
                if (resource == null)
                {
                    return;
                }

                resource.UnsetReference(text, "FontFace");
                resource.UpdateIsUsedVisible();
            }
        }
    }
}