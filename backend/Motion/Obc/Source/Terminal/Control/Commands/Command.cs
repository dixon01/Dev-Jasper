// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Command.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Command type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Commands
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Obc.Terminal.Control.DFA;

    /// <summary>
    /// The command base class.
    /// </summary>
    internal abstract class Command
    {
        /// <summary>
        /// An empty command that does nothing.
        /// </summary>
        public static readonly Command None = new None();

        private static readonly Dictionary<string, Command> Commands =
            new Dictionary<string, Command>(StringComparer.InvariantCultureIgnoreCase);

        private string name;

        static Command()
        {
            // this initialization is optional, but it improves
            // speed quite a bit if we don't have to use reflection to
            // find a type by its name; also GetNames() only works with the
            // commands added here
            Add(None);
            Add(new ChangeBrightness());
            Add(new ChangeLanguage());
            Add(new ChangeTtsVolume());
            Add(new Detour());
            Add(new DriverChange());
            Add(new InformationMessages());
            Add(new LogOffAll());
            Add(new LogOffDrive());
            Add(new PassengerCount());
            Add(new Quit());
            Add(new Razzia());
            Add(new ShowAlarms());
            Add(new ShowAnnouncement());
            Add(new SpeechGsm());
            Add(new SpeechIra());
            Add(new SystemCode());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        protected Command()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        protected Command(string name)
            : this()
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.name ?? (this.name = this.GetType().Name);
            }
        }

        /// <summary>
        /// Gets the command for the given name.
        /// </summary>
        /// <param name="name">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="Command"/>.
        /// </returns>
        public static Command GetCommand(string name)
        {
            Command command;
            if (!Commands.TryGetValue(name, out command))
            {
                var type = Type.GetType(typeof(Command).Namespace + "." + name, true, true);
                if (type == null)
                {
                    throw new TypeLoadException("Couldn't find " + name);
                }

                command = (Command)Activator.CreateInstance(type);
                Add(command);
            }

            return command;
        }

        /// <summary>
        /// Gets the available command names.
        /// </summary>
        /// <returns>
        /// The list of names.
        /// </returns>
        public static string[] GetNames()
        {
            var names = new string[Commands.Count];
            Commands.Keys.CopyTo(names, 0);
            return names;
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
        public abstract void Execute(IContext context, bool sourceIsMenu);

        private static void Add(Command command)
        {
            Commands.Add(command.Name, command);
        }
    }
}