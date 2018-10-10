// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicDataValue.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DynamicDataValue interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    /// <summary>
    /// The DynamicDataValue interface.
    /// </summary>
    public interface IDynamicDataValue : IDataValue
    {
        /// <summary>
        /// Gets or sets the evaluation of the dynamic property.
        /// </summary>
        FormulaDataViewModelBase Formula { get; set; }

        /// <summary>
        /// The restore formula references.
        /// </summary>
        void RestoreFormulaReferences();

        /// <summary>
        /// The raise formula changed.
        /// </summary>
        void RaiseFormulaChanged();
    }
}
