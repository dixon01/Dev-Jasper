// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// Section that can show multiple pages.
    /// </summary>
    public class MultiSection : PackageSectionBase
    {
        private readonly MultiSectionConfig config;

        private readonly LayoutConfig layout;

        private IPool<PageInfo> pagePool;

        private bool showingPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSection"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        internal MultiSection(MultiSectionConfig config, PoolManager poolManager, IPresentationContext context)
            : base(config, context)
        {
            this.config = config;

            this.layout = this.Context.Config.Config.Layouts.Find(c => c.Name == this.Config.Layout);

            if (config.Mode == PageMode.Pool)
            {
                this.pagePool = poolManager.GetPagePool(config.Language, config.Table);
                this.pagePool.CurrentItemValidChanged += this.PagePoolOnCurrentItemValidChanged;
            }
            else
            {
                this.CreatePagePool();
            }
        }

        /// <summary>
        /// Check if this section is currently enabled.
        /// </summary>
        /// <returns>
        /// true if this section is enabled either by configuration or by evaluation.
        /// </returns>
        public override bool IsEnabled()
        {
            if (!base.IsEnabled())
            {
                return false;
            }

            return this.pagePool.CurrentItem == null || this.pagePool.CurrentItemValid;
        }

        /// <summary>
        /// Deactivates this section.
        /// </summary>
        public override void Deactivate()
        {
            this.showingPage = false;
            if (this.config.Mode == PageMode.AllPages)
            {
                // we have to recreate the pool since we want to start from the
                // first page the next time we get activated
                this.CreatePagePool();
            }

            base.Deactivate();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            base.Dispose();

            this.pagePool.CurrentItemValidChanged -= this.PagePoolOnCurrentItemValidChanged;

            var disposable = this.pagePool as IDisposable;
            if (disposable != null && this.config.Mode != PageMode.Pool)
            {
                disposable.Dispose();
                this.pagePool = null;
            }
        }

        /// <summary>
        /// Finds the next available page.
        /// </summary>
        /// <returns>
        /// The page to be shown or null if no page can be shown and the cycle should
        /// switch to the next section.
        /// </returns>
        protected override Page FindNextObject()
        {
            if (this.layout == null)
            {
                this.showingPage = false;
                this.Logger.Warn("Layout not found: {0}", this.Config.Layout);
                return null;
            }

            var singlePage = this.config.Mode != PageMode.AllPages;
            if (this.showingPage && singlePage)
            {
                this.showingPage = false;
                return null;
            }

            if (!this.pagePool.MoveNext(singlePage))
            {
                this.showingPage = false;
                this.Logger.Debug("No more pages to show");
                return null;
            }

            this.showingPage = true;
            var info = this.pagePool.CurrentItem;
            this.Logger.Info(
                "Switching to page {0}/{1} (offset={2})", info.PageIndex + 1, info.TotalPages, info.RowOffset);

            return new Page(
                new StandardLayout(this.layout, this.Context.Config.Config),
                this.config.Duration,
                info.PageIndex,
                info.TotalPages,
                info.RowOffset,
                this.config.Language,
                this.config.Table);
        }

        private void CreatePagePool()
        {
            if (this.pagePool != null)
            {
                this.pagePool.CurrentItemValidChanged -= this.PagePoolOnCurrentItemValidChanged;

                var disposable = this.pagePool as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            this.pagePool = new PagePool(
                this.config.Language, this.config.Table, this.config.RowsPerPage, this.config.MaxPages, this.Context);
            this.pagePool.CurrentItemValidChanged += this.PagePoolOnCurrentItemValidChanged;
        }

        private void PagePoolOnCurrentItemValidChanged(object sender, EventArgs e)
        {
            this.RaiseEnabledChanged(e);
        }
    }
}