// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValueFilterDefinition.ttcode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PropertyValueFilterDefinition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[module: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName",
    Justification = "Reviewed: special extension not supported by StyleCop.")]

namespace Gorba.Center.Common.ServiceModel.Filters
{
    using System.Collections.Generic;

    /// <summary>
    /// The property value filter definition.
    /// </summary>
    public class PropertyValueFilterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValueFilterDefinition"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public PropertyValueFilterDefinition(string name, string type)
        {
            this.Name = name;
            this.Type = type;
            this.AdditionalComparisonOperators = new List<AdditionalComparisonOperator>();
        }

        /// <summary>
        /// The additional comparison operators.
        /// </summary>
        public enum AdditionalComparisonOperator
        {
            /// <summary>
            /// Case insensitive match.
            /// </summary>
            CaseInsensitiveMatch,

            /// <summary>
            /// The first value is greater than the second one.
            /// </summary>
            GreaterThan,

            /// <summary>
            /// The first value is greater than or equal to the second one.
            /// </summary>
            GreaterThanOrEqualTo,

            /// <summary>
            /// The first value is less than the second one.
            /// </summary>
            LessThan,

            /// <summary>
            /// The first value is less than or equal to the second one.
            /// </summary>
            LessThanOrEqualTo
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the additional comparison operators.
        /// </summary>
        public ICollection<AdditionalComparisonOperator> AdditionalComparisonOperators { get; private set; }
    }
}