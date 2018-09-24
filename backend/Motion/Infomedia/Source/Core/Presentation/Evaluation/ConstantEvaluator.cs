// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstantEvaluator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConstantEvaluator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Evaluator for <see cref="ConstantEval"/> that just has the configured constant value.
    /// </summary>
    public partial class ConstantEvaluator
    {
        private string stringValue;
        private int? intValue;
        private bool? boolValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantEvaluator"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public ConstantEvaluator(object value)
            : base(null, null)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the string representation of the <see cref="EvaluatorBase.Value"/>.
        /// </summary>
        public override string StringValue
        {
            get
            {
                return this.stringValue ?? (this.stringValue = base.StringValue);
            }
        }

        /// <summary>
        /// Gets the integer value or 0 if the <see cref="EvaluatorBase.Value"/> can't be converted.
        /// </summary>
        public override int IntValue
        {
            get
            {
                return this.intValue ?? (int)(this.intValue = base.IntValue);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value is true.
        /// This tries to convert the <see cref="EvaluatorBase.Value"/> to a boolean,
        /// either directly or converting it to an integer first.
        /// </summary>
        public override bool BoolValue
        {
            get
            {
                return this.boolValue ?? (bool)(this.boolValue = base.BoolValue);
            }
        }

        partial void UpdateValue()
        {
            this.Value = this.ConstantEval.Value;
        }
    }
}