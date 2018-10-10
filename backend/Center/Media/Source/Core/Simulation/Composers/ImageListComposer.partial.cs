// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListComposer.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageListComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Composer handling <see cref="ImageListElementDataViewModel"/>.
    /// </summary>
    public partial class ImageListComposer
    {
        partial void Initialize()
        {
            this.UpdateImages();
        }

        partial void PreHandlePropertyChange(string propertyName, ref bool handled)
        {
            if (propertyName != "TestData")
            {
                return;
            }

            this.UpdateImages();
            handled = true;
        }

        private void UpdateImages()
        {
            if (this.ViewModel.TestData == null || string.IsNullOrEmpty(this.ViewModel.TestData.Value))
            {
                this.Item.Images = new string[0];
                return;
            }

            var parts = ArrayUtil.SplitString(this.ViewModel.TestData.Value, this.ViewModel.Delimiter.Value);
            var resources = this.Context.ApplicationState.CurrentProject.Resources;

            this.Item.Images =
                parts.Select(
                    p =>
                        resources.FirstOrDefault(
                            r =>
                                r.Type == ResourceType.Symbol
                                && p.Trim().Equals(
                                        Path.GetFileNameWithoutExtension(r.Filename),
                                        StringComparison.InvariantCultureIgnoreCase)))
                     .Select(r => r == null ? "-" : this.Context.GetImageFileName(r.Hash))
                     .ToArray();
        }
    }
}
