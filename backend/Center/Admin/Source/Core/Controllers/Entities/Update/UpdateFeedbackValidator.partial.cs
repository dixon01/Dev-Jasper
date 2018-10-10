// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateFeedbackValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateFeedbackValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Update
{
    using System;

    using Gorba.Center.Admin.Core.DataViewModels.Update;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// Specific implementation of <see cref="UpdateFeedbackValidator"/>.
    /// </summary>
    public partial class UpdateFeedbackValidator
    {
        private static readonly string FeedbackType = XmlData.GetTypeName(typeof(UpdateStateInfo));

        partial void ValidateFeedback(UpdateFeedbackDataViewModel dvm)
        {
            dvm.ChangeError(
                "Structure",
                AdminStrings.Errors_TextNotWhitespace,
                string.IsNullOrEmpty(dvm.Feedback.Xml));

            try
            {
                dvm.Feedback.Type = FeedbackType;
                dvm.Feedback.XmlData.Deserialize();
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

        partial void ValidateUpdateCommand(UpdateFeedbackDataViewModel dvm)
        {
            dvm.ChangeError(
                "UpdateCommand", AdminStrings.Errors_NoItemSelected, dvm.UpdateCommand.SelectedEntity == null);
        }

        partial void ValidateTimestamp(UpdateFeedbackDataViewModel dvm)
        {
            dvm.ChangeError("Timestamp", AdminStrings.Errors_ValidDateTime, dvm.Timestamp == default(DateTime));
        }
    }
}