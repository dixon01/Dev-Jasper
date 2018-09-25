// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    /// <summary>
    /// Interface to be implemented by an application that wants to run as a component in System Manager.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Configures this application with the given name.
        /// The application should use the given name to register itself with the System Manager Client.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        void Configure(string name);

        /// <summary>
        /// Starts this application.
        /// The application should update its state using the registration gotten by <see cref="Configure"/>.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this application.
        /// The application should update its state using the registration gotten by <see cref="Configure"/>.
        /// </summary>
        void Stop();
    }
}
