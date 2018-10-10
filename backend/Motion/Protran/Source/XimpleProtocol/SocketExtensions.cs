// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="SocketExtensions.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>The socket extensions.</summary>
    public static class SocketExtensions
    {
        #region Public Methods and Operators

        /// <summary>The receive async.</summary>
        /// <param name="socket">The socket.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        /// <param name="socketFlags">The socket flags.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public static Task<int> ReceiveAsync(this Socket socket, byte[] buffer, int offset, int size, SocketFlags socketFlags = SocketFlags.None)
        {
            var tcs = new TaskCompletionSource<int>(socket);
            socket.BeginReceive(
                buffer, 
                offset, 
                size, 
                socketFlags, 
                asyncResult =>
                    {
                        var t = (TaskCompletionSource<int>)asyncResult.AsyncState;
                        var s = (Socket)t.Task.AsyncState;
                        try
                        {
                            t.TrySetResult(s.EndReceive(asyncResult));
                        }
                        catch (Exception exc)
                        {
                            t.TrySetException(exc);
                        }
                    }, 
                tcs);
            return tcs.Task;
        }

        #endregion
    }
}