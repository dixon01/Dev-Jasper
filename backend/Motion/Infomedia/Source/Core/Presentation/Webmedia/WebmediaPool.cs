// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaPool.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Webmedia
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    /// <summary>
    /// The webmedia cycle manager responsible for managing cycles defined in a
    /// <code>*.wm2</code> file.
    /// </summary>
    public class WebmediaPool : PoolBase<WebmediaElementBase>, IDisposable
    {
        private readonly string filename;

        private readonly IPresentationContext context;

        private readonly FileCheck fileCheck;

        private readonly List<WebmediaCycle> cycles = new List<WebmediaCycle>();

        private WebmediaCycle currentCycle;

        private bool shouldReloadConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaPool"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public WebmediaPool(string filename, IPresentationContext context)
        {
            this.filename = context.Config.GetAbsolutePathRelatedToConfig(filename);
            this.context = context;
            this.fileCheck = new FileCheck(filename);
            this.TryReloadConfigFile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaPool"/> class.
        /// This constructor is only used for testing.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        internal WebmediaPool(WebmediaConfig config, IPresentationContext context)
        {
            this.context = context;
            this.InitCycles(config);
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
            if (this.shouldReloadConfig
                || (this.fileCheck != null && this.fileCheck.CheckChanged() && this.fileCheck.Exists))
            {
                this.TryReloadConfigFile();
            }

            this.FindValidCycle();
            if (this.currentCycle != null)
            {
                this.currentCycle.ShowNextPage(wrapAround);
            }

            if (this.currentCycle == null || this.currentCycle.CurrentPage == null)
            {
                this.CurrentItem = null;
                return false;
            }

            this.Logger.Info(
                "Changing to webmedia page: {0} in cycle {1}",
                this.currentCycle.CurrentPage.Element.Name,
                this.currentCycle.Config.Name);

            this.CurrentItem = this.currentCycle.CurrentPage.Element;
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.DisposeCycles();
        }

        private void TryReloadConfigFile()
        {
            this.Logger.Info("Loading webmedia pool from {0}", this.filename);
            var configManager = new ConfigManager<WebmediaConfig> { FileName = this.filename, EnableCaching = true };
            WebmediaConfig config;
            try
            {
                config = configManager.Config;
                this.shouldReloadConfig = false;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't load config file {0}", this.filename);
                this.shouldReloadConfig = true;
                return;
            }

            // update the elements to contain absolute paths
            foreach (var cycleConfig in config.Cycles)
            {
                foreach (var element in cycleConfig.Elements)
                {
                    this.UpdateElement(element, configManager);
                }
            }

            this.DisposeCycles();
            this.InitCycles(config);
        }

        private void InitCycles(WebmediaConfig config)
        {
            foreach (var cycleConfig in config.Cycles)
            {
                var cycle = new WebmediaCycle(cycleConfig, this.context);
                this.cycles.Add(cycle);
            }

            this.currentCycle = null;
        }

        private void UpdateElement(WebmediaElementBase element, ConfigManager<WebmediaConfig> configManager)
        {
            var image = element as ImageWebmediaElement;
            if (image != null)
            {
                image.Filename = configManager.GetAbsolutePathRelatedToConfig(image.Filename);
                return;
            }

            var video = element as VideoWebmediaElement;
            if (video != null)
            {
                video.VideoUri = configManager.GetAbsolutePathRelatedToConfig(video.VideoUri);
                return;
            }

            var layout = element as LayoutWebmediaElement;
            if (layout == null)
            {
                return;
            }

            foreach (var layoutElement in layout.Elements)
            {
                var imageElement = layoutElement as ImageElement;
                if (imageElement != null)
                {
                    imageElement.Filename = configManager.GetAbsolutePathRelatedToConfig(imageElement.Filename);
                    continue;
                }

                var videoElement = layoutElement as VideoElement;
                if (videoElement != null)
                {
                    videoElement.VideoUri = configManager.GetAbsolutePathRelatedToConfig(videoElement.VideoUri);
                }
            }
        }

        private void DisposeCycles()
        {
            foreach (var cycle in this.cycles)
            {
                cycle.Dispose();
            }

            this.cycles.Clear();
        }

        private void FindValidCycle()
        {
            foreach (var cycle in this.cycles)
            {
                if (!cycle.IsEnabled())
                {
                    continue;
                }

                if (this.currentCycle == cycle)
                {
                    return;
                }

                cycle.ResetCycle();

                this.currentCycle = cycle;
                this.Logger.Info("Changing to cycle: {0}", cycle.Config.Name);
                return;
            }
        }
    }
}
