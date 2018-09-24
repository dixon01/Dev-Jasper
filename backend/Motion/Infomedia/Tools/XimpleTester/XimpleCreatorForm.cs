// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleCreatorForm.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleCreatorForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XimpleTester
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Media;
    using System.Windows.Forms;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// The ximple creator form.
    /// </summary>
    public partial class XimpleCreatorForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleCreatorForm"/> class.
        /// </summary>
        public XimpleCreatorForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the selected Ximple cell.
        /// </summary>
        public XimpleCell SelectedCell
        {
            get
            {
                return this.listBoxCells.SelectedItem as XimpleCell;
            }
        }

        private void ListBoxCellsDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0 && e.Index < this.listBoxCells.Items.Count)
            {
                var cell = this.listBoxCells.Items[e.Index] as XimpleCell;
                if (cell != null)
                {
                    var text = string.Format(
                        "[{0},{1},{2},{3}] = {4}",
                        cell.LanguageNumber,
                        cell.TableNumber,
                        cell.ColumnNumber,
                        cell.RowNumber,
                        cell.Value);
                    using (Brush myBrush = new SolidBrush(e.ForeColor))
                    {
                        e.Graphics.DrawString(text, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
                    }
                }
            }

            e.DrawFocusRectangle();
        }

        private void ButtonAddClick(object sender, EventArgs e)
        {
            var index = this.listBoxCells.Items.Add(new XimpleCell());
            this.listBoxCells.SelectedIndex = index;
            this.buttonSend.Enabled = true;
        }

        private void ButtonRemoveClick(object sender, EventArgs e)
        {
            this.listBoxCells.Items.Remove(this.SelectedCell);
            this.buttonSend.Enabled = this.listBoxCells.Items.Count > 0;
        }

        private void ListBoxCellsSelectedIndexChanged(object sender, EventArgs e)
        {
            var cell = this.SelectedCell;
            this.buttonRemove.Enabled = cell != null;
            this.comboBoxLanguage.Enabled = cell != null;
            this.comboBoxTable.Enabled = cell != null;
            this.comboBoxColumn.Enabled = cell != null;
            this.numericUpDownRow.Enabled = cell != null;
            this.textBoxValue.Enabled = cell != null;

            if (cell == null)
            {
                this.comboBoxLanguage.Text = string.Empty;
                this.comboBoxTable.Text = string.Empty;
                this.comboBoxColumn.Text = string.Empty;
                this.numericUpDownRow.Value = 0;
                this.textBoxValue.Text = string.Empty;
                return;
            }

            this.comboBoxLanguage.Text = cell.LanguageNumber.ToString(CultureInfo.InvariantCulture);
            this.comboBoxTable.Text = cell.TableNumber.ToString(CultureInfo.InvariantCulture);
            this.comboBoxColumn.Text = cell.ColumnNumber.ToString(CultureInfo.InvariantCulture);
            this.numericUpDownRow.Value = cell.RowNumber;
            this.textBoxValue.Text = cell.Value;
        }

        private void ComboBoxLanguageTextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comboBoxLanguage.Text) || this.SelectedCell == null)
            {
                return;
            }

            int language;
            if (!int.TryParse(this.comboBoxLanguage.Text, out language))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            this.SelectedCell.LanguageNumber = language;
            this.listBoxCells.Invalidate();
        }

        private void ComboBoxTableTextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comboBoxTable.Text) || this.SelectedCell == null)
            {
                return;
            }

            int table;
            if (!int.TryParse(this.comboBoxTable.Text, out table))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            this.SelectedCell.TableNumber = table;
            this.listBoxCells.Invalidate();
        }

        private void ComboBoxColumnTextUpdate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comboBoxColumn.Text) || this.SelectedCell == null)
            {
                return;
            }

            int column;
            if (!int.TryParse(this.comboBoxColumn.Text, out column))
            {
                SystemSounds.Exclamation.Play();
                return;
            }

            this.SelectedCell.ColumnNumber = column;
            this.listBoxCells.Invalidate();
        }

        private void NumericUpDownRowValueChanged(object sender, EventArgs e)
        {
            if (this.SelectedCell == null)
            {
                return;
            }

            this.SelectedCell.RowNumber = (int)this.numericUpDownRow.Value;
            this.listBoxCells.Invalidate();
        }

        private void TextBoxValueTextChanged(object sender, EventArgs e)
        {
            if (this.SelectedCell == null)
            {
                return;
            }

            this.SelectedCell.Value = this.textBoxValue.Text;
            this.listBoxCells.Invalidate();
        }

        private void ButtonSendClick(object sender, EventArgs e)
        {
            var ximple = new Ximple();
            foreach (XimpleCell cell in this.listBoxCells.Items)
            {
                ximple.Cells.Add(cell);
            }

            MessageDispatcher.Instance.Broadcast(ximple);
        }
    }
}
