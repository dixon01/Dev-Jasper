// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FormatEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using System;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Evaluator for <see cref="FormatEval"/>.
    /// </summary>
    public partial class FormatEvaluator
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FormatEvaluator>();

        partial void UpdateValue()
        {
            var args = this.EvalsArguments.ConvertAll(e => e.Value);
            try
            {
                this.Value = string.Format(this.FormatEval.Format, args.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't format: " + this.FormatEval.Format);
                this.Value = string.Empty;
            }
        }
    }
}