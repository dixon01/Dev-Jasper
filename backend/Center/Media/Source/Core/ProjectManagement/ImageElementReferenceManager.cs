// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageElementReferenceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The image element reference manager.
    /// </summary>
    public class ImageElementReferenceManager : ElementReferenceManagerBase<ImageElementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(ImageElementDataViewModel item)
        {
            this.SetReferenceFilename(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(ImageElementDataViewModel item)
        {
            this.UnsetReferenceFilename(item);
        }

        /// <summary>
        /// Adds the reference to the Filename property.
        /// </summary>
        /// <param name="image">
        /// The image element.
        /// </param>
        private void SetReferenceFilename(ImageElementDataViewModel image)
        {
            if (image.Filename == null || string.IsNullOrEmpty(image.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(image.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(image, "Filename");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Removes the reference to the Filename property.
        /// </summary>
        /// <param name="image">
        /// The image element.
        /// </param>
        private void UnsetReferenceFilename(ImageElementDataViewModel image)
        {
            if (image.Filename == null || string.IsNullOrEmpty(image.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(image.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(image, "Filename");
            resource.UpdateIsUsedVisible();
        }
    }
}