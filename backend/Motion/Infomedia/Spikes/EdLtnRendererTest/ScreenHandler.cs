namespace Gorba.Motion.Infomedia.EdLtnRendererTest
{
    using System;
    using System.Threading;
    using System.Timers;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Properties;
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol;
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Renderer;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using NLog;

    using Timer = System.Timers.Timer;

    internal class ScreenHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ScreenId = "LTN";

        private readonly RenderManagerFactory renderManagerFactory;

        private readonly RenderContext renderContext = new RenderContext();

        private LtnSerialPort serialPort;

        private ScreenRootRenderManager<ILtnRenderContext> rootRenderer;

        private Timer initialRequestTimer;

        public ScreenHandler()
        {
            this.serialPort = new LtnSerialPort(Settings.Default.ComPort);
            this.renderManagerFactory = new RenderManagerFactory(this.serialPort);
        }

        public void Start()
        {
            this.serialPort.Open();

            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.OnScreenChange);

            this.initialRequestTimer = new Timer { Interval = 3000 };
            this.initialRequestTimer.Elapsed += this.InitialRequestTimerOnElapsed;
            this.initialRequestTimer.Start();
        }

        private void InitialRequestTimerOnElapsed(object s, ElapsedEventArgs e)
        {
            var request = new ScreenRequests
                              {
                                  Screens =
                                      {
                                          new ScreenRequest
                                              {
                                                  ScreenId =
                                                      new ScreenId
                                                          {
                                                              Type = PhysicalScreenType.LED,
                                                              Id = ScreenId
                                                          },
                                                  Width = 240, // TODO: remove constant size
                                                  Height = 72
                                              }
                                      }
                              };
            MessageDispatcher.Instance.Broadcast(request);
        }

        public void Stop()
        {
            var timer = this.initialRequestTimer;
            if (timer != null)
            {
                timer.Stop();
            }

            this.serialPort.Close();

            MessageDispatcher.Instance.Unsubscribe<ScreenChanges>(this.OnScreenChange);
        }

        private void Render(RenderState state)
        {
            var renderer = this.rootRenderer;
            if (renderer == null)
            {
                return;
            }

            lock (renderer)
            {
                this.renderContext.State = state;
                renderer.Update(this.renderContext);
                renderer.Render(1, this.renderContext);
            }
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            if (this.initialRequestTimer != null)
            {
                this.initialRequestTimer.Stop();
                this.initialRequestTimer = null;
            }

            var change =
                e.Message.Changes.Find(
                    c => c.Screen.Type == PhysicalScreenType.LED && (c.Screen.Id == ScreenId || c.Screen.Id == null));
            if (change == null)
            {
                var renderer = this.rootRenderer;
                if (renderer == null || e.Message.Updates.Count == 0)
                {
                    return;
                }

                renderer.Update(e.Message.Updates);
                this.Render(RenderState.Update);
                return;
            }

            this.serialPort.EnqueueTelegram(new ClearLayout());
            this.renderContext.Reset();

            lock (this.renderManagerFactory)
            {
                if (this.rootRenderer != null)
                {
                    this.rootRenderer.Dispose();
                    this.rootRenderer = null;
                }

                this.rootRenderer = this.renderManagerFactory.CreateRenderManager(change.ScreenRoot);
            }

            this.Render(RenderState.Setup);

            // display needs some time...
            Thread.Sleep(500);
            this.Render(RenderState.Update);
        }

        private class RenderContext : ILtnRenderContext
        {
            private int nextCellNumber;

            public int MillisecondsCounter { get; private set; }

            public RenderState State { get; set; }

            public byte GetNextCellNumber()
            {
                var number = Interlocked.Increment(ref this.nextCellNumber);
                if (number >= 96)
                {
                    throw new IndexOutOfRangeException("Can't have more than 96 cells");
                }

                return (byte)number;
            }

            public void Reset()
            {
                this.nextCellNumber = -1;
            }
        }
    }
}
