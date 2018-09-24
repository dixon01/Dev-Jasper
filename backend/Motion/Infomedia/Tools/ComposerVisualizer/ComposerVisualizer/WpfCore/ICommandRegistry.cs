// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandRegistry.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.WpfCore
{
    using System.Windows.Input;

    /// <summary>
    /// The command registry interface
    /// </summary>
    public interface ICommandRegistry
    {
        /// <summary>
        /// registers a command
        /// </summary>
        /// <param name="key">the key to identify the command</param>
        /// <param name="command">the command</param>
        void RegisterCommand(string key, ICommand command);

        /// <summary>
        /// returns the command for the given key or a command that does nothing
        /// </summary>
        /// <param name="key">the name of the command</param>
        /// <returns>the command</returns>
        ICommand GetCommand(string key);
    }
}
