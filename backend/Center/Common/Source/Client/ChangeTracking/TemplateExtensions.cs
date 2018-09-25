// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceModelExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Client.ChangeTracking
{
    using System.Data.Entity.Design.PluralizationServices;
    using System.Linq;

    /// <summary>
    /// Extension methods used for templates.
    /// </summary>
    public static class TemplateExtensions
    {
        private static readonly PluralizationService Pluralizer =
        PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo("en-us"));

        /// <summary>
        /// Gets the last part of the dotted name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The last part of the name.
        /// </returns>
        public static string GetLastPart(this string name)
        {
            return name.Split('.').Last();
        }

        /// <summary>
        /// Gets the containing partition for the specified property.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property.
        /// </param>
        /// <param name="entityPartitionName">
        /// The name of the partition containing the entity the property belongs to.
        /// </param>
        /// <returns>
        /// A substring with everything before the '.', if any; otherwise, the specified entity partition.
        /// </returns>
        public static string GetPropertyPartition(this string propertyName, string entityPartitionName)
        {
            var indexOfDot = propertyName.IndexOf('.');
            if (indexOfDot >= 0)
            {
                var parts = propertyName.Split('.');
                return parts.Take(parts.Length - 1).Aggregate((s, s1) => s + '.' + s1);
            }

            return entityPartitionName;
        }

        /// <summary>
        /// Gets the containing partition followed by the given suffix, if the partition exists.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>
        /// The containing partition followed by the given suffix, if the partition exists; empty string otherwise.
        /// </returns>
        public static string GetContainingPartition(this string name, string suffix = ".")
        {
            var split = name.Split('.');
            if (split.Length > 1)
            {
                return split.Take(split.Length - 1).Aggregate((s, s1) => s + "." + s1) + suffix;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the plural of the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The plural of the given name.
        /// </returns>
        public static string Pluralize(this string name)
        {
            return Pluralizer.Pluralize(name);
        }

        /// <summary>
        /// Gets the name for a value property.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The type of the property.
        /// </returns>
        public static string GetPropertyValueName(this string type)
        {
            switch (type)
            {
                case "bool":
                case "Boolean":
                case "System.Boolean":
                    return "Boolean";
                case "string":
                case "System.String":
                    return "String";
                case "DateTime?":
                case "System.Nullable<System.DateTime>":
                    return "NullableDateTime";
                case "DateTime":
                case "System.DateTime":
                    return "DateTime";
                case "int":
                case "Int32":
                case "System.Int32":
                    return "Int32";
                case "long":
                case "Int64":
                case "System.Int64":
                    return "Int64";
                case "Guid":
                case "System.Guid":
                    return "Guid";
                default:
                    var parts = type.Split('.');
                    return parts.Last();
            }
        }

        /// <summary>
        /// Gets a valid field name.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>A field name.</returns>
        public static string GetFieldName(this string name)
        {
            return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
        }

        /// <summary>
        /// Gets a valid variable name.
        /// </summary>
        /// <param name="name">The name to transform.</param>
        /// <returns>A field name.</returns>
        public static string GetVariableName(this string name)
        {
            return name.Substring(0, 1).ToLowerInvariant() + name.Substring(1);
        }
    }
}