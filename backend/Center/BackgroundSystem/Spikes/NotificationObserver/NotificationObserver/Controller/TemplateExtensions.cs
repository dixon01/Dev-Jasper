// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplateExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.NotificationObserver.Controller
{
    using System.Data.Entity.Design.PluralizationServices;
    using System.Linq;

    /// <summary>
    /// Defines extensions used for templates.
    /// </summary>
    public static class TemplateExtensions
    {
        private static readonly PluralizationService Pluralizer =
            PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo("en-us"));

        /// <summary>
        /// Gets the plural of the specified name.
        /// </summary>
        /// <param name="name">The name to pluralize.</param>
        /// <returns>The plural of the specified name.</returns>
        public static string Pluralize(this string name)
        {
            return Pluralizer.Pluralize(name);
        }

        /// <summary>
        /// Gets the plural of the last part of the specified dotted name.
        /// </summary>
        /// <param name="name">The name to pluralize.</param>
        /// <returns>The plural of the last part of the specified dotted name.</returns>
        public static string GetPluralizedTypeName(this string name)
        {
            name = name.Split('.').Last();
            return Pluralizer.Pluralize(name);
        }

        /// <summary>
        /// Gets the namespace qualified DTO name.
        /// </summary>
        /// <param name="name">The name of the entity.</param>
        /// <param name="partition">The name of the partition containing the entity.</param>
        /// <returns>The namespace qualified DTO name.</returns>
        public static string GetDtoName(this string name, string partition)
        {
            return "Gorba.Center.Common.ServiceModel." + partition + "." + name;
        }

        /// <summary>
        /// Gets the name of the type as it is used for classes.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <returns>The name of the type as it is used for classes.</returns>
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
    }
}