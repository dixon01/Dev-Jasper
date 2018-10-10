// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSectionReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// The image section reference manager.
    /// </summary>
    public class ImageSectionReferenceManager : ElementReferenceManagerBase<ImageSectionConfigDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(ImageSectionConfigDataViewModel item)
        {
            this.SetReferenceFilename(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(ImageSectionConfigDataViewModel item)
        {
            this.UnsetReferenceFilename(item);
        }

        /// <summary>
        /// Adds the reference for the Filename property.
        /// </summary>
        /// <param name="image">
        /// The image section.
        /// </param>
        private void SetReferenceFilename(ImageSectionConfigDataViewModel image)
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
        /// Removes the reference for the Filename property.
        /// </summary>
        /// <param name="image">
        /// The image section.
        /// </param>
        private void UnsetReferenceFilename(ImageSectionConfigDataViewModel image)
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