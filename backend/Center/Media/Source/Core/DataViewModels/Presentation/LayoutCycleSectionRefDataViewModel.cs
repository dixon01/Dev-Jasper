// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutCycleSectionRefDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// Defines a cycle and section reference where a layout is used.
    /// </summary>
    public class LayoutCycleSectionRefDataViewModel : DataViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutCycleSectionRefDataViewModel"/> class.
        /// </summary>
        /// <param name="cycleReference">
        /// The cycle reference.
        /// </param>
        /// <param name="sectionReference">
        /// The section reference.
        /// </param>
        public LayoutCycleSectionRefDataViewModel(
            CycleConfigDataViewModelBase cycleReference, SectionConfigDataViewModelBase sectionReference)
        {
            this.CycleReference = cycleReference;
            this.SectionReference = sectionReference;
        }

        /// <summary>
        /// Gets the cycle reference.
        /// </summary>
        public CycleConfigDataViewModelBase CycleReference { get; private set; }

        /// <summary>
        /// Gets the section reference.
        /// </summary>
        public SectionConfigDataViewModelBase SectionReference { get; private set; }
    }
}
