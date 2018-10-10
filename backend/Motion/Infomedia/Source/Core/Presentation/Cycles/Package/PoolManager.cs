// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PoolManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Motion.Infomedia.Core.Presentation.Webmedia;

    /// <summary>
    /// The pool manager is responsible for maintaining a list of all pools
    /// that are being used by a presentation manager.
    /// </summary>
    public class PoolManager : IDisposable
    {
        private readonly IPresentationContext context;

        private readonly Dictionary<string, MediaPool> mediaPools = new Dictionary<string, MediaPool>();

        private readonly Dictionary<string, WebmediaPool> webmediaPools =
            new Dictionary<string, WebmediaPool>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<long, PagePool> pagePools = new Dictionary<long, PagePool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolManager"/> class.
        /// </summary>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public PoolManager(IPresentationContext context)
        {
            this.context = context;
        }

        private delegate T PoolCreator<T>();

        /// <summary>
        /// Gets a media pool for the given pool name.
        /// </summary>
        /// <param name="poolName">
        /// The pool name.
        /// </param>
        /// <returns>
        /// A pool that returns <see cref="DrawableElementBase"/>s
        /// that can be displayed either in full-screen or frame mode.
        /// </returns>
        public IPool<DrawableElementBase> GetMediaPool(string poolName)
        {
            return GetPool(this.mediaPools, poolName, () => new MediaPool(poolName, this.context));
        }

        /// <summary>
        /// Gets a webmedia pool for the given webmedia config file.
        /// </summary>
        /// <param name="webmediaConfigFileName">
        /// The webmedia config file name.
        /// </param>
        /// <returns>
        /// A pool that returns <see cref="WebmediaElementBase"/>s
        /// that can be used to create elements to show in the presentation.
        /// </returns>
        public IPool<WebmediaElementBase> GetWebmediaPool(string webmediaConfigFileName)
        {
            var filename = Path.GetFullPath(webmediaConfigFileName);
            return GetPool(this.webmediaPools, filename, () => new WebmediaPool(filename, this.context));
        }

        /// <summary>
        /// Gets a page pool for the given language and table.
        /// </summary>
        /// <param name="language">
        /// The generic language.
        /// </param>
        /// <param name="table">
        /// The generic table.
        /// </param>
        /// <returns>
        /// A pool that returns <see cref="Page"/>s that can be used if multiple
        /// sections should show the same page pool.
        /// </returns>
        public IPool<PageInfo> GetPagePool(int language, int table)
        {
            long id = language;
            id <<= 32;
            id |= (uint)table;
            return GetPool(this.pagePools, id, () => new PagePool(language, table, 1, -1, this.context));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var webmediaPool in this.webmediaPools.Values)
            {
                webmediaPool.Dispose();
            }

            this.webmediaPools.Clear();

            foreach (var pagePool in this.pagePools.Values)
            {
                pagePool.Dispose();
            }

            this.pagePools.Clear();
        }

        private static TPool GetPool<TKey, TPool>(Dictionary<TKey, TPool> pools, TKey key, PoolCreator<TPool> creator)
        {
            TPool pool;
            if (!pools.TryGetValue(key, out pool))
            {
                pool = creator();
                pools[key] = pool;
            }

            return pool;
        }
    }
}