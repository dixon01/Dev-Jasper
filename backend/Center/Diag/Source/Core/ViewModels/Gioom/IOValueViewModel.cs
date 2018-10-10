// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOValueViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOValueViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Gioom
{
    using System;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model that represents a value that can be assigned to a <see cref="GioomPortViewModelBase"/>.
    /// </summary>
    public class IOValueViewModel : ViewModelBase, IEquatable<IOValueViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOValueViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The human readable name.
        /// </param>
        /// <param name="value">
        /// The integer value.
        /// </param>
        public IOValueViewModel(string name, int value)
        {
            this.Value = value;
            this.Name = name;
        }

        /// <summary>
        /// Gets the human readable name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IOValueViewModel other)
        {
            return other != null && other.Value == this.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current
        /// <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            var other = obj as IOValueViewModel;
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
            return this.Value.GetHashCode();
        }
    }
}