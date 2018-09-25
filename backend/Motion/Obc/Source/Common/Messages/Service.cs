// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Service.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Service type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The service data.
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Gets or sets the current service number
        /// </summary>
        public int Umlauf { get; set; }

        /// <summary>
        /// Gets or sets the current line number
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType() + ", Line: " + this.Line + ", Umlauf: " + this.Umlauf;
        }
    }
}