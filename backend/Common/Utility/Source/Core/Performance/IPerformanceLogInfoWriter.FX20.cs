// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPerformanceLogInfoWriter.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPerformanceLogInfoWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Performance
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a generic component to write performance log information.
    /// </summary>
    public partial interface IPerformanceLogInfoWriter
    {
        /// <summary>
        /// Logs the specified category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="id">The id.</param>
        /// <param name="properties">The properties.</param>
        void Mark(string category, string tag, int id, params KeyValuePair<string, string>[] properties);
    }
}