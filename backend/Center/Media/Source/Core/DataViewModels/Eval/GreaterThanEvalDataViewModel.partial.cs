// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreaterThanEvalDataViewModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// Extends the generated model class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    /// <summary>
    /// The greater than evaluation data view model.
    /// </summary>
    public partial class GreaterThanEvalDataViewModel
    {
        /// <summary>
        /// creates a string representation of the model
        /// </summary>
        /// <returns> A string representation</returns>
        public override string HumanReadable()
        {
            var valueLeft = string.Empty;
            if (this.Left != null)
            {
                valueLeft = this.Left.HumanReadable();
            }

            var valueRight = string.Empty;
            if (this.Right != null)
            {
                valueRight = this.Right.HumanReadable();
            }

            return valueLeft + " > " + valueRight;
        }
    }
}
