// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogDirectoryType.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DialogDirectoryType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Wrapper object for a string directory type used for storing the last used directories for a file dialog.
    /// </summary>
    [DataContract]
    public sealed class DialogDirectoryType : IEquatable<DialogDirectoryType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogDirectoryType"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public DialogDirectoryType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException();
            }

            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DialogDirectoryType other)
        {
            return this.Name.Equals(other.Name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="obj"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="obj">An object to compare with this object.</param>
        public override bool Equals(object obj)
        {
            var other = obj as DialogDirectoryType;
            if (other == null || this.GetType() != other.GetType())
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}
