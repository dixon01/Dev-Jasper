// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System.IO;

    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Media.Core.DataViewModels.Project;

    /// <summary>
    /// The extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a new instance of a <see cref="Resource"/> equivalent to the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">
        /// The model to convert.
        /// </param>
        /// <returns>
        /// A resource equivalent to the given model.
        /// </returns>
        public static Resource ToResource(this ResourceInfoDataViewModel model)
        {
            return new Resource { Hash = model.Hash, OriginalFilename = Path.GetFileName(model.Filename) };
        }
    }
}