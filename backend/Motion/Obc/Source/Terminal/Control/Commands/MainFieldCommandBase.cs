// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainFieldCommandBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainFieldCommandBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Screens;

    /// <summary>
    /// The base class for all commands activating a main field.
    /// </summary>
    internal abstract class MainFieldCommandBase : Command
    {
        private readonly MainFieldKey mainFieldKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFieldCommandBase"/> class.
        /// </summary>
        /// <param name="mainFieldKey">
        /// The main field key.
        /// </param>
        protected MainFieldCommandBase(MainFieldKey mainFieldKey)
        {
            this.mainFieldKey = mainFieldKey;
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
            context.ShowMainField(this.mainFieldKey);
        }
    }
}
