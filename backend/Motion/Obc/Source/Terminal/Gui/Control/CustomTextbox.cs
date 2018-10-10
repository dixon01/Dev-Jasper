// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTextbox.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomTextbox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Gui.Control
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// The custom textbox.
    /// </summary>
    public partial class CustomTextbox : Control
    {
        private readonly Timer tmr = new Timer();

        private Bitmap bitmap;

        private string cursorText;

        private bool cursorVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTextbox"/> class.
        /// </summary>
        public CustomTextbox()
        {
            this.InitializeComponent();
            this.tmr.Tick += this.TmrTick;
            this.Password = false;
        }

        /// <summary>
        /// Gets or sets the border color when focused.
        /// </summary>
        public Color BorderColorFocused { get; set; }

        /// <summary>
        /// Gets or sets the regular border color.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this textbox represents a password.
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Append one character to this textbox.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        public void AppendCharacter(char character)
        {
            this.Text += Convert.ToChar(character);
        }

        /// <summary>
        /// Removes the last character from this textbox.
        /// </summary>
        /// <returns>
        /// True if a character was removed.
        /// </returns>
        public bool RemoveLastChar()
        {
            if (this.Text.Length == 0)
            {
                return false;
            }

            this.Text = this.Text.Substring(0, this.Text.Length - 1);
            return true;
        }

        /// <summary>
        /// Clears the contents of this textbox.
        /// </summary>
        public void Clear()
        {
            this.Text = string.Empty;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.KeyPressEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            bool handled = false;
            switch ((Keys)e.KeyChar)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    this.AppendCharacter(e.KeyChar);
                    handled = true;
                    break;

                case Keys.Back:
                    handled = this.RemoveLastChar();

                    break;

                case Keys.Clear:
                    this.Clear();
                    handled = true;
                    break;
            }

            if (handled)
            {
                this.Invalidate();
            }

            e.Handled = handled;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            base.OnClick(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnGotFocus(EventArgs e)
        {
            this.tmr.Interval = 500;
            this.tmr.Enabled = true;
            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLostFocus(EventArgs e)
        {
            this.DrawCursor();
            this.tmr.Enabled = false;
            this.Invalidate();
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
                this.bitmap = new Bitmap(this.Width, this.Height);
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
                    graph.DrawRectangle(pen, rc);

                    // Draw the text to the button
                    var textRect = new RectangleF(4, 1, this.Width - 8, this.Height - 2); // text rectangle
                    using (Brush textBrush = new SolidBrush(this.ForeColor))
                    {
                        var sf = new StringFormat
                                     {
                                         Alignment = StringAlignment.Near,
                                         LineAlignment = StringAlignment.Center
                                     };

                        var str = this.Password ? new string('*', this.Text.Length) : this.Text;
                        graph.DrawString(str, this.Font, textBrush, textRect, sf);
                    }
                }
            }

            // Draw from the memory bitmap
            pe.Graphics.DrawImage(this.bitmap, 0, 0);

            base.OnPaint(pe);
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains information about the control to paint.
        /// </param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Don't allow the background to paint
        }

        private void TmrTick(object sender, EventArgs e)
        {
            this.DrawCursor();
        }

        private void DrawCursor()
        {
            using (Graphics graph = this.CreateGraphics())
            {
                var str = this.Password ? new string('*', this.Text.Length) : this.Text;

                // Offscreen graphics
                if (this.Focused)
                {
                    // Implement the cursor
                    if (this.cursorVisible)
                    {
                        this.cursorText = str + "|";
                        this.cursorVisible = false;
                    }
                    else
                    {
                        this.cursorText = str;
                        this.cursorVisible = true;
                    }
                }
                else
                {
                    // Cursor stuff
                    this.cursorText = str;
                    this.cursorVisible = false;
                }

                // fill the rectangle
                var backRect = new Rectangle(4, 1, this.Width - 8, this.Height - 2);

                using (Brush backBrush = new SolidBrush(this.BackColor))
                {
                    graph.FillRectangle(backBrush, backRect);

                    // Write the text
                    var textRect = new RectangleF(4, 1, this.Width - 8, this.Height - 2);

                    using (Brush textBrush = new SolidBrush(this.ForeColor))
                    {
                        // text rectangle
                        var sf = new StringFormat
                                     {
                                         Alignment = StringAlignment.Near,
                                         LineAlignment = StringAlignment.Center
                                     };
                        graph.DrawString(this.cursorText, this.Font, textBrush, textRect, sf);
                    }
                }
            }
        }
    }
}