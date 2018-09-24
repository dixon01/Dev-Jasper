// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageSectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// Base class for all sections in the <see cref="PackageCycleManager"/>.
    /// </summary>
    public abstract class PackageSectionBase : SectionBase<Page>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageSectionBase"/> class.
        /// </summary>
        /// <param name="config">
        /// The section config.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        protected PackageSectionBase(SectionConfigBase config, IPresentationContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Gets the <see cref="LayoutBase"/> implementation of the currently
        /// valid layout (or null) from the this section.
        /// </summary>
        /// <returns>
        /// The <see cref="LayoutBase"/> or null if no layout can be shown.
        /// </returns>
        public override LayoutBase GetLayout()
        {
            return this.CurrentObject == null ? null : this.CurrentObject.Layout;
        }

        /// <summary>
        /// Gets the duration for this section.
        /// </summary>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        protected internal override TimeSpan GetDuration()
        {
            if (this.CurrentObject == null || this.CurrentObject.Duration <= TimeSpan.Zero)
            {
                this.Logger.Warn("Page has no duration, using default value (10s).");
                return TimeSpan.FromSeconds(10);
            }

            return this.CurrentObject.Duration;
        }
    }
}