namespace HtmlRendererTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Gorba.Motion.Infomedia.RendererBase;

    using HtmlRendererTest.Renderers;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class HtmlClientHandler : IHtmlRenderContext
    {
        private readonly string sessionId;

        private readonly List<RendererBase> renderers = new List<RendererBase>();

        private readonly AutoResetEvent updateWait = new AutoResetEvent(false);

        private readonly List<JsonUpdate> updates = new List<JsonUpdate>();

        private readonly JsonSerializer serializer;

        private bool hasNewScreen = false;

        public HtmlClientHandler(string sessionId)
        {
            this.sessionId = sessionId;
            this.serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented, // TODO: remove debugging
                };
            this.serializer.Converters.Add(new StringEnumConverter());
        }

        int IRenderContext.MillisecondsCounter
        {
            get
            {
                return Environment.TickCount;
            }
        }

        string IHtmlRenderContext.SessionId
        {
            get
            {
                return this.sessionId;
            }
        }

        public void SetRenderers(IEnumerable<RendererBase> newRenderers)
        {
            foreach (var renderer in this.renderers)
            {
                renderer.JsonUpdated -= this.RendererOnJsonUpdated;
            }

            lock (((ICollection)this.renderers).SyncRoot)
            {
                this.renderers.Clear();
                this.renderers.AddRange(newRenderers);
            }

            foreach (var renderer in this.renderers)
            {
                renderer.JsonUpdated += this.RendererOnJsonUpdated;
            }

            this.hasNewScreen = true;
            lock (this.updates)
            {
                this.updates.Clear();
            }

            this.TriggerUpdate();
        }

        public void TriggerUpdate()
        {
            if (this.hasNewScreen || this.updates.Count > 0)
            {
                this.updateWait.Set();
            }
        }

        public bool HandleRequest(HttpListenerContext context)
        {
            if (context.Request.Url.AbsolutePath == "/")
            {
                this.HandlePageRequest(context);
                return true;
            }

            if (context.Request.Url.AbsolutePath == "/updates")
            {
                this.HandleUpdateRequest(context);
                return true;
            }

            return false;
        }

        private void HandlePageRequest(HttpListenerContext context)
        {
            HttpUtilities.SetContentType(context.Response, ".html");
            HttpUtilities.SetBrowserCaching(context.Response, false);
            using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<title>Infomedia 2 HTML Renderer</title>");
                writer.WriteLine("<!-- Version {0} -->", FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).FileVersion);
                writer.WriteLine("<link href=\"/Styles.css?sid={0}\" rel=\"stylesheet\" type=\"text/css\" />", this.sessionId);
                writer.WriteLine("<script language=\"javascript\">var updateUrl = '/updates?sid={0}';</script>", this.sessionId);
                writer.WriteLine("<script src=\"/jquery.js?sid={0}\" type=\"text/javascript\"></script>", this.sessionId);
                writer.WriteLine("<script src=\"/Scripts.js?sid={0}\" type=\"text/javascript\"></script>", this.sessionId);
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }

        private void HandleUpdateRequest(HttpListenerContext context)
        {
            JsonUpdate[] currentUpdates;
            lock (this.updates)
            {
                currentUpdates = this.updates.ToArray();
                this.updates.Clear();
            }

            HttpUtilities.SetContentType(context.Response, ".json");
            HttpUtilities.SetBrowserCaching(context.Response, false);
            using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                var update = new JsonUpdates();
                if (this.updateWait.WaitOne(TimeSpan.FromSeconds(50), true))
                {
                    if (this.hasNewScreen)
                    {
                        this.hasNewScreen = false;
                        update.Screen = this.renderers.ConvertAll(r => r.CreateJsonObject());
                    }
                    else
                    {
                        update.Updates = new List<JsonUpdate>(currentUpdates);
                    }
                }

                this.serializer.Serialize(writer, update);
            }
        }

        private void RendererOnJsonUpdated(object sender, JsonUpdateEventArgs e)
        {
            lock (this.updates)
            {
                this.updates.Add(e.Update);
            }
        }
    }
}
