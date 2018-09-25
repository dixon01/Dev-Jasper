// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenXmlExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OpenXmlExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Tools.XmlDocumentor
{
    using DocumentFormat.OpenXml;

    /// <summary>
    /// Extension methods for namespace <see cref="DocumentFormat.OpenXml"/>.
    /// </summary>
    public static class OpenXmlExtensions
    {
        /// <summary>
        /// Gets the first child of a given type or appends a new instance of it if it doesn't exist yet.
        /// </summary>
        /// <param name="element">
        /// The element to find the child in.
        /// </param>
        /// <typeparam name="T">
        /// The type of child to be found.
        /// </typeparam>
        /// <returns>
        /// The found or created child.
        /// </returns>
        public static T GetOrAppendChild<T>(this OpenXmlCompositeElement element)
            where T : OpenXmlElement, new()
        {
            return element.GetFirstChild<T>() ?? element.AppendChild(new T());
        }
    }
}
