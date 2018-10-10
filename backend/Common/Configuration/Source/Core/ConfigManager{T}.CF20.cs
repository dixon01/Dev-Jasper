// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigManager{T}.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    /// <summary>
    /// Generic class used to handle configuration into/from file using serialization.
    /// </summary>
    /// <remarks>
    /// The FileName could be defined, but if you don't initialized it, the file name is the name of the T type + ".xml"
    /// </remarks>
    /// <typeparam name="T">
    /// Class of the top configuration container.
    /// </typeparam>
    public partial class ConfigManager<T> where T : class, new()
    {
        private void SaveCachedConfig()
        {
            // TODO: implement some fast binary serialization
        }

        private bool LoadCachedConfig()
        {
            // TODO: implement some fast binary serialization
            return false;
        }
    }
}
