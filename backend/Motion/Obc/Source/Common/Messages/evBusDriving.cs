// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evBusDriving.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evBusDriving type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The bus driving event.
    /// </summary>
    public class evBusDriving
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evBusDriving"/> class.
        /// </summary>
        public evBusDriving()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evBusDriving"/> class.
        /// </summary>
        /// <param name="isDriving">
        /// The is driving.
        /// </param>
        public evBusDriving(bool isDriving)
        {
            this.IsDriving = isDriving;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bus is driving.
        /// </summary>
        public bool IsDriving { get; set; }
    }
}