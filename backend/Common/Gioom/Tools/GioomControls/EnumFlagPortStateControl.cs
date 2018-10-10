// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFlagPortStateControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumFlagPortStateControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Tools.Controls
{
    using System.Globalization;
    using System.Windows.Forms;

    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    /// <summary>
    /// The enum flag port state control.
    /// </summary>
    public partial class EnumFlagPortStateControl : PortStateControlBase
    {
        private bool updatingValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFlagPortStateControl"/> class.
        /// </summary>
        public EnumFlagPortStateControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this control is read-only.
        /// </summary>
        protected override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }

            set
            {
                base.ReadOnly = value;
                this.checkedListBox.Enabled = !value;
            }
        }

        /// <summary>
        /// Changes the currently used port to a new one.
        /// </summary>
        /// <param name="newPort">
        /// The new port.
        /// </param>
        protected override void ChangePort(IPort newPort)
        {
            var values = (EnumValues)newPort.Info.ValidValues;
            this.updatingValue = true;
            try
            {
                this.textBoxValue.Clear();
                this.checkedListBox.Items.Clear();

                foreach (var value in values.Values)
                {
                    this.checkedListBox.Items.Add(value);
                }
            }
            finally
            {
                this.updatingValue = false;
            }

            base.ChangePort(newPort);
        }

        /// <summary>
        /// Updates this control with the given value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        protected override void UpdateValue(IOValue value)
        {
            base.UpdateValue(value);

            this.updatingValue = true;
            try
            {
                this.textBoxValue.Text = value.Value.ToString(CultureInfo.InvariantCulture);
                for (int i = 0; i < this.checkedListBox.Items.Count; i++)
                {
                    var item = (IOValue)this.checkedListBox.Items[i];

                    if (item.Value == 0)
                    {
                        this.checkedListBox.SetItemChecked(i, value.Value == 0);
                    }
                    else
                    {
                        this.checkedListBox.SetItemChecked(i, (value.Value & item.Value) == item.Value);
                    }
                }
            }
            finally
            {
                this.updatingValue = false;
            }
        }

        private void CheckedListBoxOnItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.updatingValue || this.Port == null)
            {
                return;
            }

            var intValue = 0;
            for (int i = 0; i < this.checkedListBox.Items.Count; i++)
            {
                var item = (IOValue)this.checkedListBox.Items[i];
                if (e.Index == i)
                {
                    if (e.NewValue != CheckState.Checked)
                    {
                        continue;
                    }
                }
                else if (!this.checkedListBox.GetItemChecked(i))
                {
                    continue;
                }

                if (item.Value != 0)
                {
                    intValue |= item.Value;
                }
            }

            var value = this.Port.CreateValue(intValue);

            this.Port.Value = value;
            this.textBoxValue.Text = intValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
