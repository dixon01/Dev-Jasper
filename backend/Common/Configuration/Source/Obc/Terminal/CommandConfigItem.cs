// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandConfigItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandConfigItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Terminal
{
    using System.Xml.Serialization;

    /// <summary>
    /// The command config item.
    /// </summary>
    public class CommandConfigItem
    {
        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        [XmlText]
        public CommandName Command { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.Command.ToString();
        }
    }
}