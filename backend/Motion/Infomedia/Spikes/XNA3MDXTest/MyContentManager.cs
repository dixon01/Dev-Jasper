// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyContentManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The my content manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using System;

    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// The my content manager.
    /// </summary>
    public class MyContentManager : ContentManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyContentManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        public MyContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyContentManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <param name="rootDirectory">
        /// The root directory.
        /// </param>
        public MyContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }
    }
}