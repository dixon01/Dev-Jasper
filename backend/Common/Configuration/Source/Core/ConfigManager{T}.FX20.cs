// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigManager{T}.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

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
            var cacheFileName = CachePathProvider.Current.GetCacheFilePath(this.FullConfigFileName);
            try
            {
                var fileInfo = new FileInfo(this.FullConfigFileName);
                using (var output = File.Create(cacheFileName))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(output, fileInfo.LastWriteTimeUtc);
                    formatter.Serialize(output, fileInfo.Length);
                    formatter.Serialize(output, this.config);
                }
            }
            catch (Exception)
            {
                // ignore the exception, we will just be using the XML file next time when deserializing
                this.EnableCaching = false;
            }
        }

        private bool LoadCachedConfig()
        {
            var cacheFileName = CachePathProvider.Current.GetCacheFilePath(this.FullConfigFileName);
            try
            {
                if (!File.Exists(cacheFileName))
                {
                    return false;
                }

                var fileInfo = new FileInfo(this.FullConfigFileName);
                using (var input = File.OpenRead(cacheFileName))
                {
                    var formatter = new BinaryFormatter();
                    var writeTime = (DateTime)formatter.Deserialize(input);
                    var length = (long)formatter.Deserialize(input);
                    if (writeTime != fileInfo.LastWriteTimeUtc || length != fileInfo.Length)
                    {
                        return false;
                    }

                    this.config = (T)formatter.Deserialize(input);
                }

                return true;
            }
            catch (Exception)
            {
                // ignore the exception, we will just be using the XML file
                return false;
            }
        }
    }
}
