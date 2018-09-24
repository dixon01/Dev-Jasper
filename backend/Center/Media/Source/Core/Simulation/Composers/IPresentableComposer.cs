// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentableComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentableComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Interface to be implemented by all composers that can be presented (i.e. have a related screen item).
    /// </summary>
    public interface IPresentableComposer : IComposer
    {
        /// <summary>
        /// Gets the screen item of this composer.
        /// </summary>
        ScreenItemBase Item { get; }
    }
}