// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionReadOnlyDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentVersionReadOnlyDataViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels.Documents
{
    /// <summary>
    /// Partial implementation of <see cref="DocumentVersionReadOnlyDataViewModel"/>.
    /// </summary>
    public partial class DocumentVersionReadOnlyDataViewModel
    {
        // ReSharper disable once RedundantAssignment
        partial void GetDisplayText(ref string displayText)
        {
            displayText = string.Format("{0}.{1} ({2})", this.Major, this.Minor, this.Description);
        }
    }
}
