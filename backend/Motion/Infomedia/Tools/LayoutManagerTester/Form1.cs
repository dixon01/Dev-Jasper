// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form1.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.LayoutManagerTester
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.DirectX.Direct3D;

    using Font = Gorba.Common.Configuration.Infomedia.Layout.Font;
    using HorizontalAlignment = Gorba.Common.Configuration.Infomedia.Layout.HorizontalAlignment;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string DefaultText = "The quick brown fox jumps over the lazy dog";

        private readonly DirectXPanel renderPanel = new DirectXPanel();

        private Point mouseDownPos;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.renderPanel.Dock = DockStyle.Fill;
            this.renderPanel.Cursor = Cursors.Cross;
            this.renderPanel.MouseDown += this.RenderPanelOnMouseDown;
            this.renderPanel.MouseMove += this.RenderPanelOnMouseMove;
            this.renderPanel.MouseUp += this.RenderPanelOnMouseUp;
            this.splitContainer1.Panel2.Controls.Add(this.renderPanel);

            this.textBox.Text = DefaultText;

            this.FillComboBox(this.comboBoxTextMode, typeof(TextMode));
            this.FillComboBox(this.comboBoxFontQuality, typeof(FontQuality));
            this.FillComboBox(this.comboBoxAlign, typeof(HorizontalAlignment));
            this.FillComboBox(this.comboBoxVAlign, typeof(VerticalAlignment));
            this.FillComboBox(this.comboBoxOverflow, typeof(TextOverflow));
            this.FillComboBox(this.comboBoxWeight, typeof(FontWeight));

            this.comboBoxTextMode.SelectedItem = TextMode.FontSprite;
            this.comboBoxFontQuality.SelectedItem = FontQuality.AntiAliased;
            this.comboBoxWeight.SelectedItem = FontWeight.Regular;

            foreach (var font in FontFamily.Families)
            {
                this.comboBoxFace.Items.Add(font.Name);
                this.toolStripComboBoxFont.Items.Add(font.Name);
            }

            this.comboBoxFace.SelectedItem = "Arial";

            foreach (var imageFile in Directory.GetFiles("Images\\"))
            {
                var item = new ToolStripMenuItem(Path.GetFileName(imageFile));
                item.Click += this.AddImageMenuItemOnClick;
                item.Tag = imageFile;
                item.Image = Image.FromFile(imageFile);
                this.toolStripDropDownButtonImages.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.UpdateTextBounds();
        }

        private void FillComboBox(ComboBox comboBox, Type type)
        {
            comboBox.Items.Clear();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                comboBox.Items.Add(field.GetValue(null));
            }

            comboBox.SelectedIndex = 0;
        }

        private void SurroundSelectedText(string left, string right)
        {
            var start = this.textBox.SelectionStart;
            var end = start + this.textBox.SelectionLength;
            this.textBox.SuspendLayout();
            try
            {
                this.textBox.Text = this.textBox.Text.Substring(0, start) + left
                                    + this.textBox.Text.Substring(start, end - start) + right
                                    + this.textBox.Text.Substring(end);
                this.textBox.Select(start + left.Length, end - start);
            }
            finally
            {
                this.textBox.ResumeLayout(true);
            }
        }

        private void UpdateTextItem()
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            var item = new TextItem();

            var textConfig = new TextConfig();
            textConfig.TextMode = (TextMode)this.comboBoxTextMode.SelectedItem;
            textConfig.FontQuality = (FontQualities)this.comboBoxFontQuality.SelectedItem;

            var bounds = this.GetTextBounds();
            item.X = bounds.X;
            item.Y = bounds.Y;
            item.Width = bounds.Width;
            item.Height = bounds.Height;
            item.Visible = true;

            item.Align = (HorizontalAlignment)this.comboBoxAlign.SelectedItem;
            item.VAlign = (VerticalAlignment)this.comboBoxVAlign.SelectedItem;
            item.Overflow = (TextOverflow)this.comboBoxOverflow.SelectedItem;
            item.ScrollSpeed = (int)this.numericUpDownScrollSpeed.Value;
            item.Rotation = (int)this.numericUpDownRotation.Value;

            item.Font = new Font
                            {
                                Face = this.comboBoxFace.Text,
                                Height = (int)this.numericUpDownSize.Value,
                                Weight = (int)(FontWeight)this.comboBoxWeight.SelectedItem,
                                Italic = this.checkBoxItalic.Checked,
                                Color = this.ColorToString(this.panelColor.BackColor)
                            };

            item.Text = this.textBox.Text.Replace("\r\n", "[br]");

            this.renderPanel.UpdateTextItem(item);
        }

        private void UpdateTextBounds()
        {
            this.renderPanel.TextBounds = this.GetTextBounds();
            this.UpdateTextItem();
        }

        private Rectangle GetTextBounds()
        {
            return new Rectangle(
                (int)this.numericUpDownX.Value,
                (int)this.numericUpDownY.Value,
                (int)this.numericUpDownWidth.Value,
                (int)this.numericUpDownHeight.Value);
        }

        private string ColorToString(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        private void PanelColorClick(object sender, EventArgs e)
        {
            this.colorDialog.Color = this.panelColor.BackColor;
            if (this.colorDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.panelColor.BackColor = this.colorDialog.Color;
            this.UpdateTextItem();
        }

        private void PanelBackgroundClick(object sender, EventArgs e)
        {
            this.colorDialog.Color = this.panelBackground.BackColor;
            if (this.colorDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.panelBackground.BackColor = this.colorDialog.Color;
            this.renderPanel.BackColor = this.colorDialog.Color;
        }

        private void ToolStripComboBoxFontSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.toolStripComboBoxFont.SelectedItem == null)
            {
                return;
            }

            this.SurroundSelectedText("[face=" + this.toolStripComboBoxFont.SelectedItem + "]", "[/face]");
            this.toolStripComboBoxFont.SelectedItem = null;
        }

        private void ToolStripMenuItemFontSizeClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            if (menuItem == null)
            {
                return;
            }

            this.SurroundSelectedText("[size=" + menuItem.Text + "]", "[/size]");
        }

        private void ToolStripButtonBoldClick(object sender, EventArgs e)
        {
            this.SurroundSelectedText("[b]", "[/b]");
        }

        private void ToolStripButtonItalicClick(object sender, EventArgs e)
        {
            this.SurroundSelectedText("[i]", "[/i]");
        }

        private void ToolStripButtonColorClick(object sender, EventArgs e)
        {
            this.colorDialog.Color = Color.White;
            if (this.colorDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            var tag = string.Format("[color={0}]", this.ColorToString(this.colorDialog.Color));
            this.SurroundSelectedText(tag, "[/color]");
        }

        private void ToolStripButtonAlternatingClick(object sender, EventArgs e)
        {
            var start = this.textBox.SelectionStart;
            var end = start + this.textBox.SelectionLength;
            this.textBox.SuspendLayout();
            try
            {
                this.textBox.Text = this.textBox.Text.Substring(0, start) + "[a]"
                                    + this.textBox.Text.Substring(start, end - start) + "[|]other[/a]"
                                    + this.textBox.Text.Substring(end);
                this.textBox.Select(end + 6, 5);
            }
            finally
            {
                this.textBox.ResumeLayout(true);
            }
        }

        private void ToolStripButtonBlinkClick(object sender, EventArgs e)
        {
            this.SurroundSelectedText("[bl]", "[/bl]");
        }

        private void ToolStripButtonTimeClick(object sender, EventArgs e)
        {
            this.textBox.Paste("[time=HH:mm]");
        }

        private void AddImageMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            this.textBox.Paste(string.Format("[img={0}]", item.Tag));
        }

        private void ToolStripButtonResetClick(object sender, EventArgs e)
        {
            this.textBox.Text = DefaultText;
        }

        private void NumericUpDownBoundsValueChanged(object sender, EventArgs e)
        {
            this.UpdateTextBounds();
        }

        private void FormattingChanged(object sender, EventArgs e)
        {
            this.UpdateTextItem();
        }

        private void RenderPanelOnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.mouseDownPos = e.Location;
            this.RenderPanelOnMouseMove(sender, e);
        }

        private void RenderPanelOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (Math.Abs(this.mouseDownPos.X - e.X) <= 5
                && Math.Abs(this.mouseDownPos.Y - e.Y) <= 5)
            {
                this.renderPanel.TextBounds = new Rectangle(this.mouseDownPos, Size.Empty);
            }
            else
            {
                var rect = new Rectangle(
                    this.mouseDownPos.X, this.mouseDownPos.Y, e.X - this.mouseDownPos.X, e.Y - this.mouseDownPos.Y);
                if (rect.Width < 0)
                {
                    rect.X = rect.Right;
                    rect.Width = -rect.Width;
                }

                if (rect.Height < 0)
                {
                    rect.Y = rect.Bottom;
                    rect.Height = -rect.Height;
                }

                this.renderPanel.TextBounds = rect;
            }
        }

        private void RenderPanelOnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var rect = this.renderPanel.TextBounds;
            this.numericUpDownX.Value = rect.X;
            this.numericUpDownY.Value = rect.Y;
            this.numericUpDownWidth.Value = rect.Width;
            this.numericUpDownHeight.Value = rect.Height;
        }
    }
}
