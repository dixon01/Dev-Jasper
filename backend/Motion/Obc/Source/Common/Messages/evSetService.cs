// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evSetService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evSetService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The set service event.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
        Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once InconsistentNaming
    public class evSetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evSetService"/> class.
        /// </summary>
        public evSetService()
        {
            this.Umlauf = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evSetService"/> class.
        /// </summary>
        /// <param name="umlauf">
        /// The service.
        /// </param>
        public evSetService(int umlauf)
        {
            this.Umlauf = umlauf;
        }

        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        public int Umlauf { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType() + ", Umlauf: " + this.Umlauf;
        }
    }
}