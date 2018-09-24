// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutResolutionCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutResolutionCollection.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using System.Linq;

    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// A collection helper for indexing resolutions
    /// </summary>
    public class LayoutResolutionCollection
    {
        private readonly ExtendedObservableCollection<ResolutionConfigDataViewModel> resolutions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutResolutionCollection"/> class.
        /// </summary>
        /// <param name="resolutions">the resolution collection</param>
        public LayoutResolutionCollection(ExtendedObservableCollection<ResolutionConfigDataViewModel> resolutions)
        {
            this.resolutions = resolutions;
        }

        /// <summary>
        /// the indexer to access the resolution by width and height
        /// </summary>
        /// <param name="width">the width</param>
        /// <param name="height">the height</param>
        /// <returns>the resolution</returns>
        public ResolutionConfigDataViewModel this[int width, int height]
        {
            get
            {
                var query = from resolution in this.resolutions
                            where resolution.Width.Value == width && resolution.Height.Value == height
                            select resolution;

                return query.FirstOrDefault();
            }
        }
    }
}