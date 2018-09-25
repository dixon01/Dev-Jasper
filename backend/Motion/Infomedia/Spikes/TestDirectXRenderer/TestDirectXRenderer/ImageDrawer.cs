// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageDrawer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageDrawer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    using System.Drawing;
    using System.IO;

    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Drawer for an image.
    /// </summary>
    public class ImageDrawer : ScreenItemDrawer
    {
        private Bitmap bitmap;
        private Sprite sprite;

        private Texture texture;

        private Device device;

        private bool recreateSprite;

        /// <summary>
        /// Prepares this drawer to be able to draw on the given device.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="dev">
        /// The device.
        /// </param>
        public override void Prepare(Device dev)
        {
            this.device = dev;
            this.CreateSprite();
        }

        /// <summary>
        /// Draws this object to the device previously given
        /// by <see cref="ScreenItemDrawer.Prepare"/>.
        /// </summary>
        /// <param name="context">
        /// The context parameters in which this drawer has to draw itself.
        /// </param>
        public override void Draw(IItemDrawContext context)
        {
            if (this.recreateSprite)
            {
                this.recreateSprite = false;
                this.DisposeSprite();
                this.CreateSprite();
            }

            if (this.sprite == null)
            {
                return;
            }

            var item = (ImageItem)this.Item;
            if (item.Blink && !context.BlinkOn)
            {
                return;
            }

            this.sprite.Begin(SpriteFlags.None);
            this.sprite.Draw2D(
                this.texture,
                new Rectangle(Point.Empty, this.bitmap.Size),
                new SizeF(item.Width, item.Height),
                new PointF(item.X, item.Y),
                Color.Transparent);
            this.sprite.End();

            item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.DisposeSprite();
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Filename")
            {
                return;
            }

            this.recreateSprite = true;
        }

        private void CreateSprite()
        {
            var item = (ImageItem)this.Item;
            if (!File.Exists(item.Filename))
            {
                return;
            }

            this.bitmap = new Bitmap(item.Filename);
            this.texture = new Texture(this.device, this.bitmap, Usage.None, Pool.Managed);
            this.sprite = new Sprite(this.device);
        }

        private void DisposeSprite()
        {
            if (this.sprite == null)
            {
                return;
            }

            this.sprite.Dispose();
            this.texture.Dispose();
            this.bitmap.Dispose();

            this.sprite = null;
            this.texture = null;
            this.bitmap = null;
        }
    }
}