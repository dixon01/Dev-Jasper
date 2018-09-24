namespace HtmlRendererTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Mime;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    using HtmlRendererTest.Renderers;

    using NLog;

    internal class Server
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private const string ScreenId = "HTML";

        private readonly HttpListener listener;

        private readonly List<RendererBase> renderers = new List<RendererBase>();

        private readonly Dictionary<string, HtmlClientHandler> clientHandlers = new Dictionary<string, HtmlClientHandler>();

        private ScreenRoot currentScreen;

        public Server()
        {
            this.listener = new HttpListener();
            this.listener.Prefixes.Add("http://*:5555/");
            MessageDispatcher.Instance.Subscribe<ScreenChanges>(this.HandleScreenChange);
        }

        public void Start()
        {
            this.listener.Start();
            this.listener.BeginGetContext(this.HandleConnection, null);

            Thread.Sleep(TimeSpan.FromSeconds(2));
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
                                                    Type = PhysicalScreenType.TFT,
                                                    Id = ScreenId
                                                },
                                        Width = 1440, // TODO: remove hardcoded size
                                        Height = 900 // TODO: remove hardcoded size
                                    }
                            }
                    });
        }

        private void HandleConnection(IAsyncResult ar)
        {
            var context = this.listener.EndGetContext(ar);
            this.listener.BeginGetContext(this.HandleConnection, null);

            try
            {
                this.HandleRequest(context);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Could not handle request", ex);
                try
                {
                    context.Response.Close();
                }
                catch (Exception closeEx)
                {
                    Logger.DebugException("Could not close response", closeEx);
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var file = ImageProvider.Instance.GetFileForPath(context.Request.Url.AbsolutePath);
            if (file != null)
            {
                if (file.Exists)
                {
                    this.SendFile(file, context.Response);
                }
                else
                {
                    this.SendNotFound(context);
                }

                return;
            }

            var resource =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    this.GetType().Namespace + "." + context.Request.Url.AbsolutePath.Substring(1));
            if (resource != null)
            {
                HttpUtilities.SetContentType(context.Response, Path.GetExtension(context.Request.Url.AbsolutePath));
                HttpUtilities.SetBrowserCaching(context.Response, true);
                this.SendFrom(resource, context.Response);
                return;
            }

            var sessionId = context.Request.QueryString["sid"];
            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
            }

            HtmlClientHandler handler;
            if (!this.clientHandlers.TryGetValue(sessionId, out handler))
            {
                // TODO: we need a timeout for the handler if it isn't used anymore
                handler = new HtmlClientHandler(sessionId);
                handler.SetRenderers(this.renderers);
                this.clientHandlers.Add(sessionId, handler);
            }

            if (!handler.HandleRequest(context))
            {
                this.SendNotFound(context);
            }
        }

        private void SendNotFound(HttpListenerContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "Not Found";
            context.Response.ContentType = MediaTypeNames.Text.Html;
            HttpUtilities.SetBrowserCaching(context.Response, false);
            using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine(
                    "<HTML><BODY><H1>404 Page Not Found</H1>The page {0} could not be found</BODY></HTML>",
                    context.Request.RawUrl);
            }
        }

        private void SendFile(FileInfo file, HttpListenerResponse response)
        {
            HttpUtilities.SetContentType(response, file.Extension);
            HttpUtilities.SetBrowserCaching(response, true);

            using (var input = file.OpenRead())
            {
                this.SendFrom(input, response);
            }
        }

        private void SendFrom(Stream input, HttpListenerResponse response)
        {
            var output = response.OutputStream;
            var buffer = new byte[1024];
            int r;
            while ((r = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, r);
            }

            output.Close();
        }

        private void HandleScreenChange(object sender, MessageEventArgs<ScreenChanges> e)
        {
            foreach (var screenChange in e.Message.Changes)
            {
                if (screenChange.Screen.Type != PhysicalScreenType.TFT
                    || (screenChange.Screen.Id != ScreenId && screenChange.Screen.Id != null))
                {
                    continue;
                }

                this.LoadScreen(screenChange.ScreenRoot);
                return;
            }

            foreach (var screenUpdate in e.Message.Updates)
            {
                foreach (var update in screenUpdate.Updates)
                {
                    lock (((ICollection)this.renderers).SyncRoot)
                    {
                        foreach (var renderer in this.renderers)
                        {
                            renderer.UpdateItem(update);
                        }
                    }
                }
            }

            foreach (var clientHandler in this.clientHandlers.Values)
            {
                clientHandler.TriggerUpdate();
            }
        }

        private void LoadScreen(ScreenRoot screen)
        {
            this.currentScreen = screen;
            var newRenderers = new List<RendererBase>();
            foreach (var item in screen.Root.Items)
            {
                try
                {
                    var renderer = RendererBase.Create(item);
                    renderer.Prepare();
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

            lock (((ICollection)this.renderers).SyncRoot)
            {
                oldRenderers = this.renderers.ToArray();
                this.renderers.Clear();
                this.renderers.AddRange(newRenderers);
                this.currentScreen = screen;
            }

            foreach (var clientHandler in this.clientHandlers.Values)
            {
                clientHandler.SetRenderers(this.renderers);
            }

            foreach (var renderer in oldRenderers)
            {
                renderer.Dispose();
            }
        }
    }
}