namespace Gorba.Motion.Infomedia.AnnaxRendererTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Timers;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.AnnaxRendererTest.Commands;
    using Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using MinimalisticTelnet;

    using NLog;

    using Timer = System.Timers.Timer;

    internal class ScreenHandler : IRenderContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ScreenId = "Annax";

        private readonly List<RendererBase> renderers = new List<RendererBase>();

        private readonly int width;

        private readonly int height;

        private int nextBitmapNumber;

        private TelnetConnection connection;

        private Timer initialRequestTimer;

        private ScreenRoot currentScreen;

        public ScreenHandler(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void Start(TelnetConnection connection)
        {
            this.connection = connection;

            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<ScreenUpdate>(this.OnScreenUpdate);

            this.initialRequestTimer = new Timer { Interval = 3000 };
            this.initialRequestTimer.Elapsed += this.InitialRequestTimerOnElapsed;
            this.initialRequestTimer.Start();
        }

        private void InitialRequestTimerOnElapsed(object s, ElapsedEventArgs e)
        {
            MessageDispatcher.Instance.Broadcast(
                new ScreenRequests
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
                                        Width = this.width,
                                        Height = this.height
                                    }
                            }
                    });
        }

        int IRenderContext.GetAvailableBitmapNumber()
        {
            return this.nextBitmapNumber++;
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            foreach (var screenChange in e.Message.Changes)
            {
                if (screenChange.Screen.Type != PhysicalScreenType.LED
                    || (screenChange.Screen.Id != ScreenId && screenChange.Screen.Id != null))
                {
                    continue;
                }

                if (this.initialRequestTimer != null)
                {
                    this.initialRequestTimer.Stop();
                    this.initialRequestTimer = null;
                }

                this.LoadScreen(screenChange.ScreenRoot);
                return;
            }
        }

        private void OnScreenUpdate(object sender, MessageEventArgs<ScreenUpdate> e)
        {
            lock (this.renderers)
            {
                if (this.currentScreen == null || this.currentScreen.Root.Id != e.Message.RootId)
                {
                    return;
                }

                var commands = new List<CommandBase>();
                foreach (var renderer in this.renderers)
                {
                    foreach (var update in e.Message.Updates)
                    {
                        commands.AddRange(renderer.Update(update, this));
                    }
                }

                this.SendCommands(commands);
            }
        }

        private void LoadScreen(ScreenRoot screen)
        {
            var newRenderers = new List<RendererBase>();
            foreach (var item in screen.Root.Items)
            {
                try
                {
                    var renderer = RendererBase.Create(item);
                    newRenderers.Add(renderer);
                }
                catch (Exception ex)
                {
                    Logger.WarnException("Could not create renderer", ex);
                }
            }

            if (newRenderers.Count == 0)
            {
                return;
            }

            newRenderers.Sort((left, right) => left.Item.ZIndex - right.Item.ZIndex);
            RendererBase[] oldRenderers;

            lock (this.renderers)
            {
                oldRenderers = this.renderers.ToArray();
                this.renderers.Clear();
                this.renderers.AddRange(newRenderers);
                this.currentScreen = screen;
            }

            foreach (var renderer in oldRenderers)
            {
                renderer.Dispose();
            }

            this.RenderScreen();
        }

        private void RenderScreen()
        {
            lock (this.renderers)
            {
                this.nextBitmapNumber = 0;
                var commands = new List<CommandBase>();
                commands.Add(new ClearCommand());
                foreach (var renderer in this.renderers)
                {
                    commands.AddRange(renderer.Setup(this));
                }

                this.SendCommands(commands);
            }
        }

        private void SendCommands(IEnumerable<CommandBase> commands)
        {
            var buffer = new StringBuilder();
            foreach (var command in commands)
            {
                command.AppendCommandString(buffer);
            }

            this.connection.WriteLine(buffer.ToString());

            this.ReadUntilDone();
        }

        private void ReadUntilDone()
        {
            ThreadPool.QueueUserWorkItem(
                s =>
                {
                    string read;
                    while ((read = this.connection.Read()) != null && read.Length > 0)
                    {
                        Console.WriteLine(read);
                    }

                    Console.WriteLine("-");
                });
        }
    }
}
