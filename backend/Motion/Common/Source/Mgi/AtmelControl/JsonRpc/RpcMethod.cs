// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcMethod.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcMethod type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    /// <summary>
    /// A local method that can be called through JSON-RPC.
    /// </summary>
    /// <param name="request">
    /// The JSON-RPC request that called this method.
    /// </param>
    /// <returns>
    /// The result object which will be used for <see cref="RpcResponse.result"/>.
    /// </returns>
    public delegate object RpcMethod(RpcRequest request);
}