// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ModelExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Extension methods used by the model.
    /// </summary>
    public static class ModelExtensions
    {
        private static readonly PluralizationService PluralizationService =
            PluralizationService.CreateService(
                CultureInfo.GetCultureInfo("en-us"));

        /// <summary>
        /// Pluralizes the given name.
        /// </summary>
        /// <param name="name">The name to pluralize.</param>
        /// <returns>The pluralized version of the name.</returns>
        public static string Pluralize(this string name)
        {
            return PluralizationService.Pluralize(name);
        }

        /// <summary>
        /// Gets the entity name removing anything before the last dot &quot;.&quot;.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The entity name.
        /// </returns>
        public static string GetEntityName(this string name)
        {
            var names = name.Split('.');
            return names.Last();
        }

        /// <summary>
        /// Gets the name of the foreign key field.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The foreign key name.
        /// </returns>
        public static string GetForeignKeyName(this string name)
        {
            return name + "_Id";
        }

        /// <summary>
        /// The get association name.
        /// </summary>
        /// <param name="associationEnds">
        /// The association ends.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetAssociationName(this IEnumerable<string> associationEnds)
        {
            Func<string, string> getTypeName = end => end.Split('.').Last();
            return associationEnds.Select(getTypeName).Aggregate("Association", (s, s1) => s + s1);
        }
    }
}