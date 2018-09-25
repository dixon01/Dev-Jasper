// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportItemBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Export
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The base class for all items to be exported.
    /// </summary>
    public abstract class ExportItemBase : ViewModelBase, INotifyDataErrorInfo
    {
        private string name;

        private bool isDirty;

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the children.
        /// </summary>
        public abstract IEnumerable<ExportItemBase> ChildItems { get; }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is dirty.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return this.isDirty;
            }

            set
            {
                this.SetProperty(ref this.isDirty, value, () => this.IsDirty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item or its children have changes.
        /// </summary>
        public abstract bool HasChanges { get; }

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <returns>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </returns>
        public virtual bool HasErrors
        {
            get
            {
                var errors = this.GetErrors(null);
                return errors == null || !errors.GetEnumerator().MoveNext();
            }
        }

        /// <summary>
        /// Clears the <see cref="HasChanges"/> flag of this item and its children.
        /// </summary>
        public abstract void ClearHasChanges();

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>,
        /// to retrieve entity-level errors.
        /// </param>
        public abstract IEnumerable GetErrors(string propertyName);

        /// <summary>
        /// Gets the validation error items for a specified property or for the entire entity.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <returns>
        /// The validation error items for the property or entity.
        /// </returns>
        public IEnumerable<ErrorItem> GetErrorMessages(string propertyName)
        {
            return this.GetErrors(propertyName) as IEnumerable<ErrorItem> ?? Enumerable.Empty<ErrorItem>();
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseErrorsChanged(DataErrorsChangedEventArgs e)
        {
            var handler = this.ErrorsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
