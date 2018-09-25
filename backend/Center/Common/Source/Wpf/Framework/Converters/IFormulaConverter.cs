// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormulaConverter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The FormulaConverter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Converters
{
    using System.Windows.Data;

    /// <summary>
    /// The FormulaConverter interface.
    /// </summary>
    public interface IFormulaConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the unchanged value.
        /// </summary>
        object UnchangedValue { get; set; }
    }
}
