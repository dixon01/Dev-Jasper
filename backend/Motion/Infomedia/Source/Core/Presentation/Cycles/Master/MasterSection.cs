// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterSection.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master
{
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The master section.
    /// </summary>
    public class MasterSection : SectionBase<MasterLayout>
    {
        private readonly PhysicalScreenConfig screen;

        private bool active;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSection"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The section config.
        /// </param>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public MasterSection(
            MasterSectionConfig sectionConfig, PhysicalScreenConfig screen, IPresentationContext context)
            : base(sectionConfig, context)
        {
            this.screen = screen;
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
            return this.CurrentObject;
        }

        /// <summary>
        /// Deactivates this cycle step.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
            this.active = false;
        }

        /// <summary>
        /// Finds the next available physical screen reference in a master layout.
        /// </summary>
        /// <returns>
        /// The physical screen reference to be shown or null if nothing can be
        /// shown and the cycle should switch to the next section.
        /// </returns>
        protected override MasterLayout FindNextObject()
        {
            var screenRef = this.FindNextLayout();
            this.active = screenRef != null;
            return screenRef;
        }

        private MasterLayout FindNextLayout()
        {
            if (this.active)
            {
                return null;
            }

            var layout =
                this.Context.Config.Config.MasterPresentation.MasterLayouts.Find(c => c.Name == this.Config.Layout);
            if (layout == null)
            {
                return null;
            }

            return new MasterLayout(layout, this.screen);
        }
    }
}