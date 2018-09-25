// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainLoginValidatorProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DomainLoginValidatorProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    /// <summary>
    /// Provider for <see cref="LoginValidatorBase"/> that supports local domain authentication.
    /// </summary>
    public class DomainLoginValidatorProvider : LoginValidatorProvider
    {
        private readonly DomainLoginValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainLoginValidatorProvider"/> class.
        /// </summary>
        public DomainLoginValidatorProvider()
        {
            this.validator = new DomainLoginValidator();
        }

        /// <summary>
        /// Provides a login validator.
        /// </summary>
        /// <returns>
        /// The <see cref="LoginValidatorBase"/>.
        /// </returns>
        public override LoginValidatorBase Provide()
        {
            return this.validator;
        }
    }
}