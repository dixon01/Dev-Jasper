// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperResult{TController,TState}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BootstrapperResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Defines the result of the <see cref="BootstrapperBase.BootstrapCore{TController,TState}"/> operation.
    /// </summary>
    /// <typeparam name="TController">The type of the application controller.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public class BootstrapperResult<TController, TState>
        where TController : IApplicationController
        where TState : IApplicationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperResult&lt;TController, TState&gt;"/> class.
        /// </summary>
        public BootstrapperResult()
        {
            this.ResourceDictionaries = new List<IResourceDictionary>();
        }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationController"/> of type <typeparamref name="TController"/>.
        /// </summary>
        /// <value>
        /// The application controller.
        /// </value>
        public TController Controller { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public TState State { get; set; }

        /// <summary>
        /// Gets or sets the resource dictionaries associated with the application.
        /// </summary>
        /// <value>
        /// The resource dictionaries.
        /// </value>
        public ICollection<IResourceDictionary> ResourceDictionaries { get; set; }
    }
}