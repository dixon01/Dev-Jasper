// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaPool.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediaPool type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Manager for a pool of files.
    /// </summary>
    public class MediaPool : PoolBase<DrawableElementBase>
    {
        private static readonly List<string> ImageExtensions =
            new List<string> { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };

        private static readonly List<string> VideoExtensions =
            new List<string> { ".asf", ".avi", ".mpg", ".mp2", ".mp4", ".mpeg", ".wmv" };

        private readonly PoolConfig pool;

        private readonly string directoryPath;

        private int currentFileIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPool"/> class.
        /// </summary>
        /// <param name="poolName">
        /// The pool name.
        /// </param>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        public MediaPool(string poolName, IPresentationContext context)
        {
            Logger.Info("Loading pool '{0}'", poolName);

            this.pool = context.Config.Config.Pools.Find(p => p.Name == poolName);
            if (this.pool == null)
            {
                throw new KeyNotFoundException("Could not find pool " + poolName);
            }

            this.directoryPath = context.Config.GetAbsolutePathRelatedToConfig(this.pool.BaseDirectory);
        }

        /// <summary>
        /// Moves to the next item.
        /// </summary>
        /// <param name="wrapAround">
        /// A flag indicating if the method should wrap around
        /// when it gets to the end of the pool or return false.
        /// </param>
        /// <returns>
        /// A flag indicating if there was a next item found.
        /// If this method returns false, <see cref="IPool{T}.CurrentItem"/> is null.
        /// </returns>
        public override bool MoveNext(bool wrapAround)
        {
            var elements = this.FindElements();

            if (elements.Count == 0)
            {
                return false;
            }

            this.currentFileIndex++;
            if (this.currentFileIndex >= elements.Count)
            {
                if (!wrapAround)
                {
                    this.currentFileIndex = -1;
                    this.CurrentItem = null;
                    return false;
                }

                this.currentFileIndex = 0;
            }

            this.CurrentItem = elements[this.currentFileIndex];
            return true;
        }

        private static bool Contains(List<string> extensions, string extension)
        {
            return extensions.Find(e => e.Equals(extension, StringComparison.InvariantCultureIgnoreCase)) != null;
        }

        private IList<DrawableElementBase> FindElements()
        {
            var directory = new DirectoryInfo(this.directoryPath);
            var elements = new List<DrawableElementBase>();
            if (!directory.Exists)
            {
                this.Logger.Warn(
                    "Couldn't find pool base directory for pool {0}: {1}",
                    this.pool.Name,
                    this.directoryPath);
                return elements;
            }

            var files = directory.GetFiles();
            Array.Sort(files, (a, b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase));
            foreach (var file in files)
            {
                if (Contains(ImageExtensions, file.Extension))
                {
                    elements.Add(new ImageElement { Filename = file.FullName, Scaling = ElementScaling.Scale });
                }
                else if (Contains(VideoExtensions, file.Extension))
                {
                    elements.Add(new VideoElement { VideoUri = file.FullName, Scaling = ElementScaling.Scale });
                }
            }

            return elements;
        }
    }
}