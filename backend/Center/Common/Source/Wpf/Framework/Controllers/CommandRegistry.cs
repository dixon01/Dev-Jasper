// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandRegistry.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandRegistry type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The Command Registry
    /// </summary>
    [Export(typeof(ICommandRegistry))]
    public class CommandRegistry : ICommandRegistry
    {
        private readonly Dictionary<string, ICommand> commandDictionary = new Dictionary<string, ICommand>();

        /// <summary>
        /// registers a command
        /// </summary>
        /// <param name="key">the key to identify the command</param>
        /// <param name="command">the command</param>
        public void RegisterCommand(string key, ICommand command)
        {
            this.commandDictionary[key] = command;
        }

        /// <summary>
        /// returns the command for the given key or a command that does nothing
        /// </summary>
        /// <param name="key">the name of the command</param>
        /// <returns>the command</returns>
        public virtual ICommand GetCommand(string key)
        {
            ICommand result;

            if (this.commandDictionary.ContainsKey(key))
            {
                result = this.commandDictionary[key];
            }
            else
            {
                result = new RelayCommand(() => { });
            }

            return result;
        }
    }
}
