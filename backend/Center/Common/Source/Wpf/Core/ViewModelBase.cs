// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the basic view model.
    /// </summary>
    [DataContract]
    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the cloned from.
        /// </summary>
        public int ClonedFrom { get; set; }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected virtual void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            this.PropertyChanged.RaisePropertyChangedEvent(this, expression);
        }

        /// <summary>
        /// Tries to set the back field of a property to the given <paramref name="value"/>.
        /// The field is not changed if its value is already equal to the desired one (object.Equals used
        /// for comparison).
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="field">The reference to the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value was set; otherwise, <c>false</c>.</returns>
        protected virtual bool TrySetPropertyBackValue<T>(ref T field, T value)
        {
            if (object.Equals(field, value))
            {
                return false;
            }

            field = value;
            return true;
        }

        /// <summary>
        /// Sets the field to the desired value if it is different than the current one. If set, the
        /// <see cref="PropertyChanged"/> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="expression">The expression resolving the property.</param>
        /// <returns><c>true</c> if the value was changed; otherwise, <c>false</c>.</returns>
        protected virtual bool SetProperty<T>(ref T field, T value, Expression<Func<object>> expression)
        {
            if (this.TrySetPropertyBackValue(ref field, value))
            {
                this.RaisePropertyChanged(expression);
                return true;
            }

            return false;
        }
    }
}