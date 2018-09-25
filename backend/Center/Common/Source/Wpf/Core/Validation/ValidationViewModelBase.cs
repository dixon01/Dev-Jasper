// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ValidationViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// Defines the base for view models that need validation.
    /// </summary>
    public abstract class ValidationViewModelBase : ViewModelBase, IDataErrorInfo
    {
        private readonly Lazy<IDictionary<string, ValidationInfo>> validationInfo;

        private readonly Lazy<IEnumerable<PropertyInfo>> allProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationViewModelBase"/> class.
        /// </summary>
        protected ValidationViewModelBase()
        {
            this.allProperties = new Lazy<IEnumerable<PropertyInfo>>(this.GetAllProperties);
            this.validationInfo = new Lazy<IDictionary<string, ValidationInfo>>(this.GetValidationInfo);
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        string IDataErrorInfo.Error
        {
            get
            {
                var propertiesWithErrors =
                    this.allProperties.Value.OrderBy(property => property.Name)
                        .Where(this.PropertyHasErrors)
                        .Select(property => property.Name);
                return string.Join(", ", propertiesWithErrors);
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                var validationResult = this.Validate(propertyName);
                var result = string.Empty;
                if (validationResult != null)
                {
                    result = string.Join(Environment.NewLine, validationResult);
                }

                return result;
            }
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValid()
        {
            return string.IsNullOrEmpty(((IDataErrorInfo)this).Error);
        }

        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public virtual string IsValid(string propertyName, object value)
        {
            return string.Empty;
        }

        /// <summary>
        /// Validates the property with the specified name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The list of error messages for the given properties. Empty enumeration if no error was found.
        /// </returns>
        protected virtual IEnumerable<string> Validate(string propertyName)
        {
            if (this.validationInfo.Value.ContainsKey(propertyName))
            {
                var propertyValue = this.validationInfo.Value[propertyName].ValueGetter();
                var errorMessages = this.validationInfo.Value[propertyName]
                    .Attributes
                    .Where(v => !v.IsValid(propertyValue))
                    .Select(v => v.ErrorMessage)
                    .ToList();
                return errorMessages;
            }

            return Enumerable.Empty<string>();
        }

        private bool PropertyHasErrors(PropertyInfo property)
        {
            var validationResult = this.Validate(property.Name);
            return validationResult == null || validationResult.Any(s => s != string.Empty);
        }

        private IEnumerable<PropertyInfo> GetAllProperties()
        {
            return this.GetType().GetProperties();
        }

        private IDictionary<string, ValidationInfo> GetValidationInfo()
        {
            var query = from property in this.allProperties.Value
                        let validators =
                            property.GetCustomAttributes(typeof(ValidationAttribute), true)
                                    .OfType<ValidationAttribute>()
                                    .ToArray()
                                    where validators.Length > 0
                        select new ValidationInfo(property.Name, () => property.GetValue(this, null), validators);
            return query.ToDictionary(info => info.Key, info => info);
        }

        private class ValidationInfo
        {
            public ValidationInfo(string key, Func<object> valueGetter, ValidationAttribute[] attributes)
            {
                this.Key = key;
                this.ValueGetter = valueGetter;
                this.Attributes = attributes;
            }

            public ValidationAttribute[] Attributes { get; private set; }

            public string Key { get; private set; }

            public Func<object> ValueGetter { get; private set; }
        }
    }
}