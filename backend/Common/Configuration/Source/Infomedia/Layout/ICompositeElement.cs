// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositeElement.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICompositeElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface to be implemented by all elements that contain child elements.
    /// </summary>
    public interface ICompositeElement : IEnumerable<ElementBase>
    {
    }
}