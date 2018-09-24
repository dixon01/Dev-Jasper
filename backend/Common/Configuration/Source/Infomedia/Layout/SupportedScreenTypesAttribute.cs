// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupportedScreenTypesAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SupportedScreenTypesAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Attribute used on classes deriving from <see cref="ElementBase"/>.
    /// This attribute tells icenter.media for which screen types the given element class can be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SupportedScreenTypesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedScreenTypesAttribute"/> class.
        /// </summary>
        /// <param name="types">
        /// The supported <see cref="PhysicalScreenType"/>s.
        /// </param>
        public SupportedScreenTypesAttribute(params PhysicalScreenType[] types)
        {
            this.Types = types;
        }

        /// <summary>
        /// Gets the supported <see cref="PhysicalScreenType"/>s.
        /// </summary>
        public PhysicalScreenType[] Types { get; private set; }

        /// <summary>
        /// Verifies if the given <see cref="PhysicalScreenType"/> is in the list
        /// of <see cref="Types"/>.
        /// </summary>
        /// <param name="type">
        /// The type to check for.
        /// </param>
        /// <returns>
        /// True if the given type is supported, otherwise false.
        /// </returns>
        public bool IsSupported(PhysicalScreenType type)
        {
            return this.Types != null && Array.IndexOf(this.Types, type) >= 0;
        }
    }
}