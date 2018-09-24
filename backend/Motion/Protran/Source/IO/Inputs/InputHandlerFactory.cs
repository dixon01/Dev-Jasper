// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputHandlerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Inputs
{
    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Factory class that creates an input handler for a given configuration.
    /// </summary>
    public class InputHandlerFactory
    {
        private static readonly Logger Logger = LogHelper.GetLogger<InputHandlerFactory>();

        /// <summary>
        /// this creates an input handler based on the configuration.
        /// </summary>
        /// <param name="input">
        /// The input handling config.
        /// </param>
        /// <param name="transformationManager">
        /// The transformation manager.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="InputHandler"/>.
        /// </returns>
        public virtual IInputHandler CreateInputHandler(
            InputHandlingConfig input, TransformationManager transformationManager, Dictionary dictionary)
        {
            if (!input.Enabled)
                {
                    return null;
                }

                var chain = transformationManager.GetChain(input.TransfRef);
                if (chain == null)
                {
                    Logger.Warn("Unknown transformation reference: {0}", input.TransfRef);
                    return null;
                }

                var inputHandler = new InputHandler(input, chain, dictionary);
                return inputHandler;
        }
    }
}
