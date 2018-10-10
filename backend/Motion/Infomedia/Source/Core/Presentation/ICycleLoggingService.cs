// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICycleLoggingService.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICycleLoggingService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;
    using Gorba.Motion.Infomedia.Core.Presentation.Package;

    /// <summary>
    /// Service interface that provides the possibility to log cycle changes.
    /// This interface will only be called from <see cref="PackagePresentationEngine"/>,
    /// not from the <see cref="MasterPresentationEngine"/>.
    /// </summary>
    public interface ICycleLoggingService
    {
        /// <summary>
        /// Method that is called whenever the current page changes.
        /// </summary>
        /// <param name="currentCycle">
        /// The current cycle being shown.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        void PageChanged(CycleBase<Page> currentCycle, IPresentationContext context);
    }
}