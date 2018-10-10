// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitOfWorkFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    /// <summary>
    /// Defines a factory for unit of works.
    /// </summary>
    public abstract class UnitOfWorkFactory
    {
        static UnitOfWorkFactory()
        {
            ResetInstance();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static UnitOfWorkFactory Current { get; private set; }

        public static void ResetInstance()
        {
            SetInstance(DefaultUnitOfWorkFactory.Instance);
        }

        public static void SetInstance(UnitOfWorkFactory instance)
        {
            UnitOfWorkFactory.Current = instance;
        }

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IUnitOfWork"/>.
        /// </returns>
        public abstract IUnitOfWork Create();

        private sealed class DefaultUnitOfWorkFactory : UnitOfWorkFactory
        {
            static DefaultUnitOfWorkFactory()
            {
                Instance = new DefaultUnitOfWorkFactory();
            }

            public static UnitOfWorkFactory Instance { get; private set; }

            public override Gorba.Center.BackgroundSystem.Data.Access.IUnitOfWork Create()
            {
                var unitOfWork = new Gorba.Center.BackgroundSystem.Data.Access.UnitOfWork();
                var dataContext = DataContextFactory.Current.Create();
                unitOfWork.Initialize(dataContext);
                return unitOfWork;
            }
        }
    }
}