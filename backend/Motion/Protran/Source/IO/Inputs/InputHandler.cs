// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InputHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Inputs
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Utils;

    using NLog;

    /// <summary>
    /// The handler for an input from a GIOoP port.
    /// </summary>
    public class InputHandler : IInputHandler, IManageableObject
    {
        private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(10);

        private static readonly Logger Logger = LogHelper.GetLogger<InputHandler>();

        private readonly InputHandlingConfig config;

        private readonly TransformationChain chain;

        private readonly MediAddress address;

        private readonly GenericUsageHandler usage;

        private PortListener portListener;

        private string currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="chain">
        /// The chain.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public InputHandler(InputHandlingConfig config, TransformationChain chain, Dictionary dictionary)
        {
            this.config = config;
            this.chain = chain;

            this.address = new MediAddress();
            this.address.Unit = !string.IsNullOrEmpty(this.config.Unit)
                               ? this.config.Unit
                               : MessageDispatcher.Instance.LocalAddress.Unit;
            this.address.Application = !string.IsNullOrEmpty(this.config.Application) ? this.config.Application : "*";

            this.usage = new GenericUsageHandler(this.config.UsedFor, dictionary);
        }

        /// <summary>
        /// Event that is fired if this handler created some data.
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the name of the input.
        /// </summary>
        public string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        /// <summary>
        /// Starts this input handler.
        /// </summary>
        public void Start()
        {
            this.portListener = new PortListener(this.address, this.config.Name);
            this.portListener.ValueChanged += this.PortListenerOnValueChanged;
            this.portListener.Start(RetryInterval);
        }

        /// <summary>
        /// Stops this input handler.
        /// </summary>
        public void Stop()
        {
            if (this.portListener != null)
            {
                this.portListener.Dispose();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            if (string.IsNullOrEmpty(this.currentValue))
            {
                yield return new ManagementProperty<string>("Current Input State", "Not available", true);
            }
            else
            {
                yield return new ManagementProperty<string>("Current Input State", this.currentValue, true);
            }
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SendXimple()
        {
            if (this.config.UsedFor == null || this.portListener == null || this.portListener.Value == null)
            {
                return;
            }

            var value = this.chain.Transform(this.portListener.Value);
            if (this.currentValue == value)
            {
                return;
            }

            Logger.Debug("Sending value '{0}' for port {1}", value, this.config.Name);

            this.currentValue = value;
            var ximple = new Ximple();
            this.usage.AddCell(ximple, value);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }

        private void PortListenerOnValueChanged(object sender, EventArgs e)
        {
            this.SendXimple();
        }
    }
}