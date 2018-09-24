// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabiMerge
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Extensions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Extensions"/> class.
        /// </summary>
        public Extensions()
        {
            this.TopboxSoftware = string.Empty;
            this.TopboxData = string.Empty;
            this.CuSoftware = string.Empty;
            this.CuData = string.Empty;
        }

        /// <summary>
        /// Gets or sets TopboxSoftware.
        /// </summary>
        public string TopboxSoftware { get; set; }

        /// <summary>
        /// Gets or sets TopboxData.
        /// </summary>
        public string TopboxData { get; set; }

        /// <summary>
        /// Gets or sets CuSoftware.
        /// </summary>
        public string CuSoftware { get; set; }

        /// <summary>
        /// Gets or sets CuData.
        /// </summary>
        public string CuData { get; set; }
    }
}
