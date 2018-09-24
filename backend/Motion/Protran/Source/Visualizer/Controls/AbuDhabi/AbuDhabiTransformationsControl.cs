// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiTransformationsControl.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiTransformationsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.AbuDhabi.Isi;
    using Gorba.Motion.Protran.Visualizer.Data.AbuDhabi;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Control that shows all transformations done for a IsiPut message.
    /// </summary>
    public partial class AbuDhabiTransformationsControl : UserControl
    {
        private readonly IAbuDhabiVisualizationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiTransformationsControl"/> class.
        /// </summary>
        public AbuDhabiTransformationsControl()
        {
            this.InitializeComponent();

            try
            {
                this.service = ServiceLocator.Current.GetInstance<IAbuDhabiVisualizationService>();
                this.service.IsiMessageEnqueued += this.ServiceOnIsiMessageEnqueued;
                this.service.DataItemTransformed += this.ServiceOnDataItemTransformed;
            }
            catch
            {
                // ignore, this only happens in design mode
            }
        }

        private void ServiceOnIsiMessageEnqueued(object sender, IsiMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<IsiMessageEventArgs>(this.ServiceOnIsiMessageEnqueued), sender, e);
                return;
            }

            this.tabControl1.TabPages.Clear();
        }

        private void ServiceOnDataItemTransformed(object sender, DataItemTransformationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<DataItemTransformationEventArgs>(this.ServiceOnDataItemTransformed), sender, e);
                return;
            }

            var transformCtrl = new TransformationControl();
            transformCtrl.Populate(e);

            var page = new TabPage(e.DataItem.Name);
            page.Controls.Add(transformCtrl);
            this.tabControl1.TabPages.Add(page);
        }
    }
}
