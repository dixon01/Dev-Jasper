// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Linq2XmlExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The Linq2XmlExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The Linq2XmlExtensions.
    /// </summary>
    public static class Linq2XmlExtensions
    {
        /// <summary>
        /// Extension method to return an attribute value if the attribute exists
        /// </summary>
        /// <param name="element">the parent element</param>
        /// <param name="attributeName">the name of the attribute</param>
        /// <param name="defaultValue">the default return value</param>
        /// <returns>the value of the attribute or defaultValue or null</returns>
        public static string TryGetAttribute(this XElement element, string attributeName, string defaultValue = null)
        {
            var result = defaultValue;

            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                result = attribute.Value;
            }

            return result;
        }

        /// <summary>
        /// Extension method to return an attribute value if the attribute exists
        /// </summary>
        /// <param name="element">the parent element</param>
        /// <param name="attributeName">the name of the attribute</param>
        /// <returns>the value of the attribute or null</returns>
        public static bool? TryGetBoolAttribute(this XElement element, string attributeName)
        {
            bool? result = null;

            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                bool booleanValue;
                if (bool.TryParse(attribute.Value, out booleanValue))
                {
                    result = booleanValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Extension method to return an attribute value if the attribute exists
        /// </summary>
        /// <param name="element">the parent element</param>
        /// <param name="attributeName">the name of the attribute</param>
        /// <param name="defaultValue">the default value</param>
        /// <returns>the value of the attribute or defaultValue or null</returns>
        public static int TryGetIntAttribute(this XElement element, string attributeName, int defaultValue)
        {
            int result = defaultValue;

            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                int intValue;
                if (int.TryParse(attribute.Value, out intValue))
                {
                    result = intValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Extension method to return an attribute value if the attribute exists
        /// </summary>
        /// <typeparam name="T">the result type</typeparam>
        /// <param name="element">the parent element</param>
        /// <param name="elementName">the name of the element</param>
        /// <param name="convert">the convert function</param>
        /// <returns>the value of the attribute or defaultValue or null</returns>
        public static T TryConvertChildElement<T>(this XElement element, string elementName, Func<XElement, T> convert)
            where T : class
        {
            T result = null;

            var childElement = element.Element(elementName);
            if (childElement != null)
            {
                result = convert(childElement);
            }

            return result;
        }

        /// <summary>
        /// Extension method to return an attribute value if the attribute exists
        /// </summary>
        /// <typeparam name="T">the result type</typeparam>
        /// <param name="element">the parent element</param>
        /// <param name="convert">the convert function</param>
        /// <returns>the value of the attribute or defaultValue or null</returns>
        public static T TryConvertFirstChildElement<T>(this XElement element, Func<XElement, T> convert)
            where T : class
        {
            T result = null;

            var childElement = element.Descendants().FirstOrDefault();
            if (childElement != null)
            {
                result = convert(childElement);
            }

            return result;
        }
    }
}