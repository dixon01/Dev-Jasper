// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventCycle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;

    /// <summary>
    /// Class that represents an event cycle.
    /// This cycle is only valid when being triggered (on a positive flank only).
    /// </summary>
    public class EventCycle : EventCycleBase<Page>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventCycle"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public EventCycle(EventCycleConfig config, PoolManager poolManager, IPresentationContext context)
            : base(config, context, new PackageSectionFactory(poolManager))
        {
        }
    }
}
