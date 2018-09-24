// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AhdlcRendererApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AhdlcRendererApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host;
    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Handlers;
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// The AHDLC renderer application.
    /// </summary>
    public class AhdlcRendererApplication : ApplicationBase
    {
        private readonly ManualResetEvent runWait = new ManualResetEvent(false);

        private readonly ITimer initialRequestTimer;

        private readonly AhdlcRendererConfig config;

        private readonly List<ChannelHandler> channels = new List<ChannelHandler>();

        private readonly List<SignHandler> missingRenderers = new List<SignHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AhdlcRendererApplication"/> class.
        /// </summary>
        public AhdlcRendererApplication()
        {
            this.initialRequestTimer = TimerFactory.Current.CreateTimer("InitialRequest");
            this.initialRequestTimer.Interval = TimeSpan.FromSeconds(3);
            this.initialRequestTimer.AutoReset = true;
            this.initialRequestTimer.Elapsed += this.InitialRequestTimerOnElapsed;

            var configMgr = new ConfigManager<AhdlcRendererConfig>();
            configMgr.FileName = PathManager.Instance.GetPath(FileType.Config, "AhdlcRenderer.xml");
            configMgr.EnableCaching = true;
            configMgr.XmlSchema = AhdlcRendererConfig.Schema;

            this.config = configMgr.Config;
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// This method should not return until after <see cref="ApplicationBase.Stop"/> was called.
        /// </summary>
        protected override void DoRun()
        {
            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.OnScreenChange);
            this.initialRequestTimer.Enabled = true;

            foreach (var channelConfig in this.config.Channels)
            {
                var handler = new ChannelHandler();
                handler.Configure(channelConfig, this.config);
                this.channels.Add(handler);
            }

            foreach (var handler in this.channels)
            {
                handler.Start();
                foreach (var renderer in handler.Handlers)
                {
                    this.missingRenderers.Add(renderer);
                }
            }

            this.SetRunning();

            this.runWait.WaitOne();

            foreach (var channel in this.channels)
            {
                channel.Stop();
            }

            this.channels.Clear();
        }

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Stop"/> method.
        /// This method should stop whatever is running in <see cref="DoRun()"/>.
        /// </summary>
        protected override void DoStop()
        {
            MessageDispatcher.Instance.Unsubscribe<ScreenChanges>(this.OnScreenChange);
            this.initialRequestTimer.Enabled = false;

            this.runWait.Set();
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            lock (this.missingRenderers)
            {
                foreach (var screenChange in e.Message.Changes)
                {
                    if (screenChange.Screen.Type != PhysicalScreenType.LED)
                    {
                        continue;
                    }

                    var found = false;
                    foreach (var channel in this.channels)
                    {
                        if (this.LoadScreen(channel, screenChange))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Logger.Warn("Couldn't find handler for screen {0}", screenChange.Screen);
                    }
                }
            }

            e.Message.Updates.Sort((a, b) => a.RootId.CompareTo(b.RootId));

            if (e.Message.Updates.Count > 0)
            {
                foreach (var channel in this.channels)
                {
                    channel.UpdateScreen(e.Message.Updates);
                }
            }

            this.initialRequestTimer.Enabled = this.missingRenderers.Count > 0;
        }

        private bool LoadScreen(ChannelHandler channel, ScreenChange screenChange)
        {
            foreach (var renderer in channel.Handlers)
            {
                if (renderer.LoadScreen(screenChange))
                {
                    this.missingRenderers.Remove(renderer);
                    return true;
                }
            }

            return false;
        }

        private void InitialRequestTimerOnElapsed(object sender, EventArgs e)
        {
            var request = new ScreenRequests();
            lock (this.missingRenderers)
            {
                foreach (var renderer in this.missingRenderers)
                {
                    request.Screens.Add(
                        new ScreenRequest
                        {
                            ScreenId = new ScreenId { Type = PhysicalScreenType.LED, Id = renderer.Id },
                            Width = renderer.Width,
                            Height = renderer.Height
                        });
                }
            }

            MessageDispatcher.Instance.Broadcast(request);
        }
    }
}
