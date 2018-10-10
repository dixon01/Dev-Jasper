// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumberInputScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the NumberInputScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The number input screen.
    /// </summary>
    internal abstract class NumberInputScreen : NumberInputBaseScreen<INumberInput>
    {
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberInputScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="maxLength">
        /// The max length.
        /// </param>
        protected NumberInputScreen(INumberInput mainField, IContext context, int maxLength)
            : base(mainField, context)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Initializes the main field.
        /// </summary>
        protected override void InitMainField()
        {
            this.MainField.Init(this.GetMainCaption(), this.GetTextInputCaption(), this.maxLength);
        }
    }
}