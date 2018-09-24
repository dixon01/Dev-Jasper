// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcVisualizerApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcVisualizerApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcVisualizer
{
    using System.Windows.Forms;

    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Infomedia.AhdlcRenderer;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Handlers;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The AHDLC visualizer application.
    /// </summary>
    public class AhdlcVisualizerApplication : ApplicationBase
    {
        private IApplication renderer;

        private volatile bool running;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun(string[])"/> or this method.
        /// </summary>
        protected override void DoRun()
        {
            var form = new MainForm();
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            var factory = new VisualizerFrameHandlerFactory();
            factory.FrameCreated += (s, e) => form.HandleFrame(e.Frame);
            factory.FrameReceived += (s, e) => form.HandleFrame(e.Frame);
            serviceContainer.RegisterInstance<FrameHandlerFactory>(factory);
            this.renderer = new AhdlcRendererApplication();
            this.renderer.Configure("AhdlcRenderer");
            this.renderer.Start();
            this.running = true;
            Application.Run(form);

            if (this.running)
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            this.running = false;
            this.renderer.Stop();
            Application.Exit();
        }
    }
}