// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Evaluator for <see cref="GenericEval"/> which updates itself
    /// automatically from the given <see cref="IPresentationContext"/>.
    /// </summary>
    public partial class GenericEvaluator
    {
        partial void Initialize()
        {
            var coordinate = this.GenericEval.ToCoordinate();
            this.Context.Generic.AddCellChangeHandler(coordinate, this.OnCellChange);
        }

        partial void Deinitialize()
        {
            var coordinate = this.GenericEval.ToCoordinate();
            this.Context.Generic.RemoveCellChangeHandler(coordinate, this.OnCellChange);
        }

        partial void UpdateValue()
        {
            var coordinate = this.GenericEval.ToCoordinate();
            this.Value = this.Context.Generic.GetGenericCellValue(coordinate) ?? string.Empty;
        }

        private void OnCellChange(XimpleCell cell)
        {
            this.Value = cell.Value;
        }
    }
}