// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemsControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Motion.Protran.AbuDhabi.Isi;
    using Gorba.Motion.Protran.Visualizer.Data.AbuDhabi;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Control that shows data items from ISI in tabs.
    /// </summary>
    public partial class DataItemsControl : UserControl
    {
        private readonly IAbuDhabiVisualizationService service;

        private bool updatingCheckAll;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemsControl"/> class.
        /// </summary>
        public DataItemsControl()
        {
            this.InitializeComponent();

            try
            {
                this.service = ServiceLocator.Current.GetInstance<IAbuDhabiVisualizationService>();
                this.service.IsiMessageSent += this.ServiceOnIsiMessageSent;
            }
            catch
            {
                // ignore, this only happens in design mode
            }
        }

        private void ServiceOnIsiMessageSent(object sender, IsiMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<IsiMessageEventArgs>(this.ServiceOnIsiMessageSent), sender, e);
                return;
            }

            var put = e.IsiMessage as IsiPut;
            if (put != null)
            {
                this.OnIsiPutSent(put);
                return;
            }

            var get = e.IsiMessage as IsiGet;
            if (get != null)
            {
                this.OnIsiGetSent(get);
            }
        }

        private void OnIsiPutSent(IsiPut put)
        {
            this.dataGridView1.DataSource = put.Items;
        }

        private void OnIsiGetSent(IsiGet get)
        {
            var page = new TabPage("<IsiPut> for " + get.IsiId);
            var control = new IsiResponseControl { Request = get, Dock = DockStyle.Fill };
            page.Controls.Add(control);
            this.tabControl1.TabPages.Add(page);
        }

        private void ButtonRequestClick(object sender, EventArgs e)
        {
            var get = new IsiGet { Items = new DataItemRequestList() };
            foreach (string name in this.checkedListBox1.CheckedItems)
            {
                get.Items.Add(name);
            }

            this.service.EnqueueIsiMessage(get);
        }

        private void CheckedListBox1ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.updatingCheckAll)
            {
                return;
            }

            this.updatingCheckAll = true;
            int checkCount = this.checkedListBox1.CheckedItems.Count;
            if (e.NewValue == CheckState.Checked)
            {
                checkCount++;
            }
            else
            {
                checkCount--;
            }

            if (checkCount <= 0)
            {
                this.checkBoxAll.CheckState = CheckState.Unchecked;
                this.buttonRequest.Enabled = false;
            }
            else if (checkCount == this.checkedListBox1.Items.Count)
            {
                this.checkBoxAll.CheckState = CheckState.Checked;
                this.buttonRequest.Enabled = true;
            }
            else
            {
                this.checkBoxAll.CheckState = CheckState.Indeterminate;
                this.buttonRequest.Enabled = true;
            }

            this.updatingCheckAll = false;
        }

        private void CheckBoxAllCheckStateChanged(object sender, EventArgs e)
        {
            if (this.updatingCheckAll)
            {
                return;
            }

            this.updatingCheckAll = true;

            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemChecked(i, this.checkBoxAll.Checked);
            }

            this.buttonRequest.Enabled = this.checkBoxAll.Checked;
            this.updatingCheckAll = false;
        }
    }
}
