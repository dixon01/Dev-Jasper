// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextDrawer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextDrawer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    using System;

    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.DirectX.Direct3D;

    using DxFont = Microsoft.DirectX.Direct3D.Font;

    /// <summary>
    /// Drawer for a text.
    /// </summary>
    public class TextDrawer : ScreenItemDrawer
    {
        private DxFont font;

        /// <summary>
        /// Prepares this drawer to be able to draw on the given device.
        /// Usually this creates all necessary DirectX resources.
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        public override void Prepare(Device device)
        {
            var item = (TextItem)this.Item;
            this.font = new DxFont(
                device,
                item.Font.Height,
                0,
                (FontWeight)item.Font.Weight,
                10,
                item.Font.Italic,
                CharacterSet.Default,
                Precision.Default,
                FontQuality.AntiAliased,
                PitchAndFamily.DefaultPitch,
                item.Font.Face);

            // TODO: how about using a sprite instead of drawing directly to the surface?
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
            var item = (TextItem)this.Item;

            // TODO: look at other parameters (rectangle, scaling, ...)
            var color = item.Font.GetColor();
            this.font.DrawText(null, item.Text, item.X, item.Y, color);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            if (this.font == null)
            {
                return;
            }

            this.font.Dispose();
        }
    }
}