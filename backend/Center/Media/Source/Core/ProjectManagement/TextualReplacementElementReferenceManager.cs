// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextualReplacementElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextualReplacementElementReferenceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels;

    /// <summary>
    /// The textual replacement element reference manager.
    /// </summary>
    public class TextualReplacementElementReferenceManager
        : ElementReferenceManagerBase<TextualReplacementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(TextualReplacementDataViewModel item)
        {
            this.SetReferenceFilename(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(TextualReplacementDataViewModel item)
        {
            this.UnsetReferenceFilename(item);
        }

        /// <summary>
        /// Adds the reference for the Filename property.
        /// </summary>
        /// <param name="replacement">
        /// The textual replacement element.
        /// </param>
        private void SetReferenceFilename(TextualReplacementDataViewModel replacement)
        {
            if (replacement.Filename == null || string.IsNullOrEmpty(replacement.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(replacement.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(replacement, "Filename");
        }

        /// <summary>
        /// Removes the reference for the Filename property.
        /// </summary>
        /// <param name="replacement">
        /// The textual replacement element.
        /// </param>
        private void UnsetReferenceFilename(TextualReplacementDataViewModel replacement)
        {
            if (replacement.Filename == null || string.IsNullOrEmpty(replacement.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(replacement.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(replacement, "Filename");
        }
    }
}