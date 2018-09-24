// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResendHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResendHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Helper class that caches and resends messages to a given target address.
    /// </summary>
    /// <typeparam name="T">
    /// The type of message to cache. It must be XML serializable.
    /// </typeparam>
    internal class ResendHandler<T>
        where T : class, new()
    {
        private const long ResendWaitTime = 15 * 1000;

        private readonly Logger logger;

        private readonly Dictionary<MediAddress, List<CacheEntry>> cache =
            new Dictionary<MediAddress, List<CacheEntry>>();

        private readonly string cacheDirectory;

        private readonly ITimer resendTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResendHandler{T}"/> class.
        /// </summary>
        /// <param name="cacheDirectory">
        /// The cache directory in the local file system.
        /// </param>
        public ResendHandler(string cacheDirectory)
        {
            this.logger = LogManager.GetLogger(ClassNameProvider.GetGenericClassName(this.GetType()));

            this.cacheDirectory = cacheDirectory;

            this.logger.Trace("Creating cache directory: {0}", this.cacheDirectory);
            Directory.CreateDirectory(this.cacheDirectory);

            this.resendTimer = TimerFactory.Current.CreateTimer(typeof(T).Name + ".Resend");
            this.resendTimer.Interval = TimeSpan.FromMilliseconds(ResendWaitTime);
            this.resendTimer.AutoReset = true;
            this.resendTimer.Elapsed += this.ResendTimerOnElapsed;
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        public void Start()
        {
            this.logger.Debug("Starting");
            lock (this.cache)
            {
                foreach (var unitDir in Directory.GetDirectories(this.cacheDirectory))
                {
                    var unit = Path.GetFileName(unitDir);
                    foreach (var appDir in Directory.GetDirectories(unitDir))
                    {
                        var address = new MediAddress(unit, Path.GetFileName(appDir));
                        List<CacheEntry> entries;
                        if (!this.cache.TryGetValue(address, out entries))
                        {
                            entries = new List<CacheEntry>();
                            this.cache.Add(address, entries);
                        }

                        foreach (var cacheFile in
                            Directory.GetFiles(appDir, "*" + FileDefinitions.TempFileExtension))
                        {
                            this.logger.Trace("Loading cache file {0}", cacheFile);
                            var entry = new CacheEntry(cacheFile);
                            entries.Add(entry);
                        }
                    }
                }
            }

            this.resendTimer.Enabled = true;
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            this.logger.Debug("Stopping");
            this.resendTimer.Enabled = false;

            lock (this.cache)
            {
                this.cache.Clear();
            }
        }

        /// <summary>
        /// Sends the given object to the <see cref="destination"/> and caches the object before sending.
        /// </summary>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="obj">
        /// The object to send.
        /// </param>
        public void Send(MediAddress destination, T obj)
        {
            var tempPath = Path.Combine(this.cacheDirectory, Path.Combine(destination.Unit, destination.Application));
            Directory.CreateDirectory(tempPath);
            var cacheFile = Path.Combine(tempPath, Guid.NewGuid() + FileDefinitions.TempFileExtension);
            this.logger.Trace("Creating cache file {0}", cacheFile);
            var entry = new CacheEntry(obj, cacheFile);

            lock (this.cache)
            {
                List<CacheEntry> entries;
                if (!this.cache.TryGetValue(destination, out entries))
                {
                    entries = new List<CacheEntry>();
                    this.cache.Add(destination, entries);
                }

                entries.Add(entry);
            }

            MessageDispatcher.Instance.Send(destination, obj);
        }

        /// <summary>
        /// Removes all cache entries matching the given <see cref="destination"/>
        /// and <see cref="match"/> criteria.
        /// </summary>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        /// <param name="match">
        /// The match against which each cache entry is tested.
        /// </param>
        /// <returns>
        /// True if at least one entry was removed.
        /// </returns>
        public bool Remove(MediAddress destination, Predicate<T> match)
        {
            var found = false;
            lock (this.cache)
            {
                List<CacheEntry> entries;
                if (!this.cache.TryGetValue(destination, out entries))
                {
                    return false;
                }

                foreach (var entry in entries.ToArray())
                {
                    if (!match(entry.Item))
                    {
                        continue;
                    }

                    this.logger.Trace("Removing cache file {0}", entry.CacheFile);
                    entries.Remove(entry);
                    File.Delete(entry.CacheFile);
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// Resends all cached entries to the given <see cref="destination"/>.
        /// </summary>
        /// <param name="destination">
        /// The destination address.
        /// </param>
        public void ResendAll(MediAddress destination)
        {
            List<T> toSend;
            lock (this.cache)
            {
                List<CacheEntry> entries;
                if (!this.cache.TryGetValue(destination, out entries))
                {
                    return;
                }

                toSend = entries.ConvertAll(i => i.Item);
            }

            foreach (var state in toSend)
            {
                this.logger.Trace("Initial resending of {0} to {1}", state, destination);
                MessageDispatcher.Instance.Send(destination, state);
            }
        }

        private void ResendTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            Dictionary<MediAddress, List<CacheEntry>> allEntries;
            lock (this.cache)
            {
                allEntries = new Dictionary<MediAddress, List<CacheEntry>>(this.cache);
            }

            foreach (var entries in allEntries)
            {
                foreach (var entry in entries.Value)
                {
                    if (entry.ShouldResend())
                    {
                        this.logger.Trace("Resending {0} to {1}", entry.Item, entries.Key);
                        MessageDispatcher.Instance.Send(entries.Key, entry.Item);
                    }
                }
            }
        }

        private class CacheEntry
        {
            private readonly long creationTime = TimeProvider.Current.TickCount;

            public CacheEntry(T item, string cacheFile)
            {
                this.CacheFile = cacheFile;
                this.Item = item;

                var serializer = new XmlSerializer(typeof(T));
                using (var output = File.Create(this.CacheFile))
                {
                    serializer.Serialize(output, item);
                }
            }

            public CacheEntry(string cacheFile)
            {
                this.CacheFile = cacheFile;

                var serializer = new XmlSerializer(typeof(T));
                using (var input = File.OpenRead(this.CacheFile))
                {
                    this.Item = (T)serializer.Deserialize(input);
                }
            }

            public string CacheFile { get; private set; }

            public T Item { get; private set; }

            public bool ShouldResend()
            {
                return this.creationTime + ResendWaitTime < TimeProvider.Current.TickCount;
            }
        }
    }
}
