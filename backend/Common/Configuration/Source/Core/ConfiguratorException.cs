// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfiguratorException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Execption for configuration serialization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;

    /// <summary>
    /// Execption for configuration serialization.
    /// </summary>
    public class ConfiguratorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="type">
        /// Type of the config manager. The config manager represents the de/serialized class at the top of the config file.
        /// </param>
        public ConfiguratorException(string message, Type type)
            : base(message)
        {
            this.ConfigMngrName = type.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfiguratorException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="type">
        /// Type of the config manager. The config manager represents the de/serialized class at the top of the config file.
        /// </param>
        /// <param name="innerException">
        /// Inner exception to pass within the serializer exception
        /// </param>
        public ConfiguratorException(string message, Type type, Exception innerException)
            : base(message, innerException)
        {
            this.ConfigMngrName = type.Name;
        }

        /// <summary>
        /// Gets ConfigMngrName.
        /// </summary>
        public string ConfigMngrName { get; private set; }
    }
}
