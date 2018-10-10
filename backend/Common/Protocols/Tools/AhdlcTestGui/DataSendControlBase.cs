// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSendControlBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataSendControlBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Tools.AhdlcTestGui
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Base class for controls that can send data to a sign.
    /// </summary>
    public partial class DataSendControlBase : UserControl
    {
        private IDataSendContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSendControlBase"/> class.
        /// </summary>
        public DataSendControlBase()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data send context.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDataSendContext Context
        {
            get
            {
                return this.context;
            }

            set
            {
                if (this.context != null)
                {
                    this.context.Updated -= this.ContextOnUpdated;
                }

                this.context = value;
                if (this.context != null)
                {
                    this.context.Updated += this.ContextOnUpdated;
                    this.UpdateFromContext();
                }
            }
        }

        /// <summary>
        /// This method can be overridden by subclasses to update themselves when
        /// the <see cref="Context"/> changed or was updated.
        /// </summary>
        protected virtual void UpdateFromContext()
        {
        }

        /// <summary>
        /// This method can be overridden by subclasses to create all necessary frames to send the data to the control.
        /// </summary>
        /// <returns>
        /// The frames to be sent to the sign; can't be null but might be empty.
        /// The frames' <see cref="FrameBase.Address"/> doesn't need to be correct,
        /// it will be updated by the calling method.
        /// </returns>
        protected virtual IEnumerable<LongFrameBase> CreateFrames()
        {
            yield break;
        }

        private void ContextOnUpdated(object sender, EventArgs eventArgs)
        {
            this.UpdateFromContext();
        }

        private void ButtonSendClick(object sender, System.EventArgs e)
        {
            if (this.Context == null)
            {
                return;
            }

            foreach (var frame in this.CreateFrames())
            {
                this.Context.SendFrame(frame);
            }
        }
    }
}
