// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiResponseControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiResponseControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Motion.Protran.Visualizer.Data.AbuDhabi;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Control that shows an IsiGet request and provides
    /// the possibility to send a corresponding IsiPut.
    /// </summary>
    public partial class IsiResponseControl : UserControl
    {
        private readonly IsiPut response = new IsiPut();

        private IsiGet request;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsiResponseControl"/> class.
        /// </summary>
        public IsiResponseControl()
        {
            this.InitializeComponent();

            var binding = new BindingSource { DataSource = this.response.Items };
            this.dataGridView1.DataSource = binding;
        }

        /// <summary>
        /// Gets or sets the IsiGet request to be shown in this control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IsiGet Request
        {
            get
            {
                return this.request;
            }

            set
            {
                this.request = value;
                this.response.IsiId = value.IsiId;
                this.response.Items.Clear();
                foreach (var name in value.Items)
                {
                    this.response.Items.Add(new DataItem { Name = name, Value = string.Empty });
                }

                this.dataGridView1.DataSource = this.response.Items;
            }
        }

        private void ButtonSendClick(object sender, System.EventArgs e)
        {
            var service = ServiceLocator.Current.GetInstance<IAbuDhabiVisualizationService>();
            service.EnqueueIsiMessage(this.response);
        }
    }
}
