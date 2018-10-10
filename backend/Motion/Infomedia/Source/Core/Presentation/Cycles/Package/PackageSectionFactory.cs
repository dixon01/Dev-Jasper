// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSectionFactory.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageSectionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation.Section;

    /// <summary>
    /// Factory for all section objects that are possible in a 
    /// cycle package.
    /// </summary>
    internal class PackageSectionFactory : ISectionFactory<Page>
    {
        private readonly PoolManager poolManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageSectionFactory"/> class.
        /// </summary>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        public PackageSectionFactory(PoolManager poolManager)
        {
            this.poolManager = poolManager;
        }

        /// <summary>
        /// Creates a new <see cref="SectionBase{T}"/> implementation for the given
        /// configuration.
        /// </summary>
        /// <param name="config">
        /// The section configuration.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The new <see cref="SectionBase{T}"/> implementation.
        /// </returns>
        public SectionBase<Page> Create(SectionConfigBase config, IPresentationContext context)
        {
            var standard = config as StandardSectionConfig;
            if (standard != null)
            {
                return new StandardSection(standard, context);
            }

            var multi = config as MultiSectionConfig;
            if (multi != null)
            {
                return new MultiSection(multi, this.poolManager, context);
            }

            var webmedia = config as WebmediaSectionConfig;
            if (webmedia != null)
            {
                return new WebmediaSection(webmedia, this.poolManager, context);
            }

            var image = config as ImageSectionConfig;
            if (image != null)
            {
                return new ImageSection(image, context);
            }

            var video = config as VideoSectionConfig;
            if (video != null)
            {
                return new VideoSection(video, context);
            }

            var pool = config as PoolSectionConfig;
            if (pool != null)
            {
                return new PoolSection(pool, this.poolManager, context);
            }

            throw new NotSupportedException("Unknown package section type: " + config.GetType().Name);
        }
    }
}