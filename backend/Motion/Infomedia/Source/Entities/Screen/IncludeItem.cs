// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncludeItem.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IncludeItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// Screen item that displays another screen inside itself.
    /// </summary>
    public class IncludeItem : DrawableItemBase, IManageable
    {
        private RootItem include;

        /// <summary>
        /// Gets or sets the included screen.
        /// </summary>
        public RootItem Include
        {
            get
            {
                return this.include;
            }

            set
            {
                if (this.include == value)
                {
                    return;
                }

                this.include = value;
                this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Include", value, null));
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            var clone = (IncludeItem)base.Clone();
            clone.Include = (RootItem)this.Include.Clone();
            return clone;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Include {{{0}}} @ [{1},{2}]", this.Include, this.X, this.Y);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return
                parent.Factory.CreateManagementProvider(
                    string.Format(
                        "{0} {1}", this.Include.GetType().Name, this.Include.Id.ToString(CultureInfo.InvariantCulture)),
                    parent,
                        this.Include);
        }

        /// <summary>
        /// Updates the value of a property.
        /// Subclasses have to override this method to set the respective property.
        /// </summary>
        /// <param name="property">
        ///   The name of the property to update.
        /// </param>
        /// <param name="value">
        ///   The new value.
        /// </param>
        /// <param name="animation">
        ///   The animation.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if the <see cref="property"/> name is unknown
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// if the <see cref="value"/> is not of the right type
        /// </exception>
        protected override void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            switch (property)
            {
                case "Include":
                    this.Include = (RootItem)value;
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
}