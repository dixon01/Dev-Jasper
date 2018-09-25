// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataErrorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataErrorViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// View model base class that implements <see cref="INotifyDataErrorInfo"/>.
    /// </summary>
    public abstract class DataErrorViewModelBase : DirtyViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<ErrorItem>> errors = new Dictionary<string, List<ErrorItem>>();

        /// <summary>
        /// The errors changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets a value indicating whether has errors.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return this.GetErrors(null) != null;
            }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// This method only returns the errors marked as <see cref="ErrorState.Error"/>.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for;
        /// or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.
        /// </param>
        public IEnumerable GetErrors(string propertyName)
        {
            var allErrors = this.GetAllErrors(propertyName).Where(e => e.State == ErrorState.Error).ToList();
            return allErrors.Count == 0 ? null : allErrors;
        }

        /// <summary>
        /// Gets all validation errors for a specified property or for the entire entity.
        /// This method returns all errors marked as <see cref="ErrorState.Error"/>,
        /// <see cref="ErrorState.Warning"/> or <see cref="ErrorState.Missing"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for;
        /// or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.
        /// </param>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        public IList<ErrorItem> GetAllErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return this.errors.Values.SelectMany(e => e).ToList();
            }

            List<ErrorItem> propertyErrors;
            if (this.errors.TryGetValue(propertyName, out propertyErrors) && propertyErrors.Count > 0)
            {
                // another ToList to clone the list
                return this.errors[propertyName].ToList();
            }

            return new ErrorItem[0];
        }

        /// <summary>
        /// Sets or removes the error of a given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="state">
        /// The error state. If this is set to <see cref="ErrorState.Ok"/>, the error is cleared instead of set.
        /// </param>
        /// <param name="message">
        /// The human readable error message.
        /// </param>
        public void SetError(string propertyName, ErrorState state, string message)
        {
            if (state == ErrorState.Ok)
            {
                this.RemoveError(propertyName, message);
            }
            else
            {
                this.SetError(propertyName, new ErrorItem(state, message));
            }
        }

        /// <summary>
        /// Sets the error of a given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SetError(string propertyName, ErrorItem error)
        {
            List<ErrorItem> propertyErrors;
            if (!this.errors.TryGetValue(propertyName, out propertyErrors))
            {
                this.errors.Add(propertyName, propertyErrors = new List<ErrorItem>());
            }
            else
            {
                var found = propertyErrors.Find(e => e.Message == error.Message);
                if (found != null)
                {
                    if (found.State == error.State)
                    {
                        return;
                    }

                    propertyErrors.Remove(found);
                }
            }

            propertyErrors.Add(error);
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Removes the error of a given <paramref name="propertyName"/>.
        /// This method removes all error independent of their <see cref="ErrorItem.State"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="errorMessage">
        /// The human readable error message.
        /// </param>
        /// <returns>
        /// True if the error was removed, otherwise false.
        /// </returns>
        public bool RemoveError(string propertyName, string errorMessage)
        {
            List<ErrorItem> propertyErrors;
            if (!this.errors.TryGetValue(propertyName, out propertyErrors))
            {
                return false;
            }

            if (propertyErrors.RemoveAll(e => e.Message == errorMessage) == 0)
            {
                return false;
            }

            this.RaiseErrorsChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Removes the error of a given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// True if the error was removed, otherwise false.
        /// </returns>
        public bool RemoveError(string propertyName, ErrorItem error)
        {
            List<ErrorItem> propertyErrors;
            if (!this.errors.TryGetValue(propertyName, out propertyErrors))
            {
                return false;
            }

            if (!propertyErrors.Remove(error))
            {
                return false;
            }

            this.RaiseErrorsChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Clears all errors of a given <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public void ClearErrors(string propertyName)
        {
            List<ErrorItem> propertyErrors;
            if (!this.errors.TryGetValue(propertyName, out propertyErrors))
            {
                return;
            }

            propertyErrors.Clear();
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clears all errors of this view model.
        /// </summary>
        public void ClearErrors()
        {
            foreach (var property in this.errors)
            {
                property.Value.Clear();
                this.RaiseErrorsChanged(property.Key);
            }
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