// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Common type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Views.Components
{
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.Startup;

    /// <summary>
    /// Defines the dictionary with common components.
    /// </summary>
    [Export(typeof(IResourceDictionary))]
    public partial class Common : IResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Common"/> class.
        /// </summary>
        public Common()
        {
            this.InitializeComponent();
        }
    }
}
