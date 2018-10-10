// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeHistoryFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeHistoryFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.History
{
    using System;

    /// <summary>
    /// Factory to create Change
    /// </summary>
    public abstract class ChangeHistoryFactory
    {
        static ChangeHistoryFactory()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static ChangeHistoryFactory Current { get; private set; }

        /// <summary>
        /// Resets the Current factory to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultChangeHistoryFactory.Instance);
        }

        /// <summary>
        /// Sets the specified instance as the Current factory.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void SetCurrent(ChangeHistoryFactory instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Creates a new instance of an implementation of the <see cref="IChangeHistory"/> interface.
        /// </summary>
        /// <returns>A new instance of an implementation of the <see cref="ChangeHistory"/> interface.</returns>
        public abstract IChangeHistory Create();

        /// <summary>
        /// Default implementation of the factory returning new <see cref="ChangeHistory"/> objects.
        /// </summary>
        internal sealed class DefaultChangeHistoryFactory : ChangeHistoryFactory
        {
            private static readonly Lazy<DefaultChangeHistoryFactory> LazyInstance =
                new Lazy<DefaultChangeHistoryFactory>(CreateDefaultChangeHistoryFactory);

            /// <summary>
            /// Gets the instance.
            /// </summary>
            public static DefaultChangeHistoryFactory Instance
            {
                get
                {
                    return LazyInstance.Value;
                }
            }

            /// <summary>
            /// Creates a new instance of an implementation of the <see cref="IChangeHistory"/> interface.
            /// </summary>
            /// <returns>
            /// A new instance of an implementation of the <see cref="ChangeHistory"/> interface.
            /// </returns>
            public override IChangeHistory Create()
            {
                return new ChangeHistory();
            }

            private static DefaultChangeHistoryFactory CreateDefaultChangeHistoryFactory()
            {
                return new DefaultChangeHistoryFactory();
            }
        }
    }
}