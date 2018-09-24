// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// The or evaluation data view model.
    /// </summary>
    public partial class OrEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var operands = string.Empty;
            if (this.Conditions != null)
            {
                operands = string.Join("; ", this.Conditions.Select(o => o != null ? o.HumanReadable() : string.Empty));
            }

            return "Or ( " + operands + " )";
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected override IEnumerable<string> Validate(string propertyName)
        {
            return this.Validate(propertyName, MediaStrings.FormulaEditor_Eval_Or);
        }
    }
}
