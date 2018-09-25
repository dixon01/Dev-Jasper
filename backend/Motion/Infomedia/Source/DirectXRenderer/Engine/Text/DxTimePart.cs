// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DxTimePart.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DxTimePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine.Text
{
    using System;
    using System.Drawing;
    using System.Globalization;

    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;

    /// <summary>
    /// Dynamic time part for DirectX.
    /// </summary>
    public class DxTimePart : DxPart, ITextPart
    {
        private readonly string timeFormat;

        private readonly TextPartCreator creator;

        private DxTextPartBase textPart;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxTimePart"/> class.
        /// </summary>
        /// <param name="timeFormat">
        /// The time format, see also: <see cref="DateTime.ToString(string)"/>
        /// </param>
        /// <param name="creator">
        /// The factory method creating a <see cref="DxTextPartBase"/>.
        /// </param>
        /// <param name="blink">
        /// A flag indicating if this part should blink.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public DxTimePart(string timeFormat, TextPartCreator creator, bool blink, IDxDeviceRenderContext context)
            : base(blink, context)
        {
            this.timeFormat = timeFormat;
            this.creator = creator;

            this.textPart = this.creator(this.GetTimeString());
        }

        /// <summary>
        /// Delegate for the factory method of <see cref="DxTextPartBase"/> objects.
        /// </summary>
        /// <param name="text">
        /// The text that should be rendered in the created <see cref="DxTextPartBase"/>.
        /// </param>
        /// <returns>
        /// A new object of a subclass of <see cref="DxTextPartBase"/>.
        /// </returns>
        public delegate DxTextPartBase TextPartCreator(string text);

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return this.textPart.Text;
            }
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        public Font Font
        {
            get
            {
                return this.textPart.Font;
            }
        }

        /// <summary>
        /// Updates the contents of this part if necessary before rendering it.
        /// This implementation checks if the formatted time string has changed,
        /// if so it creates a new <see cref="DxTextPartBase"/> for the new text.
        /// </summary>
        /// <returns>
        /// True if the content has been updated and thus the part (and probably the entire text)
        /// has to be recalculated.
        /// </returns>
        public override bool UpdateContent()
        {
            var text = this.GetTimeString();
            if (text == this.textPart.Text)
            {
                return false;
            }

            var oldPart = this.textPart;
            this.textPart = this.creator(text);
            oldPart.Dispose();
            return true;
        }

        /// <summary>
        /// Duplicates this part.
        /// The duplicates are used for alternatives.
        /// </summary>
        /// <returns>
        /// A new <see cref="DxTimePart"/>.
        /// </returns>
        public override IPart Duplicate()
        {
            return new DxTimePart(this.timeFormat, this.creator, this.Blink, this.Context);
        }

        /// <summary>
        /// Moves this part to the given location.
        /// </summary>
        /// <param name="x">
        /// The new X coordinate of the part.
        /// </param>
        /// <param name="y">
        /// The new Y coordinate of the part.
        /// </param>
        public override void MoveTo(int x, int y)
        {
            base.MoveTo(x, y);
            this.textPart.MoveTo(x, y);
        }

        /// <summary>
        /// Sets the scaling factor of this part.
        /// A scaling of 1.0 means identity, between 0.0 and 1.0 is a reduction in size.
        /// </summary>
        /// <param name="factor">
        /// The factor, usually between 0.0 and (including) 1.0.
        /// </param>
        public override void SetScaling(double factor)
        {
            this.textPart.SetScaling(factor);
            this.SetSize(this.textPart.Width, this.textPart.Height, this.textPart.Ascent);
        }

        /// <summary>
        /// Tries to split the part into two parts at the given offset.
        /// </summary>
        /// <param name="offset">
        /// The offset at which this item should be split.
        /// </param>
        /// <param name="left">
        /// The left part of the split operation. This is never null.
        /// If the part couldn't be split, this return parameter might be the object this method was called on.
        /// </param>
        /// <param name="right">
        /// The right part of the split operation. This is null if the method returns false.
        /// </param>
        /// <returns>
        /// A flag indicating if the split operation was successful.
        /// </returns>
        public override bool Split(int offset, out DxPart left, out DxPart right)
        {
            // we currently don't support splitting the time (doesn't really make sense anyways)
            left = this;
            right = null;
            return false;
        }

        /// <summary>
        /// Renders this part using the given sprite.
        /// </summary>
        /// <param name="x">
        /// The absolute x position of the parent.
        /// </param>
        /// <param name="y">
        /// The absolute y position of the parent.
        /// </param>
        /// <param name="rotationCenter">
        /// The absolut position around which this part should be rotated.
        /// </param>
        /// <param name="rotationAngle">
        /// The angle in radian at which should be rotated.
        /// </param>
        /// <param name="alpha">
        /// The alpha value.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(
            int x, int y, PointF rotationCenter, float rotationAngle, int alpha, IDxRenderContext context)
        {
            this.textPart.Render(x, y, rotationCenter, rotationAngle, alpha, context);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.textPart.Dispose();
        }

        /// <summary>
        /// This is called when the device is reset.
        /// </summary>
        public override void OnResetDevice()
        {
            this.textPart.OnResetDevice();
        }

        /// <summary>
        /// This is called when the device is lost.
        /// </summary>
        public override void OnLostDevice()
        {
            this.textPart.OnLostDevice();
        }

        /// <summary>
        /// Gets the date/time string to be shown for the format provided in the constructor.
        /// </summary>
        /// <returns>the properly formatted date/time string.</returns>
        private string GetTimeString()
        {
            return TimeProvider.Current.Now.ToString(this.timeFormat, CultureInfo.InvariantCulture);
        }
    }
}