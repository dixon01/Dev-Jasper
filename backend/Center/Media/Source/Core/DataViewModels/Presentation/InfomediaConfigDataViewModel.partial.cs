// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfomediaConfigDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The InfomediaConfigDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The InfomediaConfigDataViewModel.
    /// </summary>
    public partial class InfomediaConfigDataViewModel
    {
        /// <summary>
        /// The get elements of type iterator.
        /// </summary>
        /// <typeparam name="T">
        /// The element type to search for.
        /// </typeparam>
        /// <returns>
        /// The IEnumerable of all found elements/>.
        /// </returns>
        public IEnumerable<T> GetElementsOfType<T>()
        {
            return from layout in this.Layouts
                   from resolution in layout.Resolutions
                   from element in resolution.Elements.OfType<T>()
                   select element;
        }

        partial void ExportNotGeneratedValues(InfomediaConfig model, object exportParameters)
        {
            if (model == null)
            {
                return;
            }

            var resourceFonts = this.mediaShell.MediaApplicationState.CurrentProject.Resources.Where(
                    r => r.Type == ResourceType.Font && r.ShouldExport).ToList();
            var infomediaFonts = model.Fonts;
            var fontsToRemove = new List<FontConfig>();
            foreach (var font in infomediaFonts)
            {
                var resourceFont =
                    resourceFonts.FirstOrDefault(
                        f => f.Filename != null && font.Path.Contains(Path.GetFileName(f.Filename)));
                if (resourceFont != null)
                {
                    continue;
                }

                fontsToRemove.Add(font);
            }

            foreach (var fontConfig in fontsToRemove)
            {
                infomediaFonts.Remove(fontConfig);
            }
        }
    }
}