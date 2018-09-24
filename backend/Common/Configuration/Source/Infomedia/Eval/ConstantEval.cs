// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantEval.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConstantEval type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Eval
{
    /// <summary>
    /// Evaluation configuration that has a constant <see cref="Value"/>.
    /// </summary>
    public partial class ConstantEval
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantEval"/> class.
        /// </summary>
        /// <param name="value">
        /// The <see cref="Value"/>.
        /// </param>
        public ConstantEval(string value)
        {
            this.Value = value;
        }
    }
}