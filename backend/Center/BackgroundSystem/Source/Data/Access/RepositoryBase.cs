// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RepositoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Access
{
    using NLog;

    /// <summary>
    /// Defines a base class for repositories.
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        protected RepositoryBase()
        {
            this.Logger = LogManager.GetCurrentClassLogger();
        }
    }
}