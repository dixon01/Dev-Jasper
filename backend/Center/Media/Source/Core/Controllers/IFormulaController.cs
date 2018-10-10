// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFormulaController.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The IFormulaController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// the formula controller interface
    /// </summary>
    public interface IFormulaController
    {
        /// <summary>
        /// Parses a formula string to an <see cref="EvalDataViewModelBase"/> object
        /// </summary>
        /// <param name="formula">
        /// The formula string.
        /// </param>
        /// <returns>
        /// The generated <see cref="EvalDataViewModelBase"/> object.
        /// </returns>
        EvalDataViewModelBase ParseFormula(string formula);
    }
}