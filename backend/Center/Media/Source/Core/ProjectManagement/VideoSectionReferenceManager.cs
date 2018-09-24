// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSectionReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// The video section reference manager.
    /// </summary>
    public class VideoSectionReferenceManager : ElementReferenceManagerBase<VideoSectionConfigDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(VideoSectionConfigDataViewModel item)
        {
            this.SetReferenceVideoUri(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(VideoSectionConfigDataViewModel item)
        {
            this.UnsetReferenceVideoUri(item);
        }

        /// <summary>
        /// Adds the reference for the VideoUri property.
        /// </summary>
        /// <param name="image">
        /// The video section.
        /// </param>
        private void SetReferenceVideoUri(VideoSectionConfigDataViewModel image)
        {
            if (image.VideoUri == null || string.IsNullOrEmpty(image.VideoUri.Value))
            {
                return;
            }

            var resource = this.GetResource(image.VideoUri.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(image, "VideoUri");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Removes the reference for the Filename property.
        /// </summary>
        /// <param name="video">
        /// The video section.
        /// </param>
        private void UnsetReferenceVideoUri(VideoSectionConfigDataViewModel video)
        {
            if (video.VideoUri == null || string.IsNullOrEmpty(video.VideoUri.Value))
            {
                return;
            }

            var resource = this.GetResource(video.VideoUri.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(video, "VideoUri");
            resource.UpdateIsUsedVisible();
        }
    }
}