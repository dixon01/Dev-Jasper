// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtranVisualizerApplication.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtranVisualizerApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.SystemManagement.Host;
    using Gorba.Motion.Protran.AbuDhabi;
    using Gorba.Motion.Protran.Core;
    using Gorba.Motion.Protran.Core.Protocols;
    using Gorba.Motion.Protran.Ibis;
    using Gorba.Motion.Protran.IO;
    using Gorba.Motion.Protran.Vdv301;
    using Gorba.Motion.Protran.Visualizer.Controls;
    using Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi;
    using Gorba.Motion.Protran.Visualizer.Controls.Ibis;
    using Gorba.Motion.Protran.Visualizer.Controls.IO;
    using Gorba.Motion.Protran.Visualizer.Controls.Main;
    using Gorba.Motion.Protran.Visualizer.Controls.Vdv301;
    using Gorba.Motion.Protran.Visualizer.Data.AbuDhabi;
    using Gorba.Motion.Protran.Visualizer.Data.Ibis;
    using Gorba.Motion.Protran.Visualizer.Data.IO;
    using Gorba.Motion.Protran.Visualizer.Data.Vdv301;

    using Luminator.Motion.Protran.XimpleProtocol;

    /// <summary>
    /// The Protran Visualizer application.
    /// </summary>
    public class ProtranVisualizerApplication : ApplicationBase
    {
        private Splash splash;

        private VisualizerMainForm mainForm;

        private ApplicationHost<ProtranApplication> protranHost;

        private ApplicationLogForm logForm;

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// Implementing classes should either override <see cref="ApplicationBase.DoRun(string[])"/> or this method.
        /// </summary>
        protected override void DoRun()
        {
            this.splash = new Splash();
            this.splash.Show();

            this.mainForm = new VisualizerMainForm();
            this.mainForm.Closed += this.ParentFormOnClosed;

            var runThread = new Thread(this.RunProtran) { IsBackground = false };
            runThread.Start();

            Application.Run();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="ApplicationBase.DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            if (this.mainForm != null)
            {
                this.mainForm.Invoke(new ThreadStart(Application.Exit));
            }
            else
            {
                this.protranHost.Application.Exit("Visualizer application stopped");
            }
        }

        private void ParentFormOnClosed(object sender, EventArgs e)
        {
            this.protranHost.Application.Exit("Main form closed");
            Application.Exit();
        }

        private void RunProtran()
        {
            Thread.Sleep(100);

            this.protranHost = new ApplicationHost<ProtranApplication>();

            IbisVisualizationService.Register();
            AbuDhabiVisualizationService.Register();
            IOVisualizationService.Register();
            Vdv301VisualizationService.Register();

            this.protranHost.Application.Protran.ProtocolHost.ProtocolStarted += this.OnProtocolHostOnProtocolStarted;
            this.protranHost.Run("Protran");
        }

        private void OnProtocolHostOnProtocolStarted(object s, ProtocolEventArgs e)
        {
            this.splash.Invoke(new MethodInvoker(() => this.OpenChildForm(e.Protocol)));
        }

        private void OpenChildForm(IProtocol protocol)
        {
            var childForm = this.CreateChildForm(protocol);
            if (childForm != null)
            {
                if (this.logForm == null)
                {
                    // create the log form only once and use it as a trigger to hide the splash screen
                    this.logForm = new ApplicationLogForm();
                    this.logForm.VisibleChanged += (s, e) => this.splash.Hide();
                    this.AttachChildToParent(this.logForm);
                    this.mainForm.Show();
                }

                this.AttachChildToParent(childForm);
            }
            else
            {
                MessageBox.Show(
                    this.splash,
                    @"Protran Visualizer does not yet support " + protocol.GetType().Name,
                    @"Unsupported Protocol",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
            }
        }

        private Form CreateChildForm(IProtocol protocol)
        {
            if (protocol is IbisProtocol)
            {
                return new IbisMainForm();
            }

            if (protocol is AbuDhabiProtocol)
            {
                var abuDhabiForm = new AbuDhabiMainForm();
                var ibisForm = abuDhabiForm.CreateIbisForm();
                if (ibisForm != null)
                {
                    this.AttachChildToParent(ibisForm);
                }

                return abuDhabiForm;
            }

            if (protocol is IOProtocol)
            {
                return new IOMainForm();
            }

            if (protocol is Vdv301Protocol)
            {
                return new Vdv301MainForm();
            }

            if (protocol is XimpleProtocolImpl)
            {
                MessageBox.Show("Ximple over Sockets protocol not yet supported");
            }

            return null;
        }

        private void AttachChildToParent(Form form)
        {
            form.MdiParent = this.mainForm;
            form.Show();
            form.WindowState = FormWindowState.Normal;
        }
    }
}