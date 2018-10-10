// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductTypeValidator.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserValidator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Units
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// Specific implementation of <see cref="ProductTypeValidator"/>.
    /// </summary>
    public partial class ProductTypeValidator
    {
        private static readonly string HardwareDescriptorType = XmlData.GetTypeName(typeof(HardwareDescriptor));

        partial void ValidateName(ProductTypeDataViewModel dvm)
        {
            dvm.ChangeError("Name", AdminStrings.Errors_TextNotWhitespace, string.IsNullOrEmpty(dvm.Name));
            dvm.ChangeError(
                "Name",
                AdminStrings.Errors_DuplicateName,
                this.DataController.ProductType.All.Any(u => u.Id != dvm.Id && u.Name.Equals(dvm.Name)));
        }

        partial void ValidateHardwareDescriptor(ProductTypeDataViewModel dvm)
        {
            dvm.ChangeError(
                "HardwareDescriptor",
                AdminStrings.Errors_TextNotWhitespace,
                string.IsNullOrEmpty(dvm.HardwareDescriptor.Xml));

            try
            {
                dvm.HardwareDescriptor.Type = HardwareDescriptorType;
                dvm.HardwareDescriptor.XmlData.Deserialize();
            }
            catch (Exception ex)
            {
                var exception = ex;
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }

                dvm.AddError("HardwareDescriptor", AdminStrings.Errors_ValidXml + ": " + exception.Message);
            }
        }
    }
}