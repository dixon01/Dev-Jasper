// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardSection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StandardSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// Default section of a cycle (non-paged).
    /// </summary>
    public class StandardSection : SinglePageSectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardSection"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        internal StandardSection(StandardSectionConfig config, IPresentationContext context)
            : base(config, context)
        {
        }

        /// <summary>
        /// Create the next single page to be shown by this section.
        /// </summary>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        protected override Page CreateNextPage()
        {
            var layout = this.Context.Config.Config.Layouts.Find(c => c.Name == this.Config.Layout);
            if (layout == null)
            {
                return null;
            }

            return new Page(new StandardLayout(layout, this.Context.Config.Config), this.Config.Duration);
        }
    }
}