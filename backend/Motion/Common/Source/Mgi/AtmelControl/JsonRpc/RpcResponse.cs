// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The JSON-RPC response.
    /// </summary>
    /// <seealso cref="http://www.jsonrpc.org/specification#response_object"/>
    public sealed class RpcResponse : RpcObject
    {
        private object identifier;

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the id of the response.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public object id
        {
            get
            {
                return this.identifier;
            }

            set
            {
                if (!(value is int) && !(value is long) && !(value is string))
                {
                    throw new ArgumentException("Id has to be a number (int/long) or string.");
                }

                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets the result object.
        /// Can be null in case of an error.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public object result { get; set; }

        /// <summary>
        /// Gets or sets the error if an error occurred.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public RpcError error { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets the <see cref="result"/> property converted to a given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type expected in the result object.
        /// </typeparam>
        /// <returns>
        /// An instance of <see cref="T"/> or the default value if
        /// <see cref="result"/> is null.
        /// </returns>
        public T GetResult<T>()
        {
            return RpcObject.Convert<T>(this.result);
        }
    }
}