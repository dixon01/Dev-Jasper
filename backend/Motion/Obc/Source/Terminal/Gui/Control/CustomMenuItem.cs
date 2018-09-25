// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomMenuItem.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomMenuItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The custom menu item.
    /// </summary>
    public partial class CustomMenuItem : Control
    {
        private Bitmap bitmap;

        private bool isMultiLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMenuItem"/> class.
        /// </summary>
        public CustomMenuItem()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomMenuItem"/> class.
        /// </summary>
        /// <param name="clickAble">
        /// A flag indicating if this item is clickable.
        /// </param>
        public CustomMenuItem(bool clickAble)
        {
            this.PaintBorder = true;
            this.ClickAble = clickAble;
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to paint the border.
        /// </summary>
        public bool PaintBorder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this item is clickable.
        /// </summary>
        public bool ClickAble { get; set; }

        /// <summary>
        /// Gets or sets the border color when focused.
        /// </summary>
        public Color BorderColorFocused { get; set; }

        /// <summary>
        /// Gets or sets the regular border color.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        ///   If you will have two or one line in this control the text is either too high or too low.
        ///   For a single line with a text high 11pixel, you should set this value to 2 or 3.
        ///   By using two text lines you may set this value to 1.
        /// </summary>
        /// <param name = "value">
        /// The new multi-line value
        /// </param>
        internal void SetMultiline(bool value)
        {
            this.isMultiLine = value;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            if (this.ClickAble)
            {
                this.Focus();
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnGotFocus(EventArgs e)
        {
            ////this.BringToFront();
            this.Invalidate();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLostFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="pe">
        /// A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.bitmap == null)
            {
                // Bitmap for doublebuffering
                this.bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            using (Graphics graph = Graphics.FromImage(this.bitmap))
            {
                graph.Clear(this.BackColor);

                // Prepare rectangle
                Rectangle rc = this.ClientRectangle;
                rc.Width--;
                rc.Height--;

                // Draw rectangle
                using (Pen pen = new Pen(this.Focused ? this.BorderColorFocused : this.BorderColor))
                {
                    if (this.PaintBorder)
                    {
                        graph.DrawRectangle(pen, rc);
                    }

                    // Draw the text to the text
                    int y = this.isMultiLine ? 0 : 4;
                    var textRect = new RectangleF(6, y, this.Width - 12, this.Height - 2 - (y / 2));

                    using (Brush textBrush = new SolidBrush(Color.White))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Near };
                        graph.DrawString(this.Text, this.Font, textBrush, textRect, sf);

                        // Draw from the memory bitmap
                        pe.Graphics.DrawImage(this.bitmap, 0, 0);
                        ////base.OnPaint(pe);
                    }
                }
            }
        }
    }
}