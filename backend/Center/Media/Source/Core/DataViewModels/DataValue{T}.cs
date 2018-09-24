// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataValue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;
    using System.Diagnostics;

    using Gorba.Center.Common.Wpf.Core.Validation;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// Defines a wrapper around a value to be used within data view model properties.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    [DebuggerDisplay("{Value}, IsDirty")]
    public class DataValue<T> : ValidationViewModelBase, IDataValue, IDirty
    {
        private bool isDirty;

        private T value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public DataValue(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataValue&lt;T&gt;"/> class.
        /// </summary>
        public DataValue()
        {
            if (typeof(T) == typeof(string))
            {
                this.value = (T)Activator.CreateInstance(typeof(string), '\0', 0);
                return;
            }

            this.value = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            private set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.SetProperty(ref this.value, value, () => this.Value);
                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the value as <see cref="object" />.
        /// </summary>
        public object ValueObject
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == null)
                {
                    this.Value = default(T);
                }
                else if (this.value == null)
                {
                    this.Value = (T)value;
                }
                else
                {
                    var newValue = Convert.ChangeType(value, this.value.GetType());
                    this.Value = (T)newValue;
                }

                this.RaisePropertyChanged(() => this.ValueObject);
            }
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> flag. The default behavior only sets the flag on the current object.
        /// </summary>
        public void MakeDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag. The default behavior clears the flag on the current object and all
        /// its children.
        /// </summary>
        public virtual void ClearDirty()
        {
            this.IsDirty = false;
        }

        /// <summary>
        /// Clones the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public virtual object Clone()
        {
            return new DataValue<T> { Value = this.value };
        }

        /// <summary>
        /// Compares the current object with another.
        /// </summary>
        /// <param name="that">the other data Value</param>
        /// <returns>
        /// true if they are equal
        /// </returns>
        public virtual bool EqualsValue(DataValue<T> that)
        {
            return this.Value != null && this.Value.Equals(that.Value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            // Nothing to dispose here
        }
    }
}