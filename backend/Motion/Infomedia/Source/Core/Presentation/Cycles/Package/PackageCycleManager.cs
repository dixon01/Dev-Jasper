// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageCycleManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// The package cycle manager.
    /// </summary>
    /// <seealso cref="CyclePackageConfig"/>
    public class PackageCycleManager : CycleManagerBase<Page>
    {
        private readonly PoolManager poolManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageCycleManager"/> class.
        /// </summary>
        /// <param name="config">
        /// The entire presentation config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public PackageCycleManager(CyclePackageConfig config, IPresentationContext context)
            : base(context)
        {
            this.poolManager = new PoolManager(context);

            foreach (var cycleRef in config.StandardCycles)
            {
                var cycleConfig = context.Config.Config.Cycles.StandardCycles.Find(c => c.Name == cycleRef.Reference);
                if (cycleConfig == null)
                {
                    throw new KeyNotFoundException("Couldn't find standard cycle with name " + cycleRef.Reference);
                }

                var cycle = new StandardCycle(cycleConfig, this.poolManager, context);
                this.AddStandardCycle(cycle);
            }

            foreach (var cycleRef in config.EventCycles)
            {
                var cycleConfig = context.Config.Config.Cycles.EventCycles.Find(c => c.Name == cycleRef.Reference);
                if (cycleConfig == null)
                {
                    throw new KeyNotFoundException("Couldn't find event cycle with name " + cycleRef.Reference);
                }

                var cycle = new EventCycle(cycleConfig, this.poolManager, context);
                this.AddEventCycle(cycle);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.poolManager.Dispose();
        }
    }
}
