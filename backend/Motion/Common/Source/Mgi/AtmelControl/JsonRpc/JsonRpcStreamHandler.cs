// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcStreamHandler.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcStreamHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using NLog;

    /// <summary>
    /// Reader and writer for a JSON-RPC stream.
    /// </summary>
    public class JsonRpcStreamHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Stream stream;

        private readonly JsonStreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcStreamHandler"/> class.
        /// </summary>
        /// <param name="stream">
        /// The underlying stream.
        /// </param>
        public JsonRpcStreamHandler(Stream stream)
        {
            this.stream = stream;
            this.reader = new JsonStreamReader(stream);
        }

        /// <summary>
        /// Writes the given object synchronously to the underlying stream.
        /// </summary>
        /// <param name="obj">
        /// The object to write.
        /// </param>
        public void Write(RpcObject obj)
        {
            var text = JsonConvert.SerializeObject(obj);
            Logger.Trace("Writing: {0}", text);
            var data = Encoding.ASCII.GetBytes(text);
            this.stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Reads an object synchronously from the underlying stream.
        /// </summary>
        /// <returns>
        /// The <see cref="RpcObject"/> read from the stream.
        /// This can be one of the following types:
        /// - <see cref="RpcRequest"/>
        /// - <see cref="RpcNotification"/>
        /// - <see cref="RpcResponse"/>
        /// </returns>
        /// <exception cref="JsonReaderException">
        /// If the object read is not a well-formed JSON-RPC 2.0 object.
        /// </exception>
        public RpcObject Read()
        {
            var obj = this.reader.ReadObject() as JObject;
            if (obj == null)
            {
                throw new JsonReaderException("Couldn't read object from stream: object not a JObject");
            }

            JToken versionToken;
            obj.TryGetValue("jsonrpc", out versionToken);
            var version = versionToken as JValue;
            if (version == null || version.ToString() != "2.0")
            {
                throw new JsonReaderException(string.Format("Unsupported JSON-RPC version: '{0}'", version));
            }

            JToken methodToken;
            obj.TryGetValue("method", out methodToken);
            var method = methodToken as JValue;

            if (method != null)
            {
                JToken idToken;
                if (obj.TryGetValue("id", out idToken))
                {
                    return obj.ToObject<RpcRequest>();
                }

                return obj.ToObject<RpcNotification>();
            }

            return obj.ToObject<RpcResponse>();
        }
    }
}