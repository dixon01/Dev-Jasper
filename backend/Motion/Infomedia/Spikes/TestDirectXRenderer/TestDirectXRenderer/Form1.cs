// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Form1.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using Microsoft.DirectX.Direct3D;

    using NLog;

    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// The form 1.
    /// </summary>
    public partial class Form1 : Form, IItemDrawContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ScreenItemDrawer> drawers = new List<ScreenItemDrawer>();

        private readonly Timer initialRequestTimer;

        private Device device;

        private PresentParameters presentationParameters;

        private RootItem currentScreen;

        private int firstTime;

        private bool blinkOn = true;

        private int alternationCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque, false);

            this.initialRequestTimer = new Timer { Interval = 2000 };
            this.initialRequestTimer.Tick += (o, args) => MessageDispatcher.Instance.Broadcast(new ScreenRequest());

            try
            {
                this.CreateDevice();
                this.LoadDefaultScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        bool IItemDrawContext.BlinkOn
        {
            get
            {
                return this.blinkOn;
            }
        }

        int IItemDrawContext.AlternationCounter
        {
            get
            {
                return this.alternationCounter;
            }
        }

        /// <summary>
        /// Renders the DirectX surface.
        /// </summary>
        public void Render()
        {
            if (this.device == null || this.device.Disposed)
            {
                try
                {
                    this.CreateDevice();
                    this.LoadScreen(this.currentScreen);
                }
                catch (Exception)
                {
                    // don't retry immediately; is there a nicer way around this?
                    Thread.Sleep(100);
                    return;
                }
            }

            var screen = this.currentScreen;
            if (screen != null && 
                (screen.Width != this.presentationParameters.BackBufferWidth || 
                screen.Height != this.presentationParameters.BackBufferHeight))
            {
                this.presentationParameters.BackBufferWidth = screen.Width;
                this.presentationParameters.BackBufferHeight = screen.Height;
                this.ResetDevice();
            }

            if (this.firstTime == 0)
            {
                this.firstTime = Environment.TickCount;
            }

            int diff = Environment.TickCount - this.firstTime;
            this.alternationCounter = diff / 1000;
            this.blinkOn = ((diff / 500) % 2) == 0;

            try
            {
                // Clear the backbuffer to a black color 
                this.device.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);

                // Begin the scene
                this.device.BeginScene();

                lock (((ICollection)this.drawers).SyncRoot)
                {
                    // Rendering of scene objects
                    foreach (var drawer in this.drawers)
                    {
                        if (drawer.Item.Visible)
                        {
                            drawer.Draw(this);
                        }
                    }
                }

                // End the scene
                this.device.EndScene();
                this.device.Present();
            }
            catch (DeviceLostException)
            {
                this.RecreateDevice();
            }
            catch (DeviceNotResetException)
            {
                this.ResetDevice();
            }
            catch (Exception ex)
            {
                this.WindowState = FormWindowState.Minimized;
                Application.DoEvents();
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MessageDispatcher.Instance.Subscribe<ScreenChange>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<ScreenUpdate>(this.OnScreenUpdate);

            this.initialRequestTimer.Start();
        }

        /*protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Alt && e.KeyCode == Keys.Enter)
            {
                this.fullScreen = !this.fullScreen;
                this.RecreateDevice();
            }
        }*/

        /// <summary>
        /// Initializes the graphics.
        /// </summary>
        private void CreateDevice()
        {
            var adapter = Manager.Adapters.Default;

            this.presentationParameters = new PresentParameters
                {
                    BackBufferCount = 1,
                    DeviceWindow = this,
                    Windowed = true,
                    SwapEffect = SwapEffect.Discard,
                    PresentationInterval = PresentInterval.Immediate,
                    BackBufferFormat = Format.A8R8G8B8
                };

            var screen = this.currentScreen;
            if (screen != null)
            {
                this.presentationParameters.BackBufferWidth = screen.Width;
                this.presentationParameters.BackBufferHeight = screen.Height;
            }

            this.device = new Device(
                adapter.Adapter,
                DeviceType.Hardware,
                this,
                CreateFlags.SoftwareVertexProcessing | CreateFlags.MultiThreaded,
                this.presentationParameters);

            // disable buffer resizing, we want to present it in the given screen resolution
            this.device.DeviceResizing += (sender, args) => args.Cancel = true;
        }

        private void ResetDevice()
        {
            this.device.Reset(this.presentationParameters);
        }

        private void RecreateDevice()
        {
            var oldDevice = this.device;
            this.device = null;
            oldDevice.Dispose();
        }

        private void LoadDefaultScreen()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.GetType().Namespace + ".DefaultScreen.xml");
            if (stream == null)
            {
                return;
            }

            var serializer = new XmlSerializer(typeof(RootItem));
            var screen = (RootItem)serializer.Deserialize(stream);
            stream.Dispose();
            screen.Width = this.ClientSize.Width;
            screen.Height = this.ClientSize.Height;
            this.LoadScreen(screen);
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChange> e)
        {
            this.initialRequestTimer.Stop();

            this.LoadScreen(e.Message.Root);
        }

        private void OnScreenUpdate(object sender, MessageEventArgs<ScreenUpdate> e)
        {
            lock (((ICollection)this.drawers).SyncRoot)
            {
                foreach (var drawer in this.drawers)
                {
                    foreach (var update in e.Message.Updates)
                    {
                        if (update.ScreenItemId != drawer.Item.Id)
                        {
                            continue;
                        }

                        drawer.Item.Update(update);
                    }
                }
            }
        }

        private void LoadScreen(RootItem screen)
        {
            var newDrawers = new List<ScreenItemDrawer>();
            foreach (var item in screen.Items)
            {
                try
                {
                    var drawer = ScreenItemDrawer.Create(item);
                    drawer.Prepare(this.device);
                    newDrawers.Add(drawer);
                }
                catch (Exception ex)
                {
                    Logger.WarnException("Could not create drawer", ex);
                }
            }

            if (newDrawers.Count == 0)
            {
                return;
            }

            newDrawers.Sort((left, right) => left.Item.ZIndex - right.Item.ZIndex);
            ScreenItemDrawer[] oldDrawers;

            lock (((ICollection)this.drawers).SyncRoot)
            {
                oldDrawers = this.drawers.ToArray();
                this.drawers.Clear();
                this.drawers.AddRange(newDrawers);
                this.currentScreen = screen;
            }

            foreach (var drawer in oldDrawers)
            {
                drawer.Dispose();
            }
        }
    }
}