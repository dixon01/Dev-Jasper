// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Utility.Core.Async;

    /// <summary>
    /// The local Gorba I/O over Medi client.
    /// </summary>
    public sealed class GioomClient : GioomClientBase, IManageable
    {
        private static volatile GioomClient instance;

        private GioomClient()
            : base(MessageDispatcher.Instance)
        {
            this.LocalPorts = new Dictionary<string, IPort>();

            var root = MessageDispatcher.Instance.ManagementProviderFactory.LocalRoot;
            var provider = MessageDispatcher.Instance.ManagementProviderFactory.CreateManagementProvider(
                "GIOoM", root, this);
            root.AddChild(provider);

            this.Start();
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static GioomClient Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (typeof(GioomClient))
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    return instance = new GioomClient();
                }
            }
        }

        /// <summary>
        /// Gets the configured local ports.
        /// Don't add or remove ports directly, but rather use <see cref="RegisterPort"/>
        /// and <see cref="DeregisterPort"/> instead.
        /// </summary>
        internal Dictionary<string, IPort> LocalPorts { get; private set; }

        /// <summary>
        /// Registers a new port with this client.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If a port with the same name (see <see cref="IPortInfo.Name"/>) already exists.
        /// </exception>
        public void RegisterPort(IPort port)
        {
            lock (this.LocalPorts)
            {
                if (this.LocalPorts.ContainsKey(port.Info.Name))
                {
                    throw new ArgumentException("Can't register port twice: " + port.Info.Name, "port");
                }

                this.LocalPorts.Add(port.Info.Name, port);
            }
        }

        /// <summary>
        /// Deregisters a port from this client.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        public void DeregisterPort(IPort port)
        {
            lock (this.LocalPorts)
            {
                this.LocalPorts.Remove(port.Info.Name);
            }
        }

        /// <summary>
        /// Begins the search for ports on the given address.
        /// </summary>
        /// <param name="address">
        /// The address of the node(s) to search for ports.
        /// Wildcards can be used to search in all applications on a given unit.
        /// </param>
        /// <param name="timeout">
        /// The timeout after which this asynchronous method should complete.
        /// This method completes immediately if <see cref="address"/> is the local Medi address.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="GioomClientBase.EndFindPorts"/>.
        /// </returns>
        public override IAsyncResult BeginFindPorts(
            MediAddress address, TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (this.IsLocal(address))
            {
                var result = new SimpleAsyncResult<IPortInfo[]>(callback, state);
                var infos = new List<IPortInfo>(this.LocalPorts.Count);
                foreach (var port in this.LocalPorts.Values)
                {
                    infos.Add(port.Info);
                }

                result.Complete(infos.ToArray(), true);
                return result;
            }

            return base.BeginFindPorts(address, timeout, callback, state);
        }

        /// <summary>
        /// Opens a port.
        /// </summary>
        /// <param name="info">
        /// The information about the port. You can get this from <see cref="GioomClientBase.BeginFindPorts"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPort"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If a local port is used and that port does not exist (anymore).
        /// </exception>
        public override IPort OpenPort(IPortInfo info)
        {
            if (this.IsLocal(info.Address))
            {
                IPort port;
                if (!this.LocalPorts.TryGetValue(info.Name, out port))
                {
                    throw new ArgumentException("Couldn't find local port " + info.Name);
                }

                return new LocalPortWrapper(port);
            }

            return base.OpenPort(info);
        }

        /// <summary>
        /// Begins to open a port on the given address with the given name.
        /// This method will complete synchronously if the address is the local address.
        /// This asynchronous request times out after 10 seconds if the port was not found.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="GioomClientBase.EndOpenPort"/>.
        /// </returns>
        public override IAsyncResult BeginOpenPort(
            MediAddress address, string name, AsyncCallback callback, object state)
        {
            if (this.IsLocal(address))
            {
                var result = new SimpleAsyncResult<IPort>(callback, state);
                IPort port;
                this.LocalPorts.TryGetValue(name, out port);
                result.Complete(port == null ? null : new LocalPortWrapper(port), true);
                return result;
            }

            return base.BeginOpenPort(address, name, callback, state);
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var port in this.LocalPorts)
            {
                yield return parent.Factory.CreateManagementProvider(port.Key, parent, new ManageablePort(port.Value));
            }
        }

        /// <summary>
        /// Creates a new <see cref="MessageHandler"/> that will handle all Medi messages related
        /// to this client.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="MessageHandler"/>.
        /// </returns>
        internal override MessageHandler CreateMessageHandler()
        {
            return new LocalMessageHandler(this.Dispatcher, this);
        }

        private bool IsLocal(MediAddress address)
        {
            return address.Equals(this.Dispatcher.LocalAddress);
        }

        /// <summary>
        /// Wrapper around a local port which is needed because when calling
        /// <see cref="Dispose"/> we don't want to dispose the actual port but
        /// just the registration with it.
        /// This class also makes sure, <see cref="set_Value"/> and <see cref="get_Value"/>
        /// can only be called if the port is writable/readable respectively.
        /// </summary>
        private class LocalPortWrapper : IPort
        {
            private readonly IPort port;

            public LocalPortWrapper(IPort port)
            {
                this.port = port;
                this.port.ValueChanged += this.PortOnValueChanged;
            }

            public event EventHandler ValueChanged;

            public IPortInfo Info
            {
                get
                {
                    return this.port.Info;
                }
            }

            public IOValue Value
            {
                get
                {
                    this.CheckRead();
                    return this.port.Value;
                }

                set
                {
                    this.CheckWrite();
                    this.port.Value = value;
                }
            }

            public int IntegerValue
            {
                get
                {
                    this.CheckRead();
                    return this.port.IntegerValue;
                }

                set
                {
                    this.CheckWrite();
                    this.port.IntegerValue = value;
                }
            }

            public IOValue CreateValue(int value)
            {
                return this.port.CreateValue(value);
            }

            public void Dispose()
            {
                this.port.ValueChanged -= this.PortOnValueChanged;
            }

            private void CheckWrite()
            {
                if (!this.Info.CanWrite)
                {
                    throw new NotSupportedException("Can't write to " + this.Info.Name);
                }
            }

            private void CheckRead()
            {
                if (!this.Info.CanRead)
                {
                    throw new NotSupportedException("Can't read from " + this.Info.Name);
                }
            }

            private void PortOnValueChanged(object sender, EventArgs e)
            {
                var handler = this.ValueChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        private class ManageablePort : IManageableObject
        {
            private readonly IPort port;

            public ManageablePort(IPort port)
            {
                this.port = port;
            }

            public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
            {
                yield break;
            }

            public IEnumerable<ManagementProperty> GetProperties()
            {
                yield return new ManagementProperty<string>("Name", this.port.Info.Name, true);
                yield return
                    new ManagementProperty<string>(
                        "Value", this.port.Info.CanRead ? this.port.Value.ToString() : "n/a", true);
                yield return new ManagementProperty<bool>("CanRead", this.port.Info.CanRead, true);
                yield return new ManagementProperty<bool>("CanWrite", this.port.Info.CanWrite, true);
                yield return new ManagementProperty<string>("ValidValues", this.port.Info.ValidValues.ToString(), true);
            }
        }
    }
}
