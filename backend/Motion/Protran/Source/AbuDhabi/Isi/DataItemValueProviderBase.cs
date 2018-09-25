// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemValueProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemValueProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    /// <summary>
    /// Provider base calss for data item values.
    /// </summary>
    public abstract class DataItemValueProviderBase : IDataItemValueProvider
    {
        private string value;

        /// <summary>
        /// This event is actually never fired in this class.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets the value from this provider.
        /// </summary>
        public virtual string Value
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

        /// <summary>
        /// Fires the <see cref="StaticDataItemValueProvider.ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            var handler = this.ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}