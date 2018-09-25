// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterCycle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master
{
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The master (standards) cycle.
    /// </summary>
    public class MasterCycle : CycleBase<MasterLayout>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterCycle"/> class.
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
        public MasterCycle(MasterCycleConfig cycleConfig, PhysicalScreenConfig screen, IPresentationContext context)
            : base(cycleConfig, context, new MasterSectionFactory(screen))
        {
        }
    }
}