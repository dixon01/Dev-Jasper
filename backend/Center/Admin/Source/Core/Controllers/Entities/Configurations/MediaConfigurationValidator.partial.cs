// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaConfigurationValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaConfigurationValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Configurations
{
    using Gorba.Center.Admin.Core.DataViewModels.Configurations;
    using Gorba.Center.Admin.Core.Resources;

    /// <summary>
    /// Specific implementation of <see cref="MediaConfigurationValidator"/>.
    /// </summary>
    public partial class MediaConfigurationValidator
    {
        partial void ValidateDocument(MediaConfigurationDataViewModel dvm)
        {
            dvm.ChangeError("Document", AdminStrings.Errors_NoItemSelected, dvm.Document.SelectedEntity == null);
        }
    }
}