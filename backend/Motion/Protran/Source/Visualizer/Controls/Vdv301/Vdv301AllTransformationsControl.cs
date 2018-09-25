// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv301AllTransformationsControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv301TransformationsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.Vdv301
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Protran.Visualizer.Data;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    /// <summary>
    /// The VDV 301 control that shows all transformations.
    /// </summary>
    public partial class Vdv301AllTransformationsControl : UserControl, IVdv301VisualizationControl
    {
        private readonly BindingList<TransformationChainEventArgs> transformations =
            new BindingList<TransformationChainEventArgs>();

        private SideTab sideTab;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv301AllTransformationsControl"/> class.
        /// </summary>
        public Vdv301AllTransformationsControl()
        {
            this.InitializeComponent();

            this.listBoxTransformations.DataSource = this.transformations;
        }

        /// <summary>
        /// Assigns a <see cref="SideTab"/> to this control.
        /// This can be used to keep a reference to the tab
        /// and update it when events arrive.
        /// </summary>
        /// <param name="tab">
        /// The side tab.
        /// </param>
        public void SetSideTab(SideTab tab)
        {
            this.sideTab = tab;
        }

        /// <summary>
        /// Configure the control with the given service.
        /// </summary>
        /// <param name="service">
        /// The service to which you can for example attach event handlers
        /// </param>
        public void Configure(IVdv301VisualizationService service)
        {
            service.DataUpdated += this.ServiceOnDataUpdated;
            service.ElementTransformed += this.OnElementTransformed;
        }

        private void ServiceOnDataUpdated(object sender, DataUpdateEventArgs<object> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<DataUpdateEventArgs<object>>(this.ServiceOnDataUpdated), sender, e);
                return;
            }

            this.transformations.Clear();
        }

        private void OnElementTransformed(object sender, TransformationChainEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<TransformationChainEventArgs>(this.OnElementTransformed), sender, e);
                return;
            }

            this.transformations.Add(e);
        }

        private void ListBoxTransformationsOnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.vdv301TransformationControl.Populate(
                this.listBoxTransformations.SelectedItem as TransformationChainEventArgs);
        }
    }
}
