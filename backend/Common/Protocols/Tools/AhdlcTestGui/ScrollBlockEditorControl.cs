// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollBlockEditorControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScrollBlockEditorControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ahdlc.Providers;

    /// <summary>
    /// The scroll block editor control.
    /// </summary>
    public partial class ScrollBlockEditorControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollBlockEditorControl"/> class.
        /// </summary>
        public ScrollBlockEditorControl()
        {
            this.InitializeComponent();
            this.LargeWidthSupported = false;
            this.ScrollSpeedSupported = false;

            foreach (var speed in Enum.GetValues(typeof(ScrollSpeed)))
            {
                this.comboBoxSpeed.Items.Add(speed);
            }

            this.comboBoxSpeed.SelectedItem = ScrollSpeed.Fastest;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this block is enabled.
        /// </summary>
        public bool BlockEnabled
        {
            get
            {
                return this.checkBoxEnabled.Checked;
            }

            set
            {
                this.checkBoxEnabled.Checked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether large width values are supported (above 255).
        /// </summary>
        public bool LargeWidthSupported
        {
            get
            {
                return this.numericUpDownStartX.Maximum == 0xFFFF;
            }

            set
            {
                var max = value ? 0xFFFF : 0xFF;
                this.numericUpDownStartX.Maximum = max;
                this.numericUpDownEndX.Maximum = max;
                this.numericUpDownWidth.Maximum = max;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether scroll speed is supported.
        /// </summary>
        public bool ScrollSpeedSupported
        {
            get
            {
                return this.comboBoxSpeed.Visible;
            }

            set
            {
                this.comboBoxSpeed.Visible = value;
                this.labelSpeed.Visible = value;
            }
        }

        /// <summary>
        /// Gets the viewport (rectangle within the sign where to display the scroll block).
        /// </summary>
        public Rectangle Viewport
        {
            get
            {
                return new Rectangle(
                    (int)this.numericUpDownStartX.Value,
                    (int)this.numericUpDownStartY.Value,
                    (int)(this.numericUpDownEndX.Value - this.numericUpDownStartX.Value),
                    (int)(this.numericUpDownEndY.Value - this.numericUpDownStartY.Value));
            }
        }

        /// <summary>
        /// Gets the scroll speed of this block.
        /// </summary>
        public ScrollSpeed ScrollSpeed
        {
            get
            {
                return (ScrollSpeed)this.comboBoxSpeed.SelectedItem;
            }
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap Bitmap
        {
            get
            {
                return this.bitmapCreatorControl.Bitmap;
            }
        }

        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            var width = this.numericUpDownWidth.Value;
            var height = Math.Abs(this.numericUpDownEndY.Value - this.numericUpDownStartY.Value);
            this.bitmapCreatorControl.BitmapSize = new Size((int)width, (int)height);
        }
    }
}
