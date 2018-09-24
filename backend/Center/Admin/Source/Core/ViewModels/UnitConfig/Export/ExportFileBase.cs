// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportFileBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportFileBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System.Collections.Generic;

    /// <summary>
    /// The base class for all files to be exported.
    /// </summary>
    public abstract class ExportFileBase : ExportItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportFileBase"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        protected ExportFileBase(string fileName)
        {
            this.Name = fileName;
        }

        /// <summary>
        /// Gets the children, this is always an empty list.
        /// </summary>
        public override IEnumerable<ExportItemBase> ChildItems
        {
            get
            {
                return new ExportItemBase[0];
            }
        }
    }
}