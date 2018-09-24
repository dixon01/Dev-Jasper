// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StandardCycle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;

    /// <summary>
    /// Class that represents a regular (a.k.a. criteria) cycle.
    /// </summary>
    public class StandardCycle : CycleBase<Page>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardCycle"/> class.
        /// </summary>
        /// <param name="config">
        /// The cycle config.
        /// </param>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public StandardCycle(StandardCycleConfig config, PoolManager poolManager, IPresentationContext context)
            : base(config, context, new PackageSectionFactory(poolManager))
        {
        }
    }
}