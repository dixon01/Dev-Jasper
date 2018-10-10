namespace HtmlRendererTest
{
    using System;
    using System.Net;
    using System.Net.Mime;

    public static class HttpUtilities
    {
        public static void SetContentType(HttpListenerResponse response, string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".css":
                    response.ContentType = "text/css";
                    return;
                case ".gif":
                    response.ContentType = MediaTypeNames.Image.Gif;
                    return;
                case ".htm":
                case ".html":
                    response.ContentType = "text/html; charset=utf-8";
                    return;
                case ".jpg":
                case ".jpeg":
                    response.ContentType = MediaTypeNames.Image.Jpeg;
                    return;
                case ".js":
                    response.ContentType = "text/javascript";
                    return;
                case ".json":
                    response.ContentType = "application/json";
                    return;
                case ".png":
                    response.ContentType = "image/png";
                    return;
                case ".xml":
                    response.ContentType = "text/xml; charset=utf-8";
                    return;
                default:
                    response.ContentType = MediaTypeNames.Text.Plain;
                    return;
            }
        }

        public static void SetBrowserCaching(HttpListenerResponse response, bool cacheEnabled)
        {
            var now = DateTime.Now;
            response.Headers.Add(HttpResponseHeader.LastModified, now.ToString("r"));

            if (cacheEnabled)
            {
                var expires = 24 * 60 * 60; // one day
                response.Headers.Add(HttpResponseHeader.Expires, now.AddSeconds(expires).ToString("r"));
                response.Headers.Add(HttpResponseHeader.CacheControl, "max-age=" + expires);
                response.Headers.Add(HttpResponseHeader.Pragma, "public");
                return;
            }

            response.Headers.Add(HttpResponseHeader.Expires, "-1");
            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
            response.Headers.Add(HttpResponseHeader.Pragma, "no-cache");
        }
    }
}