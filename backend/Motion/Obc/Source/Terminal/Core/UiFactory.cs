// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UiFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UiFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The UI factory.
    /// </summary>
    public abstract class UiFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UiFactory"/> class.
        /// </summary>
        protected UiFactory()
        {
            if (Instance != null)
            {
                throw new NotSupportedException("Can't have more than one UiFactory.");
            }

            Instance = this;
        }

        /// <summary>
        /// Gets the single instance.
        /// </summary>
        public static UiFactory Instance { get; private set; }

        /// <summary>
        /// This method is not really for much, it just looks nicer to have a
        /// template method to create the instance instead of calling the constructor directly.
        /// </summary>
        /// <typeparam name="TUiFactory">type of the <see cref="UiFactory"/> to create</typeparam>
        public static void CreateInstance<TUiFactory>() where TUiFactory : UiFactory, new()
        {
            Instance = new TUiFactory();
        }

        /// <summary>
        /// Creates the root.
        /// </summary>
        /// <returns>
        /// The <see cref="IUiRoot"/> implementation.
        /// </returns>
        public abstract IUiRoot CreateRoot();
    }
}