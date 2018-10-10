// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecutableFile.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExecutableFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Messages
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// A file taken from resources that is executed as part of <see cref="RunCommands"/>.
    /// </summary>
    public class ExecutableFile : FileUpdate, IEquatable<ExecutableFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutableFile"/> class.
        /// </summary>
        public ExecutableFile()
        {
            this.Arguments = string.Empty;
        }

        /// <summary>
        /// Gets or sets the command line arguments for the executable.
        /// </summary>
        [XmlAttribute("Args")]
        public string Arguments { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals(ExecutableFile other)
        {
            return base.Equals(other) && this.Arguments.Equals(other.Arguments, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the
        /// current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current
        /// <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current
        /// <see cref="T:System.Object"/>.</param>
        public override bool Equals(object obj)
        {
            var other = obj as ExecutableFile;
            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Arguments.GetHashCode();
        }
    }
}