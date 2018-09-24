// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenItemDrawer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenItemDrawer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Base class for drawing classes that draw a given <see cref="DrawableItemBase"/>.
    /// </summary>
    public abstract class ScreenItemDrawer : IDisposable
    {
        /// <summary>
        /// Gets the draw item being drawn by this class.
        /// </summary>
        public DrawableItemBase Item { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ScreenItemDrawer"/> for the given
        /// <see cref="DrawableItemBase"/>.
        /// </summary>
        /// <param name="item">
        /// The item to be drawn.
        /// </param>
        /// <returns>
        /// a new drawer for the given item. 
        /// </returns>
        public static ScreenItemDrawer Create(DrawableItemBase item)
        {
            var drawer = CreateDrawer(item);
            drawer.Item = item;
            return drawer;
        }

        /// <summary>
        /// Prepares this drawer to be able to draw on the given device.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public abstract void Prepare(Device device);

        /// <summary>
        /// Draws this object to the device previously given
        /// by <see cref="Prepare"/>.
        /// </summary>
        /// <param name="context">
        /// The context parameters in which this drawer has to draw itself.
        /// </param>
        public abstract void Draw(IItemDrawContext context);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();

        private static ScreenItemDrawer CreateDrawer(DrawableItemBase item)
        {
            var text = item as TextItem;
            if (text != null)
            {
                return new TextDrawer();
            }

            var image = item as ImageItem;
            if (image != null)
            {
                return new ImageDrawer();
            }

            throw new NotSupportedException("Could not create drawer for " + item.GetType().FullName);
        }
    }
}
