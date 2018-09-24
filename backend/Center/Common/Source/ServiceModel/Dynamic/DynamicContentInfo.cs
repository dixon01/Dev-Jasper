// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicContentInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicContentInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Dynamic
{
    using System.Collections.Generic;

    using Gorba.Center.Common.ServiceModel.Update;

    /// <summary>
    /// Information about all dynamic content that is related
    /// to an <see cref="UpdatePart"/>.
    /// This object is used as value of <see cref="UpdatePart.DynamicContent"/>.
    /// </summary>
    public class DynamicContentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicContentInfo"/> class.
        /// </summary>
        public DynamicContentInfo()
        {
            this.Parts = new List<DynamicContentPartBase>();
        }

        /// <summary>
        /// Gets or sets the list of parts.
        /// </summary>
        public List<DynamicContentPartBase> Parts { get; set; }
    }
}
