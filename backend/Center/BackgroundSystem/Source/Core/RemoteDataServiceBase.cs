// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataServiceBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteDataServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System;

    using NLog;

    /// <summary>
    /// Defines a base class for remote data services.
    /// </summary>
    public abstract class RemoteDataServiceBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDataServiceBase"/> class.
        /// </summary>
        protected RemoteDataServiceBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Logs details about an occurred exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        protected virtual void OnError(Exception exception)
        {
            this.Logger.Error(exception, "Error occurred while executing an operation on the inner service");
        }
    }
}