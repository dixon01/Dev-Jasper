// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ButtonControl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ButtonControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.IO
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Visualizer.Data.IO;

    /// <summary>
    /// The control that handles button creation and sending button status.
    /// </summary>
    public partial class ButtonControl : UserControl
    {
        private IIOVisualizationService controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonControl"/> class.
        /// </summary>
        public ButtonControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Configure(IIOVisualizationService control)
        {
            this.controller = control;
            if (this.controller.Config.SerialPorts != null)
            {
                int x = 0;
                int y = 0;
                foreach (var input in this.controller.Config.Inputs)
                {
                    var inputButton = new CheckBox();
                    inputButton.Appearance = Appearance.Button;
                    inputButton.Name = string.Format("{0}{1}", "button", input.Name);
                    inputButton.Text = input.Name;
                    inputButton.Location = new Point(x + 20, y + 20);
                    y = 50;
                    inputButton.Width = 110;
                    inputButton.BringToFront();
                    this.Controls.Add(inputButton);
                    inputButton.CheckedChanged += this.CheckBoxCheckChanged;
                }
            }
        }

        private void CheckBoxCheckChanged(object sender, EventArgs e)
        {
            var button = (CheckBox)sender;
            var config = this.controller.Config.Inputs.Find(c => c.Name == button.Text);
            this.controller.SendPinChangedEvent(config, button.Checked);
        }
    }
}
