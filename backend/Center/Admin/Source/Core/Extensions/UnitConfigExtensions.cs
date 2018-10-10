// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitConfigExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitConfigExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Extensions
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Admin.Core.Models.UnitConfig;

    /// <summary>
    /// Extension methods for the <see cref="Gorba.Center.Admin.Core.Models.UnitConfig"/> namespace.
    /// </summary>
    public static class UnitConfigExtensions
    {
        /// <summary>
        /// Tries to get the string value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// True if the value is found.
        /// </returns>
        public static bool TryGetStringValue(this UnitConfigPart part, string key, out string value)
        {
            var partValue = part.Values.FirstOrDefault(p => p.Key == key);
            if (partValue == null)
            {
                value = null;
                return false;
            }

            value = partValue.Value;
            return true;
        }

        /// <summary>
        /// Gets the string value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static string GetValue(this UnitConfigPart part, string defaultValue, string key = null)
        {
            string value;
            return part.TryGetStringValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the decimal value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static decimal GetValue(this UnitConfigPart part, decimal defaultValue, string key = null)
        {
            string value;
            decimal decValue;
            return part.TryGetStringValue(key, out value) && decimal.TryParse(value, out decValue)
                       ? decValue
                       : defaultValue;
        }

        /// <summary>
        /// Gets the boolean value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static bool GetValue(this UnitConfigPart part, bool defaultValue, string key = null)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return part.GetValue((bool?)defaultValue, key).Value;
        }

        /// <summary>
        /// Gets the nullable boolean value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static bool? GetValue(this UnitConfigPart part, bool? defaultValue, string key = null)
        {
            string value;
            if (!part.TryGetStringValue(key, out value))
            {
                return defaultValue;
            }

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            bool boolValue;
            return bool.TryParse(value, out boolValue) ? boolValue : defaultValue;
        }

        /// <summary>
        /// Gets the timespan value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static TimeSpan GetValue(this UnitConfigPart part, TimeSpan defaultValue, string key = null)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return part.GetValue((TimeSpan?)defaultValue, key).Value;
        }

        /// <summary>
        /// Gets the nullable timespan value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static TimeSpan? GetValue(this UnitConfigPart part, TimeSpan? defaultValue, string key = null)
        {
            string value;
            if (!part.TryGetStringValue(key, out value))
            {
                return defaultValue;
            }

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            TimeSpan timeSpan;
            return TimeSpan.TryParseExact(value, "G", CultureInfo.InvariantCulture, out timeSpan)
                       ? timeSpan
                       : defaultValue;
        }

        /// <summary>
        /// Gets the color value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static Color GetValue(this UnitConfigPart part, Color defaultValue, string key = null)
        {
            // ReSharper disable once PossibleInvalidOperationException
            return part.GetValue((Color?)defaultValue, key).Value;
        }

        /// <summary>
        /// Gets the nullable color value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static Color? GetValue(this UnitConfigPart part, Color? defaultValue, string key = null)
        {
            string value;
            if (!part.TryGetStringValue(key, out value))
            {
                return defaultValue;
            }

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (value.Length != 9 || value[0] != '#')
            {
                return defaultValue;
            }

            byte a, r, g, b;
            if (byte.TryParse(value.Substring(1, 2), NumberStyles.HexNumber, null, out a)
                && byte.TryParse(value.Substring(3, 2), NumberStyles.HexNumber, null, out r)
                && byte.TryParse(value.Substring(5, 2), NumberStyles.HexNumber, null, out g)
                && byte.TryParse(value.Substring(7, 2), NumberStyles.HexNumber, null, out b))
            {
                return Color.FromArgb(a, r, g, b);
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the enum value for the given key in the part.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum.
        /// </typeparam>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static T GetEnumValue<T>(this UnitConfigPart part, T defaultValue, string key = null)
            where T : struct, IConvertible
        {
            string value;
            T enumValue;
            return part.TryGetStringValue(key, out value) && Enum.TryParse(value, out enumValue)
                       ? enumValue
                       : defaultValue;
        }

        /// <summary>
        /// Gets the XML deserialized value for the given key in the part.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum.
        /// </typeparam>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise a new instance of <see cref="T"/>.
        /// </returns>
        public static T GetXmlValue<T>(this UnitConfigPart part, string key = null)
            where T : new()
        {
            return part.GetXmlValue(new T(), key);
        }

        /// <summary>
        /// Gets the XML deserialized value for the given key in the part.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum.
        /// </typeparam>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// If the key is found, the corresponding value, otherwise <paramref name="defaultValue"/>.
        /// </returns>
        public static T GetXmlValue<T>(this UnitConfigPart part, T defaultValue, string key = null)
        {
            string value;
            if (!part.TryGetStringValue(key, out value))
            {
                return defaultValue;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var reader = new StringReader(value);
                return (T)serializer.Deserialize(reader);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets the string value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, string value, string key = null)
        {
            var partValue = part.Values.FirstOrDefault(p => p.Key == key);
            if (partValue == null)
            {
                partValue = new UnitConfigPartValue { Key = key };
                part.Values.Add(partValue);
            }

            partValue.Value = value;
        }

        /// <summary>
        /// Sets the decimal value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, decimal value, string key = null)
        {
            part.SetValue(value.ToString(CultureInfo.InvariantCulture), key);
        }

        /// <summary>
        /// Sets the boolean value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, bool value, string key = null)
        {
            part.SetValue(value.ToString(), key);
        }

        /// <summary>
        /// Sets the nullable boolean value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, bool? value, string key = null)
        {
            part.SetValue(value.HasValue ? value.Value.ToString() : string.Empty, key);
        }

        /// <summary>
        /// Sets the timespan value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, TimeSpan value, string key = null)
        {
            part.SetValue(value.ToString("G", CultureInfo.InvariantCulture), key);
        }

        /// <summary>
        /// Sets the nullable timespan value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, TimeSpan? value, string key = null)
        {
            part.SetValue(value.HasValue ? value.Value.ToString("G", CultureInfo.InvariantCulture) : string.Empty, key);
        }

        /// <summary>
        /// Sets the color value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, Color value, string key = null)
        {
            part.SetValue((Color?)value, key);
        }

        /// <summary>
        /// Sets the nullable color value for the given key in the part.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetValue(this UnitConfigPart part, Color? value, string key = null)
        {
            part.SetValue(
                value.HasValue
                    ? string.Format(
                        "#{0:X2}{1:X2}{2:X2}{3:X2}",
                        value.Value.A,
                        value.Value.R,
                        value.Value.G,
                        value.Value.B)
                    : string.Empty,
                key);
        }

        /// <summary>
        /// Sets the enum value for the given key in the part.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum.
        /// </typeparam>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetEnumValue<T>(this UnitConfigPart part, T value, string key = null)
            where T : struct, IConvertible
        {
            part.SetValue(value.ToString(CultureInfo.InvariantCulture), key);
        }

        /// <summary>
        /// Sets the XML serialized value for the given key in the part.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to be serialized.
        /// </typeparam>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public static void SetXmlValue<T>(this UnitConfigPart part, T value, string key = null)
        {
            var serializer = new XmlSerializer(typeof(T));
            var writer = new StringWriter();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };
            var xmlWriter = XmlWriter.Create(writer, settings);
            serializer.Serialize(xmlWriter, value, namespaces);
            part.SetValue(writer.ToString(), key);
        }
    }
}
