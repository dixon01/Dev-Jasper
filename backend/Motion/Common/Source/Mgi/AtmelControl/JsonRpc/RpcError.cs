// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcError.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcError type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The JSON-RPC error object.
    /// </summary>
    /// <seealso cref="http://www.jsonrpc.org/specification#error_object"/>
    public sealed class RpcError
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public int code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public string message { get; set; }

        /// <summary>
        /// Gets or sets the error data.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public object data { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets the <see cref="data"/> property converted to a given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type expected in the data object.
        /// </typeparam>
        /// <returns>
        /// An instance of <see cref="T"/> or the default value if
        /// <see cref="data"/> is null.
        /// </returns>
        public T GetData<T>()
        {
            return RpcObject.Convert<T>(this.data);
        }
    }
}