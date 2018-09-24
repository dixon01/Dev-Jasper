// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NullCycle type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// <see cref="CycleBase{T}"/> implementation that represents an invalid
    /// cycle. This class is used to allow no cycle / section to be shown in
    /// case no valid section or cycle is found.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item inside this cycle's sections. To be defined by subclasses.
    /// </typeparam>
    internal class NullCycle<T> : CycleBase<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullCycle{T}"/> class.
        /// </summary>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public NullCycle(IPresentationContext context)
            : base(
                new StandardCycleConfig
                    {
                        Name = "<null>",
                        Sections = { new StandardSectionConfig { Duration = TimeSpan.FromDays(1) } }
                    },
                context,
                new SectionFactory())
        {
        }

        /// <summary>
        /// The type of section this cycle contains.
        /// </summary>
        public class Section : SectionBase<T>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Section"/> class.
            /// </summary>
            /// <param name="config">
            /// The config.
            /// </param>
            /// <param name="context">
            /// The context.
            /// </param>
            public Section(SectionConfigBase config, IPresentationContext context)
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
                return null;
            }

            /// <summary>
            /// Always returns true.
            /// </summary>
            /// <returns>
            /// always true.
            /// </returns>
            public override bool ShowNextObject()
            {
                return true;
            }

            /// <summary>
            /// Finds the next available page.
            /// This method has to be implemented by different types of sections.
            /// </summary>
            /// <returns>
            /// The page to be shown or null if no page can be shown and the cycle should
            /// switch to the next section.
            /// </returns>
            protected override T FindNextObject()
            {
                return null;
            }
        }

        private class SectionFactory : ISectionFactory<T>
        {
            public SectionBase<T> Create(SectionConfigBase config, IPresentationContext context)
            {
                return new Section(config, context);
            }
        }
    }
}