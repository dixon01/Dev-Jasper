// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebMediaCycle.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Webmedia
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Webmedia;

    /// <summary>
    /// Class that represents a Web media cycle.
    /// </summary>
    public class WebmediaCycle : IDisposable
    {
        private readonly DynamicPropertyHandler enabledHandler;

        private readonly List<WebmediaPage> pages = new List<WebmediaPage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaCycle"/> class.
        /// </summary>
        /// <param name="config">
        ///     The config.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        public WebmediaCycle(WebmediaCycleConfig config, IPresentationContext context)
        {
            this.Config = config;
            this.enabledHandler = new DynamicPropertyHandler(config.EnabledProperty, true, context);

            foreach (var element in config.Elements)
            {
                if (element != null)
                {
                    this.pages.Add(new WebmediaPage(element, context));
                }
            }
        }

        /// <summary>
        /// Gets the config.
        /// </summary>
        public WebmediaCycleConfig Config { get; private set; }

        /// <summary>
        /// Gets the current element.
        /// </summary>
        public WebmediaPage CurrentPage { get; private set; }

        /// <summary>
        /// The is enabled.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEnabled()
        {
            return this.Config.Enabled && (this.enabledHandler == null || this.enabledHandler.BoolValue);
        }

        /// <summary>
        /// Switches to the next page.
        /// </summary>
        /// <param name="wrapAround">
        /// A flag indicating if the method should wrap around
        /// when it gets to the end of the cycle or return false.
        /// </param>
        /// <returns>
        /// A flag indicating if there was a next page found.
        /// </returns>
        public bool ShowNextPage(bool wrapAround)
        {
            int index = this.pages.IndexOf(this.CurrentPage);

            var pageCount = this.pages.Count;
            for (int i = 0; i < pageCount; i++)
            {
                index++;
                if (index >= pageCount)
                {
                    if (!wrapAround)
                    {
                        this.CurrentPage = null;
                        return false;
                    }

                    index = 0;
                }

                var page = this.pages[index];
                if (!page.IsEnabled())
                {
                    continue;
                }

                this.CurrentPage = page;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The reset cycle.
        /// </summary>
        public void ResetCycle()
        {
            this.CurrentPage = null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var page in this.pages)
            {
                page.Dispose();
            }

            this.enabledHandler.Dispose();
        }
    }
}
