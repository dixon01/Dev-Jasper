// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterSectionFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterSectionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Master
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// Factory for <see cref="MasterSection"/>.
    /// </summary>
    internal class MasterSectionFactory : ISectionFactory<MasterLayout>
    {
        private readonly PhysicalScreenConfig screen;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterSectionFactory"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        public MasterSectionFactory(PhysicalScreenConfig screen)
        {
            this.screen = screen;
        }

        /// <summary>
        /// Creates a new <see cref="SectionBase{T}"/> implementation for the given
        /// configuration.
        /// </summary>
        /// <param name="sectionConfig">
        /// The section configuration.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The new <see cref="SectionBase{T}"/> implementation.
        /// </returns>
        public SectionBase<MasterLayout> Create(SectionConfigBase sectionConfig, IPresentationContext context)
        {
            var standard = sectionConfig as MasterSectionConfig;
            if (standard != null)
            {
                return new MasterSection(standard, this.screen, context);
            }

            throw new NotSupportedException("Unknown master section type: " + sectionConfig.GetType().Name);
        }
    }
}