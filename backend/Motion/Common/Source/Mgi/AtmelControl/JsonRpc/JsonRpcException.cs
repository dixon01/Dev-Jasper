// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown when JSON-RPC reports an error as the response to a remote request.
    /// </summary>
    [Serializable]
    public class JsonRpcException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcException"/> class.
        /// </summary>
        public JsonRpcException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcException"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public JsonRpcException(int code, string message)
            : base(CreateMessage(code, message))
        {
            this.Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcException"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public JsonRpcException(int code, string message, Exception inner)
            : base(CreateMessage(code, message), inner)
        {
            this.Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcException"/> class.
        /// </summary>
        /// <param name="error">
        /// The JSON-RPC error.
        /// </param>
        internal JsonRpcException(RpcError error)
            : this(error.code, error.message)
        {
            this.Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected JsonRpcException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// Gets the JSON-RPC error.
        /// </summary>
        public RpcError Error { get; private set; }

        private static string CreateMessage(int code, string message)
        {
            return string.Format("{0} ({1})", message, code);
        }
    }
}