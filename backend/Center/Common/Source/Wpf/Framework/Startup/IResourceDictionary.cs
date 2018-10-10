// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceDictionary.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceDictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Markup;

    /// <summary>
    /// Defines a resource dictionary.
    /// </summary>
    public interface IResourceDictionary : IDictionary, INameScope, IUriContext, ISupportInitialize
    {
    }
}