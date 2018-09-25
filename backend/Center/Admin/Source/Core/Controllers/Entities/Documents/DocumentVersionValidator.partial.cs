// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DocumentVersionValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Documents
{
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Documents;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="DocumentVersionValidator"/>.
    /// </summary>
    public partial class DocumentVersionValidator
    {
        partial void ValidateDocument(DocumentVersionDataViewModel dvm)
        {
            dvm.ChangeError("Document", AdminStrings.Errors_NoItemSelected, dvm.Document.SelectedEntity == null);
        }

        partial void ValidateContent(DocumentVersionDataViewModel dvm)
        {
            dvm.ChangeError(
                "Content",
                AdminStrings.Errors_TextNotWhitespace + " (XML)",
                string.IsNullOrEmpty(dvm.Content.Xml));
            dvm.ChangeError(
                "Content",
                AdminStrings.Errors_TextNotWhitespace + " (Type)",
                string.IsNullOrEmpty(dvm.Content.Type));
        }

        partial void ValidateMajor(DocumentVersionDataViewModel dvm)
        {
            this.ValidateVersionNumber(dvm);
        }

        partial void ValidateMinor(DocumentVersionDataViewModel dvm)
        {
            this.ValidateVersionNumber(dvm);
        }

        private void ValidateVersionNumber(DocumentVersionDataViewModel dvm)
        {
            var hasError = this.DataController.DocumentVersion.All.Any(
                d =>
                d.Id != dvm.Id && d.Major == dvm.Major && d.Minor == dvm.Minor && dvm.Document.SelectedEntity != null
                && d.Document.Id == dvm.Document.SelectedEntity.Id);
            dvm.ChangeError("Minor", AdminStrings.Errors_DuplicateValue, hasError);
            dvm.ChangeError("Major", AdminStrings.Errors_DuplicateValue, hasError);
        }
    }
}