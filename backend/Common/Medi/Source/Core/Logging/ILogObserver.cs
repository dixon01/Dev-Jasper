// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILogObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Logging
{
    using System;

    using NLog;

    /// <summary>
    /// Log observer base class that can be used to get local
    /// and remote log events.
    /// Only use this class after calling <see cref="MessageDispatcher.Configure"/>!
    /// </summary>
    public interface ILogObserver
    {
        /// <summary>
        /// Event that is risen whenever a log message has arrived.
        /// </summary>
        event EventHandler<LogEventArgs> MessageLogged;

        /// <summary>
        /// Gets or sets the minimum level of this observer.
        /// </summary>
        LogLevel MinLevel { get; set; }
    }
}
