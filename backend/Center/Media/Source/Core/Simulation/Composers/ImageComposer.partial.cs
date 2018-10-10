// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageComposer.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Composer handling <see cref="ImageElementDataViewModel"/>.
    /// </summary>
    public partial class ImageComposer
    {
        partial void Initialize()
        {
            this.UpdateFilename();
        }

        partial void PreHandlePropertyChange(string propertyName, ref bool handled)
        {
            if (propertyName != "Filename")
            {
                return;
            }

            this.UpdateFilename();
            handled = true;
        }

        private void UpdateFilename()
        {
            var image = this.ViewModel.Image;
            this.Item.Filename = this.Context.GetImageFileName(image == null ? null : image.Hash);
        }
    }
}
