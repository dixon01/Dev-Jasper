// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisUI.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisUI type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MyHyperTerminal
{
    using System;
    using System.Windows.Forms;

    using Gorba.Motion.Protran.Controls;

    /// <summary>
    /// The graphical panel to manage the IBIS telegrams.
    /// </summary>
    public partial class IbisUi : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisUi"/> class.
        /// </summary>
        public IbisUi()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event that is fired every time a new ibis telegram is created
        /// </summary>
        public event EventHandler<DataEventArgs> IbisTelegramCreated;

        private void TelegramCreationControl1IbisTelegramCreated(object sender, DataEventArgs e)
        {
            var handler = this.IbisTelegramCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}