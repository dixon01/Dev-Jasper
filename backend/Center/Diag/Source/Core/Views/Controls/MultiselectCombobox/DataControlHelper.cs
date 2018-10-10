// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataControlHelper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataControlHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls.MultiselectCombobox
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The data control helper.
    /// </summary>
    public static class DataControlHelper
    {
        /// <summary>
        /// The get property info.
        /// </summary>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        /// <param name="bindingPath">
        /// The binding path.
        /// </param>
        /// <returns>
        /// The <see cref="PropertyInfo"/>.
        /// </returns>
        public static PropertyInfo GetPropertyInfo(Type objectType, string bindingPath)
        {
            if (bindingPath == null)
            {
                return null;
            }

            PropertyInfo propertyInfo = null;
            foreach (var path in bindingPath.Split('.'))
            {
                propertyInfo = objectType.GetProperty(path);
                if (propertyInfo == null)
                {
                    break;
                }

                objectType = propertyInfo.PropertyType;
            }

            return propertyInfo;
        }
    }
}
