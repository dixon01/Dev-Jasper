// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePartValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdatePartValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Common;

    /// <summary>
    /// Specific implementation of <see cref="UpdatePartValidator"/>.
    /// </summary>
    public partial class UpdatePartValidator
    {
        private static readonly string StructureType = XmlData.GetTypeName(typeof(UpdateFolderStructure));

        partial void ValidateStructure(UpdatePartDataViewModel dvm)
        {
            dvm.ChangeError(
                "Structure",
                AdminStrings.Errors_TextNotWhitespace,
                string.IsNullOrEmpty(dvm.Structure.Xml));

            try
            {
                dvm.Structure.Type = StructureType;
                dvm.Structure.XmlData.Deserialize();
            }
            catch (Exception ex)
            {
                var exception = ex;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                dvm.AddError("Structure", AdminStrings.Errors_ValidXml + ": " + exception.Message);
            }
        }

        partial void ValidateStart(UpdatePartDataViewModel dvm)
        {
            dvm.ChangeError("Start", AdminStrings.Errors_ValidDateTime, dvm.Start == default(DateTime));
        }

        partial void ValidateEnd(UpdatePartDataViewModel dvm)
        {
            dvm.ChangeError("End", AdminStrings.Errors_ValidDateTime, dvm.End == default(DateTime));
        }

        partial void ValidateUpdateGroup(UpdatePartDataViewModel dvm)
        {
            dvm.ChangeError("UpdateGroup", AdminStrings.Errors_NoItemSelected, dvm.UpdateGroup.SelectedEntity == null);
        }
    }
}