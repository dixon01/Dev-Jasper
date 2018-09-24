// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginValidatorProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the login validator factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Security
{
    using System;

    /// <summary>
    /// Defines the login validator factory.
    /// </summary>
    public abstract class LoginValidatorProvider
    {
        static LoginValidatorProvider()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static LoginValidatorProvider Current { get; private set; }

        /// <summary>
        /// Sets the current factory.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void SetCurrent(LoginValidatorProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Resets the current factory to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultLoginValidatorProvider.Instance);
        }

        /// <summary>
        /// Provides a login validator.
        /// </summary>
        /// <returns>
        /// The <see cref="LoginValidatorBase"/>.
        /// </returns>
        public abstract LoginValidatorBase Provide();

        private class DefaultLoginValidatorProvider : LoginValidatorProvider
        {
            private readonly DatabaseLoginValidator validator;

            static DefaultLoginValidatorProvider()
            {
                Instance = new DefaultLoginValidatorProvider();
            }

            private DefaultLoginValidatorProvider()
            {
                this.validator = new DatabaseLoginValidator();
            }

            public static DefaultLoginValidatorProvider Instance { get; private set; }

            public override LoginValidatorBase Provide()
            {
                return this.validator;
            }
        }
    }
}
