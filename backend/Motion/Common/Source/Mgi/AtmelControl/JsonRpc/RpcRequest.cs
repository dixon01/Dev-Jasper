// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcRequest.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The JSON-RPC request.
    /// </summary>
    /// <seealso cref="http://www.jsonrpc.org/specification#request_object"/>
    public sealed class RpcRequest : RpcRequestBase
    {
        private object identifier;

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the id of the request.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public object id
        {
            get
            {
                return this.identifier;
            }

            set
            {
                if (!(value is int) && !(value is string))
                {
                    throw new ArgumentException("Id has to be a number (int) or string.");
                }

                this.identifier = value;
            }
        }

        // ReSharper restore InconsistentNaming
    }
}