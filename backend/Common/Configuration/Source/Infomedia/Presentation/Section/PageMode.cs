// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PageMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Section
{
    /// <summary>
    /// The mode to be used to show pages in a step.
    /// </summary>
    public enum PageMode
    {
        /// <summary>
        /// Show one page every time this step is shown.
        /// </summary>
        OnePage,

        /// <summary>
        /// Show all pages when this step is shown. Each
        /// page will be shown for the configured 
        /// <see cref="SectionConfigBase.DurationSeconds"/>.
        /// </summary>
        AllPages,

        /// <summary>
        /// Show one page every time this step is shown, but use the same
        /// pool of pages for all <see cref="MultiSectionConfig"/> referring
        /// to the same language and table.
        /// </summary>
        Pool
    }
}