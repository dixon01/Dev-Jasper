// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISectionFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISectionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;

    /// <summary>
    /// Interface for a factory that creates <see cref="SectionBase{T}"/> implementations.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside the section. To be defined by subclasses.
    /// </typeparam>
    public interface ISectionFactory<T>
        where T : class
    {
        /// <summary>
        /// Creates a new <see cref="SectionBase{T}"/> implementation for the given
        /// configuration.
        /// </summary>
        /// <param name="config">
        /// The section configuration.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The new <see cref="SectionBase{T}"/> implementation.
        /// </returns>
        SectionBase<T> Create(SectionConfigBase config, IPresentationContext context);
    }
}
