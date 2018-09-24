// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// View model base class that implements <see cref="INotifyDataErrorInfo"/>.
    /// </summary>
    public class ErrorViewModelBase : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <returns>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </returns>
        public bool HasErrors
        {
            get
            {
                return this.GetErrors(null) != null;
            }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for;
        /// or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.
        /// </param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                var list = this.errors.Values.SelectMany(e => e).ToList();
                return list.Count == 0 ? null : list;
            }

            List<string> messages;
            this.errors.TryGetValue(propertyName, out messages);
            return messages;
        }

        /// <summary>
        /// Adds or removes an error for the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        /// <param name="enabled">
        /// A flag indicating if the error is enabled (true) and should therefore be added or removed (false).
        /// </param>
        public void ChangeError(string propertyName, string errorMessage, bool enabled)
        {
            if (enabled)
            {
                this.AddError(propertyName, errorMessage);
            }
            else
            {
                this.RemoveError(propertyName, errorMessage);
            }
        }

        /// <summary>
        /// Adds an error for the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        public void AddError(string propertyName, string errorMessage)
        {
            List<string> messages;
            if (!this.errors.TryGetValue(propertyName, out messages))
            {
                messages = new List<string>();
                this.errors[propertyName] = messages;
            }
            else if (messages.Contains(errorMessage))
            {
                return;
            }

            messages.Add(errorMessage);
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Removes an error for the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        /// <returns>
        /// True if the error was removed, otherwise false.
        /// </returns>
        public bool RemoveError(string propertyName, string errorMessage)
        {
            List<string> messages;
            if (!this.errors.TryGetValue(propertyName, out messages))
            {
                return false;
            }

            if (!messages.Remove(errorMessage))
            {
                return false;
            }

            this.RaiseErrorsChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        public void ClearErrors()
        {
            this.errors.Clear();
            this.RaiseErrorsChanged();
        }

        /// <summary>
        /// Clears the errors of the given property.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public void ClearErrors(string propertyName)
        {
            List<string> messages;
            if (!this.errors.TryGetValue(propertyName, out messages) || messages.Count == 0)
            {
                return;
            }

            messages.Clear();
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected void RaiseErrorsChanged(string propertyName = "")
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }

            this.RaisePropertyChanged(() => this.HasErrors);
        }
    }
}