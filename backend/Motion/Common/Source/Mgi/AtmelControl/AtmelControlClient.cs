// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AtmelControlClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AtmelControlClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Common.Mgi.AtmelControl.Args;
    using Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JSON-RPC implementation to talk to the AtmelControl application.
    /// </summary>
    public class AtmelControlClient : JsonRpcClient
    {
        private readonly Dictionary<string, List<Action<RpcNotification>>> objectCallbacks =
            new Dictionary<string, List<Action<RpcNotification>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AtmelControlClient"/> class.
        /// </summary>
        public AtmelControlClient()
        {
            this.AddLocalNotification("notifyObject", this.NotifyObject);
        }

        /// <summary>
        /// Registers an object with AtmelControl.
        /// AtmelControl will then send instances of the given type back whenever
        /// data changes.
        /// </summary>
        /// <param name="callback">
        /// The callback that will be called whenever an object has arrived.
        /// </param>
        /// <typeparam name="T">
        /// The type of object; has to be a known subclass of <see cref="AtmelControlObject"/>.
        /// </typeparam>
        public void RegisterObject<T>(Action<T> callback)
            where T : AtmelControlObject
        {
            var typeName = typeof(T).Name;
            List<Action<RpcNotification>> callbacks;
            lock (this.objectCallbacks)
            {
                this.objectCallbacks.TryGetValue(typeName, out callbacks);
                if (callbacks == null)
                {
                    callbacks = new List<Action<RpcNotification>>();
                    this.objectCallbacks.Add(typeName, callbacks);
                }
            }

            lock (callbacks)
            {
                callbacks.Add(n => callback(n.GetParams<T>()));
                if (callbacks.Count > 1)
                {
                    return;
                }
            }

            this.BeginInvokeRemoteMethod("registerObject", new[] { typeName }, this.ObjectRegistered, typeName);
        }

        /// <summary>
        /// Sets the display backlight value for a given panel.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="panelNo">
        /// The panel number.
        /// </param>
        /// <param name="backlightValue">
        /// The backlight value.
        /// The brightness range is 0..255 for direct brightness and -1 for automatic regulation.
        /// </param>
        public void SetDisplayBacklight(int address, int panelNo, int backlightValue)
        {
            this.InvokeRemoteMethodAsync(
                "displaySetBacklightValue",
                new DisplaySetBacklightValueArgs
                    {
                        Address = address,
                        PanelNo = panelNo,
                        BacklightValue = backlightValue
                    });
        }

        /// <summary>
        /// Set parameters for automatic backlight regulation.
        /// </summary>
        /// <param name="address">
        /// The panel address.
        /// </param>
        /// <param name="panelNo">
        /// The panel number.
        /// </param>
        /// <param name="minimum">
        /// The minimum backlight value.
        /// </param>
        /// <param name="maximum">
        /// The maximum backlight value.
        /// </param>
        /// <param name="speed">
        /// The speed of backlight regulation
        /// 1 – slow (~ 1 minute)
        /// 10 – fast (instantly)
        /// </param>
        public void SetDisplayBacklightParams(int address, int panelNo, int minimum, int maximum, int speed)
        {
            this.InvokeRemoteMethodAsync(
                "displaySetBacklightParams",
                new DisplaySetBacklightParamsArgs
                {
                    Address = address,
                    PanelNo = panelNo,
                    Minimum = minimum,
                    Maximum = maximum,
                    Speed = speed
                });
        }

        /// <summary>
        /// Sets the display contrast value for a given panel.
        /// Value is only adjustable if input signal is stable.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="panelNo">
        /// The panel number.
        /// </param>
        /// <param name="contrast">
        /// The contrast value.
        /// </param>
        public void SetDisplayContrast(int address, int panelNo, int contrast)
        {
            this.InvokeRemoteMethodAsync(
                "displaySetContrast",
                new DisplaySetContrastArgs
                {
                    Address = address,
                    PanelNo = panelNo,
                    Contrast = contrast
                });
        }

        /// <summary>
        /// Sets the display sharpness value for a given panel.
        /// Value is only adjustable if input signal is stable.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="panelNo">
        /// The panel number.
        /// </param>
        /// <param name="sharpness">
        /// The sharpness value.
        /// </param>
        public void SetDisplaySharpness(int address, int panelNo, int sharpness)
        {
            this.InvokeRemoteMethodAsync(
                "displaySetSharpness",
                new DisplaySetSharpnessArgs
                {
                    Address = address,
                    PanelNo = panelNo,
                    Sharpness = sharpness
                });
        }

        /// <summary>
        /// Sets the display color balance for a given panel.
        /// Value is only adjustable if input signal is stable.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="panelNo">
        /// The panel number.
        /// </param>
        /// <param name="red">
        /// The red color balance value.
        /// </param>
        /// <param name="green">
        /// The green color balance value.
        /// </param>
        /// <param name="blue">
        /// The blue color balance value.
        /// </param>
        public void SetDisplayColor(int address, int panelNo, int red, int green, int blue)
        {
            this.InvokeRemoteMethodAsync(
                "displaySetColor",
                new DisplaySetColorArgs
                {
                    Address = address,
                    PanelNo = panelNo,
                    Red = red,
                    Green = green,
                    Blue = blue
                });
        }

        /// <summary>
        /// Sets the IBIS address for this machine.
        /// </summary>
        /// <param name="address">
        /// The IBIS address (1..15).
        /// </param>
        public void SetIbisAddress(int address)
        {
            this.InvokeRemoteMethodAsync("ibisSetAddress", new IbisSetAddressArgs { Address = address });
        }

        /// <summary>
        /// Sets the IBIS reply value for this machine.
        /// </summary>
        /// <param name="replyValue">
        /// The reply value (0..15).
        /// </param>
        public void SetIbisReplyValue(int replyValue)
        {
            this.InvokeRemoteMethodAsync("ibisSetReplyValue", new IbisSetReplyValueArgs { Value = replyValue });
        }

        /// <summary>
        /// Sets the RS485 interface connection switch.
        /// </summary>
        /// <param name="iface">
        /// RS485 Interface switch state.
        /// Default: <see cref="Rs485Interface.At91"/>
        /// </param>
        public void SetRs485InterfaceConnectionSwitch(Rs485Interface iface)
        {
            this.InvokeRemoteMethodAsync(
                "systemSetRS485InterfaceConnectionSwitch",
                new SystemSetRs485InterfaceConnectionSwitchArgs { Value = (int)iface });
        }

        /// <summary>
        /// Gets the RS485 interface connection switch.
        /// </summary>
        /// <returns>
        /// RS485 Interface switch state.
        /// Default: <see cref="Rs485Interface.At91"/>
        /// </returns>
        public Rs485Interface GetRs485InterfaceConnectionSwitch()
        {
            return (Rs485Interface)this.InvokeRemoteMethod<int>(
                "systemGetRS485InterfaceConnectionSwitch", new object());
        }

        /// <summary>
        /// Raises the <see cref="JsonRpcClient.ConnectedChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseConnectedChanged(EventArgs e)
        {
            if (!this.Connected)
            {
                // clear the list of callbacks when we disconnect (users must subscribe again)
                lock (this.objectCallbacks)
                {
                    this.objectCallbacks.Clear();
                }
            }

            base.RaiseConnectedChanged(e);
        }

        private void InvokeRemoteMethodAsync(string methodName, object args)
        {
            this.BeginInvokeRemoteMethod(
                methodName, args, this.EndInvokeRemoteMethodAsync, string.Format("{0}({1})", methodName, args));
        }

        private void EndInvokeRemoteMethodAsync(IAsyncResult ar)
        {
            var result = this.EndInvokeRemoteMethod<int>(ar);
            if (result == 0)
            {
                this.Logger.Debug("Got result 0 from {0}", ar.AsyncState);
            }
            else
            {
                this.Logger.Warn("Got result {1} from {0}", ar.AsyncState, result);
            }
        }

        private void ObjectRegistered(IAsyncResult ar)
        {
            var result = this.EndInvokeRemoteMethod<int>(ar);
            if (result == 0)
            {
                this.Logger.Debug("Got result 0 from registerObject({0})", ar.AsyncState);
            }
            else
            {
                this.Logger.Warn("Got result {1} from registerObject({0})", ar.AsyncState, result);
            }
        }

        private void NotifyObject(RpcNotification notification)
        {
            var typeName = notification.GetParams<JObject>()["objectName"].ToString();
            List<Action<RpcNotification>> callbacks;
            lock (this.objectCallbacks)
            {
                this.objectCallbacks.TryGetValue(typeName, out callbacks);
            }

            if (callbacks == null || callbacks.Count == 0)
            {
                return;
            }

            lock (callbacks)
            {
                foreach (var callback in callbacks)
                {
                    callback(notification);
                }
            }
        }
    }
}
