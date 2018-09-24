// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioElementReferenceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioElementReferenceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// The audio element reference manager.
    /// </summary>
    public class AudioElementReferenceManager : ElementReferenceManagerBase<AudioFileElementDataViewModel>
    {
        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void SetReferences(AudioFileElementDataViewModel item)
        {
            this.SetReferenceFilename(item);
        }

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public override void UnsetReferences(AudioFileElementDataViewModel item)
        {
            this.UnsetReferenceFilename(item);
        }

        /// <summary>
        /// Adds the reference for the Filename property.
        /// </summary>
        /// <param name="audio">
        /// The audio element.
        /// </param>
        private void SetReferenceFilename(AudioFileElementDataViewModel audio)
        {
            if (audio.Filename == null || string.IsNullOrEmpty(audio.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(audio.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.SetReference(audio, "Filename");
        }

        /// <summary>
        /// Removes the reference for the Filename property.
        /// </summary>
        /// <param name="audio">
        /// The audio element.
        /// </param>
        private void UnsetReferenceFilename(AudioFileElementDataViewModel audio)
        {
            if (audio.Filename == null || string.IsNullOrEmpty(audio.Filename.Value))
            {
                return;
            }

            var resource = this.GetResource(audio.Filename.Value);
            if (resource == null)
            {
                return;
            }

            resource.UnsetReference(audio, "Filename");
        }
    }
}