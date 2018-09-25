// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SideTab.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SideTab type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;

    /// <summary>
    /// Tab header control for <see cref="IbisTabControl"/>.
    /// </summary>
    [DefaultEvent("Click")]
    public partial class SideTab : UserControl
    {
        private bool selected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SideTab"/> class.
        /// </summary>
        public SideTab()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this tab is to be drawn as selected.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Selected
        {
            get
            {
                return this.selected;
            }

            set
            {
                if (this.selected == value)
                {
                    return;
                }

                this.selected = value;
                this.Invalidate(false);
            }
        }

        /// <summary>
        /// Gets or sets the title of the tab.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        public string Title
        {
            get
            {
                return this.labelTitle.Text;
            }

            set
            {
                this.labelTitle.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the description of the tab.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue("")]
        public string Description
        {
            get
            {
                return this.labelDescription.Text;
            }

            set
            {
                this.labelDescription.Text = value;
            }
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            var graphics = e.Graphics;

            var rect = new Rectangle(Point.Empty, this.Size);
            rect.Inflate(0, -3);
            rect.Width--;

            int roundCorner = Math.Min(10, rect.Height / 2);

            var pen = SystemPens.ControlDarkDark;
            if (roundCorner > 0)
            {
                graphics.DrawArc(pen, rect.Left, rect.Top, roundCorner, roundCorner, 180, 90);
                graphics.DrawArc(pen, rect.Left, rect.Bottom - roundCorner, roundCorner, roundCorner, 90, 90);
            }

            graphics.DrawLine(pen, rect.Left, rect.Top + (roundCorner / 2), rect.Left, rect.Bottom - (roundCorner / 2));
            graphics.DrawLine(pen, rect.Left + (roundCorner / 2), rect.Top, rect.Right, rect.Top);
            graphics.DrawLine(pen, rect.Left + (roundCorner / 2), rect.Bottom, rect.Right, rect.Bottom);

            if (this.Selected)
            {
                graphics.DrawLine(pen, rect.Right, 0, rect.Right, rect.Top);
                graphics.DrawLine(pen, rect.Right, rect.Bottom, rect.Right, this.Height);
            }
            else
            {
                graphics.DrawLine(pen, rect.Right, 0, rect.Right, this.Height);
            }
        }

        /// <summary>
        /// Raises the <see cref="Control.Resize"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private void LabelTitleClick(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void LabelDescriptionClick(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}
