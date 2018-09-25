// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterCycleManager.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterCycleManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master
{
    using System.Diagnostics;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    using NLog;

    /// <summary>
    /// The master cycle manager.
    /// </summary>
    /// <seealso cref="MasterPresentationConfig"/>
    public class MasterCycleManager : CycleManagerBase<MasterLayout>
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MasterCycleManager>();
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterCycleManager"/> class.
        /// </summary>
        /// <param name="screen">
        /// The physical screen for which this cycle manager is responsible.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public MasterCycleManager(PhysicalScreenConfig screen, IPresentationContext context)
            : base(context)
        {
            Logger.Debug("Enter PhysicalScreenConfig={0}", screen.Name);
            foreach (var cycleConfig in context.Config.Config.MasterPresentation.MasterCycles)
            {
                var cycle = new MasterCycle(cycleConfig, screen, context);
                this.AddStandardCycle(cycle);
            }

            foreach (var cycleConfig in context.Config.Config.MasterPresentation.MasterEventCycles)
            {
                var cycle = new MasterEventCycle(cycleConfig, screen, context);
                this.AddEventCycle(cycle);
            }
        }
    }
}