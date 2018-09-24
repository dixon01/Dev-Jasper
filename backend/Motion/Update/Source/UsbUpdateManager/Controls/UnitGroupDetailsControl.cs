// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitGroupDetailsControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitGroupDetailsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.UsbUpdateManager.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Motion.Update.UsbUpdateManager.Data;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The control that contains all details about a single Unit Group.
    /// </summary>
    public partial class UnitGroupDetailsControl : UserControl
    {
        private UnitGroup unitGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitGroupDetailsControl"/> class.
        /// </summary>
        public UnitGroupDetailsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the Unit Group for which the information has to be shown.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UnitGroup UnitGroup
        {
            get
            {
                return this.unitGroup;
            }

            set
            {
                this.unitGroup = value;
                this.textBoxName.Text = value == null ? string.Empty : value.Name;
                this.textBoxDescription.Text = value == null ? string.Empty : value.Description;
            }
        }

        private void TextBoxDescriptionLeave(object sender, EventArgs e)
        {
            if (this.unitGroup == null || this.unitGroup.Description == this.textBoxDescription.Text)
            {
                return;
            }

            this.unitGroup.Description = this.textBoxDescription.Text;
            ServiceLocator.Current.GetInstance<IProjectManager>().Save();
        }
    }
}
