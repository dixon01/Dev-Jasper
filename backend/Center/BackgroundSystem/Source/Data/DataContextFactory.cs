// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataContextFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data
{
    using System;
    using System.Data.Entity.Infrastructure;

    /// <summary>
    /// Defines the data context factory.
    /// </summary>
    public abstract class DataContextFactory
    {
        static DataContextFactory()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static DataContextFactory Current { get; private set; }

        /// <summary>
        /// Sets the current factory.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void SetCurrent(DataContextFactory instance)
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
            SetCurrent(DefaultDataContextFactory.Instance);
        }

        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <returns>
        /// The <see cref="CenterDataContext"/>.
        /// </returns>
        public abstract CenterDataContext Create();

        /// <summary>
        /// Gets the database connection info.
        /// </summary>
        /// <returns>
        /// The <see cref="DbConnectionInfo"/>.
        /// </returns>
        public abstract DbConnectionInfo GetDbConnectionInfo();

        private class DefaultDataContextFactory : DataContextFactory
        {
            static DefaultDataContextFactory()
            {
                Instance = new DefaultDataContextFactory();
            }

            public static DefaultDataContextFactory Instance { get; private set; }

            public override CenterDataContext Create()
            {
                return new CenterDataContext();
            }

            public override DbConnectionInfo GetDbConnectionInfo()
            {
                return new DbConnectionInfo("CenterDataContext");
            }
        }
    }
}