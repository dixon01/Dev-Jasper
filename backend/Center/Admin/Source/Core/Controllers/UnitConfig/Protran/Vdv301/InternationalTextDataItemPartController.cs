// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternationalTextDataItemPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InternationalTextDataItemPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// The item part controller for a <see cref="InternationalTextDataItemConfig"/>.
    /// </summary>
    public class InternationalTextDataItemPartController : DataItemPartControllerBase<InternationalTextDataItemConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternationalTextDataItemPartController"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the data item.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public InternationalTextDataItemPartController(string[] path, CategoryControllerBase parent)
            : base(path, parent)
        {
            this.ShouldShowLanguage = false;
        }
    }
}