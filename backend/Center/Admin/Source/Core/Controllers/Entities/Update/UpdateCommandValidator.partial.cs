// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.ServiceModel;

    using UpdateCommandMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;

    /// <summary>
    /// Specific implementation of <see cref="UpdateCommandValidator"/>.
    /// </summary>
    public partial class UpdateCommandValidator
    {
        private static readonly string CommandType = XmlData.GetTypeName(typeof(UpdateCommandMsg));

        partial void ValidateUnit(UpdateCommandDataViewModel dvm)
        {
            dvm.ChangeError("Unit", AdminStrings.Errors_NoItemSelected, dvm.Unit.SelectedEntity == null);
        }

        partial void ValidateUpdateIndex(UpdateCommandDataViewModel dvm)
        {
            dvm.ChangeError(
                "UpdateIndex",
                AdminStrings.Errors_DuplicateValue,
                this.DataController.UpdateCommand.All.Any(
                    d => d.Id != dvm.Id
                        && d.UpdateIndex == dvm.UpdateIndex
                        && dvm.Unit.SelectedEntity != null
                        && d.Unit.Id == dvm.Unit.SelectedEntity.Id));
        }

        partial void ValidateCommand(UpdateCommandDataViewModel dvm)
        {
            dvm.ChangeError("Command", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Command.Xml));

            try
            {
                dvm.Command.Type = CommandType;
                dvm.Command.XmlData.Deserialize();
            }
            catch (Exception ex)
            {
                var exception = ex;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                dvm.AddError("Command", AdminStrings.Errors_ValidXml + ": " + exception.Message);
            }
        }
    }
}