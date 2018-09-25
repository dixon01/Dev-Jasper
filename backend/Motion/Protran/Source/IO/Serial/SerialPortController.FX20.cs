// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialPortController.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerialPortController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Serial
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;

    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Controller for I/O's on a single serial port.
    /// </summary>
    public partial class SerialPortController
    {
        private readonly Logger logger;
        private readonly SerialPortConfig config;

        private readonly List<IOPortHandler> portHandlers = new List<IOPortHandler>();

        private ISerialPortIOs ios;

        private SerialPort serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialPortController"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public SerialPortController(SerialPortConfig config)
        {
            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + config.Name);
            this.config = config;
        }

        private delegate bool IOPinGetter();

        private delegate void IOPinSetter(bool value);

        /// <summary>
        /// Gets the name of the serial port.
        /// </summary>
        public string Name
        {
            get
            {
                return this.config.Name;
            }
        }

        /// <summary>
        /// Starts this controller and creates all <see cref="IPort"/>s for the configured ports.
        /// </summary>
        public void Start()
        {
            try
            {
                this.ios = ServiceLocator.Current.GetInstance<ISerialPortIOs>(this.config.Name.ToUpper());
                this.logger.Debug("Port is provided by another application");
            }
            catch (ActivationException)
            {
                this.logger.Debug("Port is not provided by anybody, creating it");
                this.serialPort = new SerialPort(this.config.Name);
                this.serialPort.Open();
                this.ios = new SerialPortIOHandler { SerialPort = this.serialPort };
            }

            this.CreateIOPorts();
        }

        /// <summary>
        /// Stops this controller and closes all <see cref="IPort"/>s for the configured ports.
        /// </summary>
        public void Stop()
        {
            foreach (var port in this.portHandlers)
            {
                port.Dispose();
            }

            this.portHandlers.Clear();

            if (this.serialPort != null)
            {
                this.serialPort.Close();
                this.serialPort.Dispose();
                this.serialPort = null;
            }
        }

        private void CreateIOPorts()
        {
            this.CreateIOPorts(
                "RTS",
                this.config.Rts,
                () => this.ios.RtsEnable,
                v => this.ios.RtsEnable = v,
                e => this.ios.RtsChanged += e);
            this.CreateIOPorts("CTS", this.config.Cts, () => this.ios.CtsHolding, null, e => this.ios.CtsChanged += e);

            this.CreateIOPorts(
                "DTR",
                this.config.Dtr,
                () => this.ios.DtrEnable,
                v => this.ios.DtrEnable = v,
                e => this.ios.DtrChanged += e);
            this.CreateIOPorts("DSR", this.config.Dsr, () => this.ios.DsrHolding, null, e => this.ios.DsrChanged += e);
        }

        private void CreateIOPorts(
            string name,
            string alias,
            IOPinGetter getter,
            IOPinSetter setter,
            Action<EventHandler> addEventHandler)
        {
            var defaultName = string.Format("{0}.{1}", this.config.Name, name);
            this.CreateIOPort(defaultName, getter, setter, addEventHandler);
            if (!string.IsNullOrEmpty(alias) && defaultName != alias)
            {
                this.CreateIOPort(alias, getter, setter, addEventHandler);
            }
        }

        private void CreateIOPort(
            string name,
            IOPinGetter getter,
            IOPinSetter setter,
            Action<EventHandler> addEventHandler)
        {
            var port = new IOPortHandler(name, getter, setter, addEventHandler, this.logger);
            this.portHandlers.Add(port);
            port.Start();
        }

        private class IOPortHandler : IDisposable
        {
            private readonly string name;

            private readonly IOPinGetter getter;
            private readonly IOPinSetter setter;

            private readonly Logger logger;

            private readonly IPort port;

            private IOValue lastValue;

            public IOPortHandler(
                string name,
                IOPinGetter getter,
                IOPinSetter setter,
                Action<EventHandler> addEventHandler,
                Logger logger)
            {
                this.name = name;
                this.getter = getter;
                this.setter = setter;
                this.logger = logger;

                this.port = new SimplePort(
                    name, true, setter != null, new FlagValues(), FlagValues.GetValue(!getter()));
                addEventHandler(this.PinOnValueChanged);
                if (this.setter != null)
                {
                    this.port.ValueChanged += this.PortOnValueChanged;
                }
            }

            public void Start()
            {
                GioomClient.Instance.RegisterPort(this.port);
            }

            public void Dispose()
            {
                GioomClient.Instance.DeregisterPort(this.port);
                this.port.Dispose();
            }

            private void PinOnValueChanged(object sender, EventArgs e)
            {
                var value = FlagValues.GetValue(!this.getter());
                if (value.Equals(this.lastValue))
                {
                    return;
                }

                this.logger.Debug("Serial port pin changed, setting {0} to {1}", this.name, value);
                this.port.Value = value;
                this.lastValue = value;
            }

            private void PortOnValueChanged(object sender, EventArgs e)
            {
                this.setter(this.port.Value.Equals(FlagValues.False));
                this.logger.Debug("Serial port pin change requested, setting {0} to {1}", this.name, this.port.Value);
            }
        }
    }
}