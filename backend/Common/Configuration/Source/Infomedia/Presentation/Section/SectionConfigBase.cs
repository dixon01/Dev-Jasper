// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SectionConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Section
{
    using System;

    /// <summary>
    /// Base class for all types of section configurations.
    /// </summary>
    public abstract partial class SectionConfigBase
    {
        /// <summary>
        /// This is a little trick for backwards compatibility:
        /// in file format 1.2 the "Duration" property was a double, now we have a proper XML duration (<c>PTxx</c>).
        /// </summary>
        private static class XmlConvert
        {
            public static string ToString(TimeSpan timeSpan)
            {
                return System.Xml.XmlConvert.ToString(timeSpan);
            }

            public static TimeSpan ToTimeSpan(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException();
                }

                if (value[0] == 'P')
                {
                    return System.Xml.XmlConvert.ToTimeSpan(value);
                }

                return TimeSpan.FromSeconds(double.Parse(value));
            }
        }
    }
}