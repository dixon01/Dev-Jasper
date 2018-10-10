// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitDetailsControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitDetailsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Gorba.Motion.Update.UsbUpdateManager.Data;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The control that contains all details about a single Unit.
    /// </summary>
    public partial class UnitDetailsControl : UserControl
    {
        private readonly IProjectManager projectManager;

        private Unit unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDetailsControl"/> class.
        /// </summary>
        public UnitDetailsControl()
        {
            this.InitializeComponent();

            this.listBoxLogFiles.DisplayMember = "Name";

            try
            {
                this.projectManager = ServiceLocator.Current.GetInstance<IProjectManager>();
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Gets or sets the Unit for which the information has to be shown.
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
            this.textBoxName.Text = this.Unit == null ? string.Empty : this.Unit.Name;
            this.textBoxDescription.Text = this.Unit == null ? string.Empty : this.Unit.Description;

            this.listBoxLogFiles.Items.Clear();

            if (this.Unit != null)
            {
                foreach (var logFile in this.projectManager.GetLogFilesFor(this.Unit))
                {
                    this.listBoxLogFiles.Items.Add(new FileInfo(logFile));
                }
            }

            if (this.Unit == null || this.Unit.Updates.Count == 0)
            {
                this.textBoxLastUpdateName.Text = string.Empty;
                this.textBoxLastUpdateState.Text = string.Empty;
                this.toolTip.SetToolTip(this.textBoxLastUpdateState, string.Empty);
                this.buttonDetails.Enabled = false;
                return;
            }

            var update = this.Unit.Updates[this.Unit.Updates.Count - 1];
            this.textBoxLastUpdateName.Text = update.Name;
            this.textBoxLastUpdateState.Text = update.CurrentState.State.ToString();
            this.toolTip.SetToolTip(
                this.textBoxLastUpdateState, update.CurrentState.TimeStamp.ToLocalTime().ToString("F"));
            this.buttonDetails.Enabled = true;
        }

        private void TextBoxDescriptionLeave(object sender, EventArgs e)
        {
            if (this.unit == null || this.unit.Description == this.textBoxDescription.Text)
            {
                return;
            }

            this.unit.Description = this.textBoxDescription.Text;
            ServiceLocator.Current.GetInstance<IProjectManager>().Save();
        }

        private void ButtonDetailsClick(object sender, EventArgs e)
        {
            var details = new UnitUpdateDetailsDialog();
            details.Unit = this.Unit;
            details.ShowDialog(this);
        }

        private void ListBoxLogFilesDoubleClick(object sender, EventArgs e)
        {
            var selectedFile = this.listBoxLogFiles.SelectedItem as FileInfo;
            if (selectedFile == null)
            {
                return;
            }

            Process.Start("Notepad.exe", selectedFile.FullName);
        }
    }
}
