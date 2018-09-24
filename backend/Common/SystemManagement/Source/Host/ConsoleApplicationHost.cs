// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleApplicationHost.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    /// <summary>
    /// Application host for console applications.
    /// This host allows to quit it through pressing "q" in the console.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the application to be run in this host.
    /// </typeparam>
    public partial class ConsoleApplicationHost<T> : ApplicationHost<T>
        where T : IRunnableApplication, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleApplicationHost{T}"/> class.
        /// </summary>
        /// <param name="arguments">
        /// The command line arguments.
        /// </param>
        public ConsoleApplicationHost(params string[] arguments)
            : base(arguments)
        {
        }
    }
}
