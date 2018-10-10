// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitUpdateDetailsControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitUpdateDetailsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Control showing all details about all updates for a certain unit.
    /// </summary>
    public partial class UnitUpdateDetailsControl : UserControl
    {
        private Unit unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitUpdateDetailsControl"/> class.
        /// </summary>
        public UnitUpdateDetailsControl()
        {
            this.InitializeComponent();

            var timeStampColumn = new DataGridViewTextBoxColumn
                                      {
                                          DataPropertyName = "TimeStamp",
                                          HeaderText = "Time Stamp"
                                      };
            timeStampColumn.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss.f";
            this.dataGridViewHistory.Columns.Add(timeStampColumn);
            this.dataGridViewHistory.Columns.Add(
                new DataGridViewTextBoxColumn { DataPropertyName = "State", HeaderText = "State" });
            this.dataGridViewHistory.Columns.Add(
                new DataGridViewTextBoxColumn { DataPropertyName = "ErrorReason", HeaderText = "Error Reason" });
            this.dataGridViewHistory.AutoGenerateColumns = false;

            this.LoadData();
        }

        /// <summary>
        /// Gets or sets the unit for which the information should be displayed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Unit Unit
        {
            get
            {
                return this.unit;
            }

            set
            {
                this.unit = value;

                this.LoadData();
            }
        }

        private void LoadData()
        {
            this.listBoxUpdates.Items.Clear();
            if (this.Unit == null)
            {
                this.LoadSelectedUpdate();
                return;
            }

            for (int i = this.Unit.Updates.Count - 1; i >= 0; i--)
            {
                var update = this.Unit.Updates[i];
                this.listBoxUpdates.Items.Add(update);
            }

            if (this.listBoxUpdates.Items.Count > 0)
            {
                this.listBoxUpdates.SelectedIndex = 0;
            }
            else
            {
                this.LoadSelectedUpdate();
            }
        }

        private void LoadSelectedUpdate()
        {
            if (this.listBoxUpdates.SelectedItem == null || !(this.listBoxUpdates.SelectedItem is UpdateInfo))
            {
                this.updateFolderStructureControl.UpdateCommand = null;
                this.preInstallationActionsControl.RunCommands = null;
                this.postInstallationActionsControl.RunCommands = null;
                return;
            }

            var update = (UpdateInfo)this.listBoxUpdates.SelectedItem;
            this.textBoxValidFrom.Text = update.Command.ActivateTime.ToLocalTime().ToString("dd.MM.yyyy hh:mm:ss");
            this.textBoxState.Text = update.CurrentState.State.ToString();
            this.textBoxTimestamp.Text = update.CurrentState.TimeStamp.ToLocalTime().ToString("F");

            this.dataGridViewHistory.DataSource = update.States;
            this.updateFolderStructureControl.UpdateCommand = update.Command;
            this.preInstallationActionsControl.RunCommands = update.Command.PreInstallation;
            this.postInstallationActionsControl.RunCommands = update.Command.PostInstallation;
        }

        private void ListBoxUpdatesDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            var update = (UpdateInfo)this.listBoxUpdates.Items[e.Index];

            var stateBounds = e.Bounds;
            stateBounds.Width = stateBounds.Height;

            Brush stateBrush;
            switch (update.CurrentState.State)
            {
                case UpdateState.Created:
                case UpdateState.Transferring:
                case UpdateState.Transferred:
                case UpdateState.Installing:
                    stateBrush = Brushes.DarkGray;
                    break;
                case UpdateState.Installed:
                    stateBrush = Brushes.LimeGreen;
                    break;
                case UpdateState.Ignored:
                case UpdateState.PartiallyInstalled:
                    stateBrush = Brushes.Orange;
                    break;
                case UpdateState.TransferFailed:
                case UpdateState.InstallationFailed:
                    stateBrush = Brushes.Red;
                    break;
                default:
                    stateBrush = Brushes.White;
                    break;
            }

            var stateCircle = stateBounds;
            stateCircle.Inflate(-2, -2);
            e.Graphics.FillEllipse(stateBrush, stateCircle);
            e.Graphics.DrawEllipse(Pens.Black, stateCircle);

            var textBounds = e.Bounds;
            textBounds.Width -= stateBounds.Width;
            textBounds.X += stateBounds.Width;
            e.Graphics.DrawString(
                update.Name,
                this.listBoxUpdates.Font,
                (e.State & DrawItemState.Selected) == 0 ? SystemBrushes.ControlText : SystemBrushes.HighlightText,
                textBounds,
                StringFormat.GenericDefault);

            e.DrawFocusRectangle();
        }

        private void ListBoxUpdatesSelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadSelectedUpdate();
        }

        private void DataGridViewHistoryCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!(e.Value is DateTime))
            {
                return;
            }

            var value = (DateTime)e.Value;
            e.Value = value.ToLocalTime().ToString(e.CellStyle.Format, e.CellStyle.FormatProvider);
            e.FormattingApplied = true;
        }
    }
}
