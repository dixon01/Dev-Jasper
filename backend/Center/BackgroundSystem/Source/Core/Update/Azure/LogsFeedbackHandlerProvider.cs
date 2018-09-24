// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogsFeedbackHandlerProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The logs feedback handler provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update.Azure
{
    using System;

    /// <summary>
    /// The logs feedback handler provider.
    /// </summary>
    public abstract class LogsFeedbackHandlerProvider
    {
        static LogsFeedbackHandlerProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static LogsFeedbackHandlerProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = DefaultLogsFeedbackHandlerProvider.Instance;
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(LogsFeedbackHandlerProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LogsFeedbackHandlerBase"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="LogsFeedbackHandlerBase"/>.
        /// </returns>
        public abstract LogsFeedbackHandlerBase Create();

        private class DefaultLogsFeedbackHandlerProvider : LogsFeedbackHandlerProvider
        {
            static DefaultLogsFeedbackHandlerProvider()
            {
                Instance = new DefaultLogsFeedbackHandlerProvider();
            }

            public static DefaultLogsFeedbackHandlerProvider Instance { get; private set; }

            public override LogsFeedbackHandlerBase Create()
            {
                return new LocalLogsFeedbackHandler();
            }
        }
    }
}
