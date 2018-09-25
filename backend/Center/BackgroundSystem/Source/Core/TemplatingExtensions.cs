// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatingExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemplatingExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core
{
    using System.Data.Entity.Design.PluralizationServices;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Defines extensions used for template.
    /// </summary>
    public static class TemplatingExtensions
    {
        private static readonly PluralizationService PluralizationService =
            PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        /// <summary>
        /// Gets the entity name optionally prefixed with full namespace (only required when the entity is in
        /// a different partition).
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>
        /// The name of the entity; the name is namespace-qualified if it is in a different partition.
        /// </returns>
        public static string GetNameWithServiceModelNamespace(this string entityName)
        {
            if (entityName.IndexOf('.') == -1)
            {
                return entityName;
            }

            return "Gorba.Center.Common.ServiceModel." + entityName;
        }

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
        /// Gets the variable name for the given entity.
        /// </summary>
        /// <param name="entityName">The name of the entity.</param>
        /// <returns>The variable name.</returns>
        public static string GetVariableName(this string entityName)
        {
            return
                entityName.Substring(0, 1).ToLowerInvariant() + entityName.Substring(1) + "ChangeTrackingDataService";
        }

        /// <summary>
        /// Pluralizes the given noun.
        /// </summary>
        /// <param name="noun">The noun to pluralize.</param>
        /// <returns>The pluralized noun.</returns>
        public static string Pluralize(this string noun)
        {
            return PluralizationService.Pluralize(noun);
        }
    }
}