// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoElementReferenceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The video element reference manager.
    /// </summary>
    public class VideoElementReferenceManager : ElementReferenceManagerBase<VideoElementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(VideoElementDataViewModel item)
        {
            this.SetReferenceVideoUri(item);
            this.SetReferenceFallbackImage(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(VideoElementDataViewModel item)
        {
            this.UnsetReferenceVideoUri(item);
            this.UnsetReferenceFallbackImage(item);
        }

        /// <summary>
        /// Adds the reference for the VideoUri property.
        /// </summary>
        /// <param name="video">
        /// The video element.
        /// </param>
        private void SetReferenceVideoUri(VideoElementDataViewModel video)
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

            resource.SetReference(video, "VideoUri");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Removes the reference for the VideoUri property.
        /// </summary>
        /// <param name="video">
        /// The video element.
        /// </param>
        private void UnsetReferenceVideoUri(VideoElementDataViewModel video)
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

        /// <summary>
        /// Adds the reference for the FallbackImage property.
        /// </summary>
        /// <param name="video">
        /// The video element.
        /// </param>
        private void SetReferenceFallbackImage(VideoElementDataViewModel video)
        {
            if (video.FallbackImage == null || string.IsNullOrEmpty(video.FallbackImage.Value))
            {
                return;
            }

            var resource = this.GetResource(video.FallbackImage.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(video, "FallbackImage");
            resource.UpdateIsUsedVisible();
        }

        /// <summary>
        /// Adds the reference for the FallbackImage property.
        /// </summary>
        /// <param name="video">
        /// The video element.
        /// </param>
        private void UnsetReferenceFallbackImage(VideoElementDataViewModel video)
        {
            if (video.FallbackImage == null || string.IsNullOrEmpty(video.FallbackImage.Value))
            {
                return;
            }

            var resource = this.GetResource(video.FallbackImage.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(video, "FallbackImage");
            resource.UpdateIsUsedVisible();
        }
    }
}