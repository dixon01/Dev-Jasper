// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryOperation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The BinaryOperation.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    /// <summary>
    /// the binary operation
    /// </summary>
    public class BinaryOperation
    {
        static BinaryOperation()
        {
            GreaterThan = new BinaryOperation(">");
            GreaterThanOrEqual = new BinaryOperation(">=");
            LessThan = new BinaryOperation("<");
            LessThanOrEqual = new BinaryOperation("<=");
            IsEqual = new BinaryOperation("=");
            NotEquals = new BinaryOperation("<>");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperation"/> class.
        /// </summary>
        /// <param name="op">the operator</param>
        private BinaryOperation(string op)
        {
            this.Operator = op;
        }

        /// <summary>
        /// Gets the greater than binary operation
        /// </summary>
        public static BinaryOperation GreaterThan { get; private set; }

        /// <summary>
        /// Gets the greater than or equal binary operation
        /// </summary>
        public static BinaryOperation GreaterThanOrEqual { get; private set; }

        /// <summary>
        /// Gets the less than binary operation
        /// </summary>
        public static BinaryOperation LessThan { get; private set; }

        /// <summary>
        /// Gets the less than or equal binary operation
        /// </summary>
        public static BinaryOperation LessThanOrEqual { get; private set; }

        /// <summary>
        /// Gets the equals binary operation
        /// </summary>
        public static BinaryOperation IsEqual { get; private set; }

        /// <summary>
        /// Gets the not equals binary operation
        /// </summary>
        public static BinaryOperation NotEquals { get; private set; }

        /// <summary>
        /// Gets the operator
        /// </summary>
        public string Operator { get; private set; }
    }
}