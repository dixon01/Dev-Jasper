// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemPartController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Vdv301
{
    using Gorba.Common.Configuration.Protran.VDV301;

    /// <summary>
    /// The part controller for a regular data item part.
    /// </summary>
    public class DataItemPartController : DataItemPartControllerBase<DataItemConfig>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemPartController"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to the data item.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public DataItemPartController(string[] path, CategoryControllerBase parent)
            : base(path, parent)
        {
        }
    }
}