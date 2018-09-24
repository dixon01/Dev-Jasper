// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenRoot.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenRoot type.
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
    /// The root of the screen hierarchy.
    /// </summary>
    public class ScreenRoot : ItemBase, IManageable
    {
        private bool visible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenRoot"/> class.
        /// </summary>
        public ScreenRoot()
        {
            this.visible = true;
        }

        /// <summary>
        /// Gets or sets the root item to be shown on the screen.
        /// You shouldn't use the setter to change the screen at a later time,
        /// but rather send a new <see cref="ScreenRoot"/> to the renderer.
        /// </summary>
        public RootItem Root { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this screen is visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (this.visible == value)
                {
                    return;
                }

                this.visible = value;
                this.RaisePropertyValueChanged(new AnimatedPropertyChangedEventArgs("Visible", value, null));
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
            var clone = (ScreenRoot)base.Clone();
            clone.Root = (RootItem)this.Root.Clone();
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
            return string.Format("Screen {{{0}}}", this.Root);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return
                parent.Factory.CreateManagementProvider(
                    string.Format(
                        "{0} {1}", this.Root.GetType().Name, this.Root.Id.ToString(CultureInfo.InvariantCulture)),
                    parent,
                    this.Root);
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
                case "Visible":
                    this.Visible = (bool)value;
                    break;
                default:
                    base.SetProperty(property, value, animation);
                    break;
            }
        }
    }
}
