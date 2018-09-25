// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Screen
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// A single item on a screen.
    /// </summary>
    [XmlInclude(typeof(TextItem))]
    [XmlInclude(typeof(ImageItem))]
    [XmlInclude(typeof(VideoItem))]
    [XmlInclude(typeof(IncludeItem))]
    [XmlInclude(typeof(AnalogClockItem))]
    [XmlInclude(typeof(ImageListItem))]
    public abstract class ItemBase : ICloneable, IManageableObject
    {
        private static int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemBase"/> class.
        /// </summary>
        protected ItemBase()
        {
            this.Id = Interlocked.Increment(ref nextId);
        }

        /// <summary>
        /// Event that is fired whenever a property changes.
        /// </summary>
        public event EventHandler<AnimatedPropertyChangedEventArgs> PropertyValueChanged;

        /// <summary>
        /// Gets or sets a unique id of this item used in <see cref="PropertyValueChanged"/>
        /// to know which item has changed.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the element ID.
        /// </summary>
        public int ElementId { get; set; }

        /// <summary>
        /// Updates this item with the given <see cref="ItemUpdate"/>.
        /// </summary>
        /// <param name="update">
        /// The update.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if the <see cref="Id"/> of this item does not match
        /// the given <see cref="ItemUpdate.ScreenItemId"/>.
        /// </exception>
        public void Update(ItemUpdate update)
        {
            if (update.ScreenItemId != this.Id)
            {
                throw new ArgumentException("Screen item id doesn't match");
            }

            if (update.Property == "Include")
            {
                var rootItem = update.Value as RootItem;
                if (rootItem != null)
                {
                    foreach(var item in rootItem.Items)
                    {
                        string fileName = string.Empty;
                        if (item is ImageItem imageItem)
                        {
                            fileName = imageItem.Filename;
                        }
                        else if (item is VideoItem videoItem)
                        {
                            fileName = videoItem.VideoUri;
                        }
                        else
                        {
                            continue;
                        }

                        var message = new DrawableComposerInitMessage();
                        message.UnitName = MessageDispatcher.Instance.LocalAddress.Unit;
                        message.ElementID = item.ElementId;
                        message.Status = DrawableStatus.Rendering;
                        message.ElementFileName = fileName;
                        MessageDispatcher.Instance.Broadcast(message);
                        
                    }
                }
            }
            this.SetProperty(update.Property, update.Value, update.Animation);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            var clone = (ItemBase)this.MemberwiseClone();
            clone.PropertyValueChanged = null;
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
            return this.GetType().Name;
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                yield return new ManagementProperty<string>(
                    propertyInfo.Name, Convert.ToString(propertyInfo.GetValue(this, null)), true);
            }
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
        protected virtual void SetProperty(string property, object value, PropertyChangeAnimation animation)
        {
            throw new ArgumentException(
                string.Format("Property not found: {0} in {1}", property, this.GetType()));
        }

        /// <summary>
        /// Raises the <see cref="PropertyValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaisePropertyValueChanged(AnimatedPropertyChangedEventArgs e)
        {
            var handler = this.PropertyValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}