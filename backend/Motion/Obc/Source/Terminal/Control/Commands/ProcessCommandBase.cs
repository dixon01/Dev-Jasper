// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessCommandBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessCommandBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;

    /// <summary>
    /// Base class for all commands processing an input alphabet value.
    /// </summary>
    internal abstract class ProcessCommandBase : Command
    {
        private readonly InputAlphabet input;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCommandBase"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        protected ProcessCommandBase(InputAlphabet input)
            : base(input.ToString())
        {
            this.input = input;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="sourceIsMenu">
        /// A flag indicating if the source is the menu.
        /// </param>
        public override void Execute(IContext context, bool sourceIsMenu)
        {
            context.Process(this.input);
        }
    }
}
