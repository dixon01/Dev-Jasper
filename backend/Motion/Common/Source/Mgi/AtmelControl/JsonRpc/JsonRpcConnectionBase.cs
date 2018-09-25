// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRpcConnectionBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRpcConnectionBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Threading;

    using Gorba.Common.Utility.Core.Async;

    using NLog;

    /// <summary>
    /// Base class for a JSON-RPC connection.
    /// </summary>
    public abstract class JsonRpcConnectionBase : IDisposable
    {
        /// <summary>
        /// The logger for subclasses.
        /// </summary>
        protected readonly Logger Logger;

        private readonly Dictionary<string, Action<RpcNotification>> localNotifications =
            new Dictionary<string, Action<RpcNotification>>();

        private readonly Dictionary<string, RpcMethod> localMethods = new Dictionary<string, RpcMethod>();

        private readonly Dictionary<int, SimpleAsyncResult<RpcResponse>> asyncRequests =
            new Dictionary<int, SimpleAsyncResult<RpcResponse>>();

        private int currentId;

        private TcpClient client;

        private JsonRpcStreamHandler streamHandler;

        private int readThreadId;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcConnectionBase"/> class.
        /// </summary>
        protected JsonRpcConnectionBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        public virtual void Close()
        {
            this.Disconnect();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Starts this client by listening to the given TCP client.
        /// </summary>
        /// <param name="tcpClient">
        /// The TCP client.
        /// </param>
        protected void Start(TcpClient tcpClient)
        {
            this.client = tcpClient;
            this.streamHandler = new JsonRpcStreamHandler(this.client.GetStream());

            var thread = new Thread(this.ReadLoop);
            thread.IsBackground = true;
            thread.Start();
            this.readThreadId = thread.ManagedThreadId;
        }

        /// <summary>
        /// Disconnects the underlying socket.
        /// </summary>
        protected virtual void Disconnect()
        {
            var tcpClient = this.client;
            this.client = null;
            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't close connection");
                }
            }
        }

        /// <summary>
        /// Adds a local notification method that can be called from the server.
        /// </summary>
        /// <param name="methodName">
        /// The method name used by the server to identify the method.
        /// </param>
        /// <param name="method">
        /// The method that will be called.
        /// The argument is the <see cref="RpcNotification"/> coming from the other peer.
        /// </param>
        protected void AddLocalNotification(string methodName, Action<RpcNotification> method)
        {
            this.localNotifications.Add(methodName, method);
        }

        /// <summary>
        /// Adds a local method that can be called from the server.
        /// </summary>
        /// <param name="methodName">
        /// The method name used by the server to identify the method.
        /// </param>
        /// <param name="method">
        /// The method that will be called.
        /// The argument is the <see cref="RpcRequest"/> coming from the other peer.
        /// </param>
        protected void AddLocalMethod(string methodName, RpcMethod method)
        {
            this.localMethods.Add(methodName, method);
        }

        /// <summary>
        /// Begins to invoke a remote method.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <param name="callback">
        /// The async callback.
        /// </param>
        /// <param name="state">
        /// The async state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndInvokeRemoteMethod"/>.
        /// </returns>
        protected IAsyncResult BeginInvokeRemoteMethod(
            string methodName, object args, AsyncCallback callback, object state)
        {
            var id = Interlocked.Increment(ref this.currentId);

            var request = new RpcRequest
                              {
                                  jsonrpc = "2.0",
                                  id = id.ToString(CultureInfo.InvariantCulture),
                                  method = methodName,
                                  @params = args
                              };

            var result = new SimpleAsyncResult<RpcResponse>(callback, state);
            lock (this.asyncRequests)
            {
                this.asyncRequests.Add(id, result);
            }

            this.SendObject(request);

            return result;
        }

        /// <summary>
        /// Ends the remote method call invoked with <see cref="BeginInvokeRemoteMethod"/>.
        /// </summary>
        /// <param name="ar">
        /// The async result returned by <see cref="BeginInvokeRemoteMethod"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type of the return value.
        /// </typeparam>
        /// <returns>
        /// The return value of the remote call.
        /// </returns>
        protected T EndInvokeRemoteMethod<T>(IAsyncResult ar)
        {
            return this.EndInvokeRemoteMethod(ar).GetResult<T>();
        }

        /// <summary>
        /// Ends the remote method call invoked with <see cref="BeginInvokeRemoteMethod"/>.
        /// </summary>
        /// <param name="ar">
        /// The async result returned by <see cref="BeginInvokeRemoteMethod"/>.
        /// </param>
        /// <returns>
        /// The return value of the remote call.
        /// </returns>
        protected RpcResponse EndInvokeRemoteMethod(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<RpcResponse>;
            if (result == null)
            {
                throw new ArgumentException("Provide async result returned by BeginInvokeRemoteMethod()", "ar");
            }

            if (!result.IsCompleted && this.InReadThread())
            {
                throw new InvalidOperationException("Can't call EndInvokeRemoteMethod() from the message read thread");
            }

            result.AsyncWaitHandle.WaitOne();
            return result.Value;
        }

        /// <summary>
        /// Synchronously invokes a remote method.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <typeparam name="T">
        /// The type of the return value.
        /// </typeparam>
        /// <returns>
        /// The return value of the remote call.
        /// </returns>
        protected T InvokeRemoteMethod<T>(string methodName, object args)
        {
            var result = this.BeginInvokeRemoteMethod(methodName, args, null, null);
            return this.EndInvokeRemoteMethod<T>(result);
        }

        /// <summary>
        /// Synchronously invokes a remote method.
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The return value of the remote call.
        /// </returns>
        protected RpcResponse InvokeRemoteMethod(string methodName, object args)
        {
            var result = this.BeginInvokeRemoteMethod(methodName, args, null, null);
            return this.EndInvokeRemoteMethod(result);
        }

        /// <summary>
        /// Sends an object to the remote server.
        /// </summary>
        /// <param name="obj">
        /// The object to send.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If we are not connected to the server.
        /// </exception>
        protected void SendObject(RpcObject obj)
        {
            if (this.streamHandler == null)
            {
                throw new InvalidOperationException("Can't send an object if not connected");
            }

            this.streamHandler.Write(obj);
        }

        /// <summary>
        /// Checks if we are inside the read thread.
        /// This can be used to determine if we are allowed to make synchronous calls
        /// that are waiting for a result from the server.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool InReadThread()
        {
            return Thread.CurrentThread.ManagedThreadId == this.readThreadId;
        }

        private void HandleResponse(RpcResponse response)
        {
            var id = Convert.ToInt32(response.id);
            SimpleAsyncResult<RpcResponse> result;
            lock (this.asyncRequests)
            {
                if (this.asyncRequests.TryGetValue(id, out result))
                {
                    this.asyncRequests.Remove(id);
                }
            }

            if (result == null)
            {
                this.Logger.Warn("Unknown result id {0}", id);
                return;
            }

            if (response.error == null)
            {
                result.Complete(response, false);
                return;
            }

            try
            {
                throw new JsonRpcException(response.error);
            }
            catch (Exception ex)
            {
                result.CompleteException(ex, false);
            }
        }

        private void ReadLoop()
        {
            try
            {
                while (this.streamHandler != null)
                {
                    var obj = this.streamHandler.Read();
                    try
                    {
                        this.HandleReceivedObject(obj);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Warn(ex, "Exception while handling received object");
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.streamHandler == null)
                {
                    // we were closed, so of course we get an exception
                    return;
                }

                this.Logger.Warn(ex, "Exception while reading");
                this.Disconnect();
            }
        }

        private void HandleReceivedObject(RpcObject obj)
        {
            var response = obj as RpcResponse;
            if (response != null)
            {
                this.HandleResponse(response);
                return;
            }

            var notification = obj as RpcNotification;
            if (notification != null)
            {
                this.HandleNotification(notification);
                return;
            }

            var request = obj as RpcRequest;
            if (request != null)
            {
                this.HandleRequest(request);
                return;
            }

            this.Logger.Warn("Unknown type of RPC object: {0}", obj.GetType().FullName);
        }

        private void HandleNotification(RpcNotification notification)
        {
            Action<RpcNotification> method;
            if (!this.localNotifications.TryGetValue(notification.method, out method))
            {
                this.Logger.Warn("Unknown method: {0}", notification.method);
                return;
            }

            try
            {
                method(notification);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while calling notification: " + notification.method);
            }
        }

        private void HandleRequest(RpcRequest request)
        {
            RpcResponse response;
            try
            {
                RpcMethod method;
                if (!this.localMethods.TryGetValue(request.method, out method))
                {
                    throw new JsonRpcException(-32601, "Method not found");
                }

                var result = method(request);
                response = new RpcResponse { id = request.id, result = result };
            }
            catch (JsonRpcException ex)
            {
                this.Logger.Warn(ex, "Exception while calling method: " + request.method);
                var error = new RpcError { code = ex.Code, message = ex.Message };
                response = new RpcResponse { id = request.id, error = error };
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while calling method: " + request.method);

                var error = new RpcError { code = -32000, message = ex.Message };
                response = new RpcResponse { id = request.id, error = error };
            }

            this.SendObject(response);
        }
    }
}