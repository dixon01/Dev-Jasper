// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateSectionParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The parameters needed to create a section.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;

    /// <summary>
    /// The parameters needed to create a section.
    /// </summary>
    public class CreateSectionParameters
    {
        /// <summary>
        /// Gets or sets the section type.
        /// </summary>
        public SectionType SectionType { get; set; }

        /// <summary>
        /// Gets or sets the layout to be assigned.
        /// </summary>
        public LayoutConfigDataViewModel Layout { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string Format = "[CreateSectionParameters Type: {0}, Layout: {1}]";
            return string.Format(Format, this.SectionType, this.Layout.Name.Value);
        }
    }
}
