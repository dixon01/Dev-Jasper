// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererApplication.cs" company="Gorba AG">
//   Copyright © 2011-2018 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRendererApp
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.DirectXRenderer;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Motion.Infomedia.DirectXRenderer;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Handles management of DirectX Renderer
    /// </summary>
    public class RendererApplication : ApplicationBase, IManageable
    {
        /// <summary>
        /// The management name.
        /// </summary>
        internal static readonly string ManagementName = "DirectXRenderer";

        private readonly Renderer renderer;

        private ILoopObserver loopObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererApplication"/> class.
        /// </summary>
        public RendererApplication()
        {
            var config = RendererConfig.LoadFrom(PathManager.Instance.GetPath(FileType.Config, "DirectXRenderer.xml"));

            this.renderer = new Renderer(config);
            this.renderer.Started += this.RendererOnStarted;
            this.renderer.ExitRequested += this.RendererOnExitRequested;

            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                ManagementName, root, this);
            root.AddChild(provider);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield return parent.Factory.CreateManagementProvider("Renderer", parent, this.renderer);
        }

        /// <summary>
        /// The do run.
        /// </summary>
        protected override void DoRun()
        {
            this.renderer.Run();

            if (this.loopObserver != null)
            {
                this.loopObserver.Dispose();
            }
        }

        /// <summary>
        /// The do stop.
        /// </summary>
        protected override void DoStop()
        {
            this.renderer.Stop();
        }

        private void RendererOnStarted(object sender, EventArgs e)
        {
            this.SetRunning();

            var registration = ServiceLocator.Current.GetInstance<IApplicationRegistration>(ManagementName);
            try
            {
                this.loopObserver = registration.CreateLoopObserver(this.GetType().Name, TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't create loop observer");
            }

            if (this.loopObserver != null)
            {
                this.renderer.FrameRendered += (s, ev) => this.loopObserver.Trigger();
            }
        }

        private void RendererOnExitRequested(object sender, ExitRequestEventArgs e)
        {
            try
            {
                var registration = ServiceLocator.Current.GetInstance<IApplicationRegistration>(ManagementName);
                registration.Exit(e.Reason);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                this.Logger.Debug(ex, "Couldn't request exit");
            }
        }
    }
}
