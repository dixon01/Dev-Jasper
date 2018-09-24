// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaPage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Webmedia
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package;

    /// <summary>
    /// A single "page" of a <see cref="WebmediaCycle"/>.
    /// This is not to be confused with a <see cref="WebmediaSection"/>.
    /// </summary>
    public class WebmediaPage : IDisposable
    {
        private readonly DynamicPropertyHandler enabledHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaPage"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public WebmediaPage(WebmediaElementBase element, IPresentationContext context)
        {
            this.Element = element;
            this.enabledHandler = new DynamicPropertyHandler(element.EnabledProperty, true, context);
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        public WebmediaElementBase Element { get; private set; }

        /// <summary>
        /// The is enabled.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEnabled()
        {
            return this.Element.Enabled && (this.enabledHandler == null || this.enabledHandler.BoolValue);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.enabledHandler.Dispose();
        }
    }
}