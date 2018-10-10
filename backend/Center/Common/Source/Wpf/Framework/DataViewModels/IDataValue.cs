// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataValue.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;

    /// <summary>
    /// The DataValue interface.
    /// </summary>
    public interface IDataValue : ICloneable
    {
        /// <summary>
        /// Gets or sets the value as "object".
        /// </summary>
        object ValueObject { get; set; }
    }
}
