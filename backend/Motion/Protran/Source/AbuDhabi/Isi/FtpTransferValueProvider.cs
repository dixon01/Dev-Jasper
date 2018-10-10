// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FtpTransferValueProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// Value provider about the FTP downloading status.
    /// </summary>
    public class FtpTransferValueProvider : IDataItemValueProvider
    {
        private bool value;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpTransferValueProvider"/> class.
        /// </summary>
        public FtpTransferValueProvider()
        {
            this.value = false;
        }

        /// <summary>
        /// This event is fired every time the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets a value indicating whether FTP transfer is running.
        /// </summary>
        public bool Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value == value)
                {
                    return;
                }

                this.value = value;
                this.OnValueChanged(EventArgs.Empty);
            }
        }

        string IDataItemValueProvider.Value
        {
            get
            {
                return this.Value ? "1" : "0";
            }
        }

        /// <summary>
        /// Fires the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
