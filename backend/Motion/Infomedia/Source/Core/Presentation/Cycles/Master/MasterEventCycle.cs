// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterEventCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterEventCycle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master
{
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The master event cycle.
    /// </summary>
    public class MasterEventCycle : EventCycleBase<MasterLayout>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterEventCycle"/> class.
        /// </summary>
        /// <param name="cycleConfig">
        /// The cycle config.
        /// </param>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public MasterEventCycle(MasterEventCycleConfig cycleConfig, PhysicalScreenConfig screen, IPresentationContext context)
            : base(cycleConfig, context, new MasterSectionFactory(screen))
        {
        }
    }
}