// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Container of all the constant strings used in the whole XIMPLE library.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Utils
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Container of all the constant strings used in the whole XIMPLE library.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Ximple version 2.0.0
        /// </summary>
        public static readonly string Version2 = "2.0.0";

        /// <summary>
        /// Default Ximple version
        /// </summary>
        public static readonly string DefaultVersion = Version2;

        public const string XimpleEndXmlTag = "</Ximple>";

        /// <summary>
        /// Gets the version of this assembly.
        /// </summary>
        public static string VersionLib
        {
            get
            {
                Version version;
                try
                {
                    version = Assembly.GetExecutingAssembly().GetName().Version;
                }
                catch (Exception)
                {
                    version = null;
                }

                if (version == null)
                {
                    // an error was occured getting the library's version.
                    return string.Empty;
                }

                // else...
                return version.ToString();
            }
        }
    }
}
