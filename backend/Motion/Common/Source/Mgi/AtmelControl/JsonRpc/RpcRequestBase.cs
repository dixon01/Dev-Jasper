// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcRequestBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcRequestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Base class for JSON-RPC request and notification.
    /// </summary>
    /// <seealso cref="http://www.jsonrpc.org/specification#request_object"/>
    public abstract class RpcRequestBase : RpcObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RpcRequestBase"/> class.
        /// </summary>
        internal RpcRequestBase()
        {
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the method to be called.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public string method { get; set; }

        /// <summary>
        /// Gets or sets the parameters of the method to be called.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public object @params { get; set; }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets the <see cref="@params"/> property converted to a given type.
        /// </summary>
        /// <typeparam name="T">
        /// The type expected in the <code>params</code> object.
        /// </typeparam>
        /// <returns>
        /// An instance of <see cref="T"/> or the default value if
        /// <see cref="@params"/> is null.
        /// </returns>
        public T GetParams<T>()
        {
            return RpcObject.Convert<T>(this.@params);
        }
    }
}