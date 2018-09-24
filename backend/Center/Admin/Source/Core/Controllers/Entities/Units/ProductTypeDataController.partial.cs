// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductTypeDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductTypeDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Units
{
    using Gorba.Center.Admin.Core.DataViewModels.Units;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// Partial implementation of the special behavior of the <see cref="ProductTypeDataController"/>.
    /// </summary>
    public partial class ProductTypeDataController
    {
        partial void PostCreateEntity(ProductTypeDataViewModel dataViewModel)
        {
            dataViewModel.HardwareDescriptor.XmlData = new XmlData(new HardwareDescriptor());
        }
    }
}