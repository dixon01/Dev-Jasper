// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SinglePageSectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SinglePageSectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;

    /// <summary>
    /// Base class for sections that show a single page at the time (most sections are like that).
    /// </summary>
    public abstract class SinglePageSectionBase : PackageSectionBase
    {
        private bool showingPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SinglePageSectionBase"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected SinglePageSectionBase(SectionConfigBase config, IPresentationContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Deactivates this cycle step.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
            this.showingPage = false;
        }

        /// <summary>
        /// Create the next single page to be shown by this section.
        /// </summary>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        protected abstract Page CreateNextPage();

        /// <summary>
        /// Finds the next available page.
        /// </summary>
        /// <returns>
        /// The page to be shown or null if no page can be shown and the cycle should
        /// switch to the next section.
        /// </returns>
        protected override Page FindNextObject()
        {
            if (this.showingPage)
            {
                this.showingPage = false;
                return null;
            }

            var page = this.CreateNextPage();
            this.showingPage = page != null;
            return page;
        }
    }
}