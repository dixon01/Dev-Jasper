// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PageModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System.Collections.Generic;

    /// <summary>
    /// The page model.
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageModel"/> class.
        /// </summary>
        public PageModel()
        {
            this.CenterApplications = new List<CenterApplication>();
        }

        /// <summary>
        /// Gets or sets the center applications.
        /// </summary>
        public ICollection<CenterApplication> CenterApplications { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Portal should provide the Beta versions of the applications
        /// (<c>true</c>) or not (<c>false</c>).
        /// </summary>
        public bool ClickOnceUseBeta { get; set; }
    }
}