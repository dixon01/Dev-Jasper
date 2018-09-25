// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceContainer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility.Container
{
    /// <summary>
    /// The Gorba service container implementation.
    /// </summary>
    public partial class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceContainer"/> class.
        /// </summary>
        public ServiceContainer()
        {
            this.Initialize();

            // register ourselves
            this.RegisterInstance(this);
            this.RegisterInstance<IServiceContainer>(this);
        }

        partial void Initialize();
    }
}