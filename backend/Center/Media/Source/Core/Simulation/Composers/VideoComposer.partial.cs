// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoComposer.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Composer handling <see cref="VideoElementDataViewModel" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Composer handling <see cref="VideoElementDataViewModel"/>.
    /// </summary>
    public partial class VideoComposer
    {
        partial void Initialize()
        {
            this.UpdateVideoUri();
            this.UpdateFallbackUri();
        }

        partial void PreHandlePropertyChange(string propertyName, ref bool handled)
        {
            if (propertyName != "VideoUri")
            {
                return;
            }

            this.UpdateVideoUri();
            handled = true;
        }

        private void UpdateVideoUri()
        {
            this.Item.VideoUri = this.Context.GetVideoUri(this.ViewModel.Hash);
        }

        private void UpdateFallbackUri()
        {
            if (string.IsNullOrEmpty(this.Item.FallbackImage))
            {
                return;
            }

            this.Item.FallbackImage = this.Context.GetResourceUriByFilename(this.Item.FallbackImage);
        }
    }
}