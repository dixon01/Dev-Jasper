// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomButton.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VCustomButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    /// <summary>
    /// A custom button.
    /// </summary>
    public partial class VCustomButton : Control
    {
        private Bitmap bitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCustomButton"/> class.
        /// </summary>
        public VCustomButton()
        {
            this.InitializeComponent();
            this.IsPushed = false;
            this.Size = new Size(100, 20);
        }

        ////public VCustomButton(
        ////    Image foreground,
        ////    Image backgroundClicked,
        ////    Image backgroudNormal,
        ////    Image backgroundDisabled,
        ////    Image backgroundFocused)
        ////{
        ////    this.InitializeComponent();
        ////    this.isPushed = false;
        ////    this.Size = new Size(100, 20);
        ////    this.normalBgImage = backgroudNormal;
        ////    this.clickedBgImage = backgroundClicked;
        ////    this.focusedBgImage = backgroundFocused;
        ////    this.DisabledBgImage = backgroundDisabled;
        ////}

        /// <summary>
        /// Gets or sets the foreground image.
        /// </summary>
        [DefaultValue("")]
        public Image FgImage { get; set; }

        /// <summary>
        /// Gets or sets the normal background image.
        /// </summary>
        [DefaultValue("")]
        public Image NormalBgImage { get; set; }

        /// <summary>
        /// Gets or sets the clicked background image.
        /// </summary>
        [DefaultValue("")]
        public Image ClickedBgImage { get; set; }

        /// <summary>
        /// Gets or sets the disabled background image.
        /// </summary>
        [DefaultValue("")]
        public Image DisabledBgImage { get; set; }

        /// <summary>
        /// Gets or sets the focused background image.
        /// </summary>
        public Image FocusedBgImage { get; set; }

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the border color when focused.
        /// </summary>
        public Color BorderColorFocused { get; set; }

        /// <summary>
        /// Gets or sets the border color when clicked.
        /// </summary>
        public Color BorderColorClicked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is in toggle mode.
        /// </summary>
        public bool ToggleMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is currently pushed down.
        /// </summary>
        public bool IsPushed { get; set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="pe">
        /// A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Brush backBrush;
            Pen borderPen;

            if (this.bitmap == null)
            {
                // Bitmap for doublebuffering
                this.bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            using (Graphics graph = Graphics.FromImage(this.bitmap))
            {
                graph.Clear(this.BackColor);

                Image image; // image to be used
                if (this.IsPushed)
                {
                    image = this.ClickedBgImage;
                    backBrush = new SolidBrush(this.BackColor);
                    borderPen = new Pen(this.BorderColorClicked);
                }
                else
                {
                    if (this.Focused)
                    {
                        image = this.FocusedBgImage;
                        backBrush = new SolidBrush(this.BackColor);
                        borderPen = new Pen(this.BorderColorFocused);
                    }
                    else
                    {
                        image = this.NormalBgImage;
                        backBrush = new SolidBrush(this.BackColor);
                        borderPen = new Pen(this.BorderColor);
                    }
                }

                // Prepare rectangle
                graph.FillRectangle(backBrush, this.ClientRectangle);
                var textRect = this.DrawBackgroundImage(image, graph);

                // Prepare rectangle
                Rectangle rc = this.ClientRectangle;
                rc.Width--;
                rc.Height--;

                // Draw rectangle
                graph.DrawRectangle(borderPen, rc);

                // Draw the text to the button
                using (Brush textBrush = new SolidBrush(this.ForeColor))
                {
                    var sf = new StringFormat
                                 {
                                     Alignment = StringAlignment.Center,
                                     LineAlignment = StringAlignment.Center
                                 };
                    graph.DrawString(this.Text, this.Font, textBrush, textRect, sf);

                    // Draw from the memory bitmap
                    pe.Graphics.DrawImage(this.bitmap, 0, 0);
                }
            }

            borderPen.Dispose();
            backBrush.Dispose();

            base.OnPaint(pe);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.ToggleMode)
            {
                this.IsPushed = true;
            }
            else
            {
                this.IsPushed = !this.IsPushed;
            }

            this.Invalidate();
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.ToggleMode)
            {
                this.IsPushed = false;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLostFocus(EventArgs e)
        {
            // [wes] really???
            this.OnGotFocus(e);
        }

        private Color GetBackgroundImageColor(Image image)
        {
            using (var bmp = new Bitmap(image))
            {
                return bmp.GetPixel(0, 0);
            }
        }

        private RectangleF DrawBackgroundImage(Image image, Graphics graph)
        {
            if (image == null)
            {
                return new RectangleF(0, 0, this.Width, this.Height);
            }

            // Center the image relativelly to the control
            ////int imageLeft = (this.Width - image.Width) / 2;
            ////int imageTop = (this.Height - image.Height) / 2;

            Rectangle imgRect; // image rectangle
            RectangleF textRect;
            if (!this.IsPushed)
            {
                ////imgRect = new Rectangle(imageLeft, imageTop, image.Width, image.Height);
                imgRect = new Rectangle(0, 0, this.Width, this.Height);
                textRect = new RectangleF(0, 0, this.Width, this.Height);
            }
            else
            {
                // The button was pressed
                // Shift the image by one pixel
                ////imgRect = new Rectangle(imageLeft + 1, imageTop + 1, image.Width, image.Height);
                imgRect = new Rectangle(0, 0, this.Width, this.Height);
                textRect = new RectangleF(0, 0, this.Width, this.Height);
            }

            // Set transparent key
            var imageAttr = new ImageAttributes();
            var backgroundColor = this.GetBackgroundImageColor(image);
            imageAttr.SetColorKey(backgroundColor, backgroundColor);

            // Draw image
            graph.DrawImage(image, imgRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
            return textRect;
        }
    }
}