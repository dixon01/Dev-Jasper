// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChildItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the contract for an object that has a parent object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.T4Templating.Utility
{
    /// <summary>
    /// Defines the contract for an object that has a parent object
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    /// <remarks>
    /// <see cref="http://www.thomaslevesque.com/2009/06/12/c-parentchild-relationship-and-xml-serialization/"/>
    /// </remarks>
    public interface IChildItem<TParent>
        where TParent : class
    {
        /// <summary>
        /// Gets or sets the parent item.
        /// </summary>
        TParent Parent { get; set; }
    }
}