// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitUpdateDetailsDialog.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitUpdateDetailsDialog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Motion.Update.UsbUpdateManager.Data;

    /// <summary>
    /// Dialog showing all details about all updates for a certain unit.
    /// </summary>
    public partial class UnitUpdateDetailsDialog : Form
    {
        private Unit unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitUpdateDetailsDialog"/> class.
        /// </summary>
        public UnitUpdateDetailsDialog()
        {
            this.InitializeComponent();
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
            this.unitUpdateDetailsControl.Unit = this.Unit;
            if (this.Unit == null)
            {
                this.Text = "All Updates";
                return;
            }

            this.Text = string.Format("All Updates of {0}", this.Unit.Name);
        }
    }
}
