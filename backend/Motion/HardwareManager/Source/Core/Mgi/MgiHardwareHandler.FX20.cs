// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MgiHardwareHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MgiHardwareHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Mgi
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.HardwareManager.Mgi;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.Mgi.AtmelControl;
    using Gorba.Motion.Common.Mgi.IO;

    using NLog;

    /// <summary>
    /// Handles all MGI topbox related hardware
    /// </summary>
    public partial class MgiHardwareHandler : IHardwareHandler, IManageableObject
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly MgiConfig config;

        private readonly Dictionary<int, BinaryPort> gpios = new Dictionary<int, BinaryPort>(8);
        private readonly List<BinaryPort> otherPorts = new List<BinaryPort>(8);
        private readonly List<IPort> levelShifterPorts = new List<IPort>(8);
        private readonly List<IPort> protocolPorts = new List<IPort>(8);
        private readonly SimplePort ignitionPort;
        private readonly SimplePort backlightPort;
        private readonly SimplePort rs485InterfacePort;

        private readonly IInputOutputManager inputOutputManager;

        private readonly ITimer updateTimer;

        private readonly AtmelControlClient atmelControlClient;

        private readonly InfovisionInputState inputState = new InfovisionInputState();

        private InfovisionSystemState systemState;

        private InfovisionDisplayState displayState;

        private bool initialBacklightValueSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="MgiHardwareHandler"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public MgiHardwareHandler(MgiConfig config)
        {
            this.config = config;
            if (config.Enabled)
            {
                this.inputOutputManager = InputOutputManagerFactory.Create();

                this.atmelControlClient = new AtmelControlClient();

                this.ignitionPort = new SimplePort("Ignition", true, false, new FlagValues(), FlagValues.True);
                this.backlightPort = new SimplePort("Backlight", true, true, new FlagValues(), FlagValues.True);
                this.rs485InterfacePort = new SimplePort(
                    "RS485Interface", true, true, EnumValues.FromEnum<Rs485Interface>(), (int)Rs485Interface.At91);

                this.updateTimer = TimerFactory.Current.CreateTimer(this.GetType().Name);
                this.updateTimer.AutoReset = true;
                this.updateTimer.Interval = config.PollingInterval;
                this.updateTimer.Elapsed += this.UpdateTimerOnElapsed;

                this.backlightPort.ValueChanged += this.BacklightPortOnValueChanged;
                this.rs485InterfacePort.ValueChanged += this.Rs485InterfacePortOnValueChanged;
            }
        }

        /// <summary>
        /// Gets the name of this handler.
        /// </summary>
        public string Name
        {
            get
            {
                return "MGI";
            }
        }

        /// <summary>
        /// Gets the serial number of the underlying hardware.
        /// </summary>
        public string SerialNumber
        {
            get
            {
                return this.systemState == null ? null : this.systemState.Serial;
            }
        }

        /// <summary>
        /// Starts the handler
        /// </summary>
        public void Start()
        {
            BinaryPort port;
            foreach (var pin in this.config.Gpio.Pins)
            {
                if (pin.Index < 0 || string.IsNullOrEmpty(pin.Name))
                {
                    continue;
                }

                var io = this.inputOutputManager.GetGpio(pin.Index);
                port = AddBinaryPort(pin.Name, io);
                this.gpios.Add(pin.Index, port);
            }

            port = AddBinaryPort(this.config.Button, this.inputOutputManager.Button);
            if (port != null)
            {
                this.otherPorts.Add(port);
            }

            port = AddBinaryPort(this.config.UpdateLed, this.inputOutputManager.UpdateLed);
            if (port != null)
            {
                this.otherPorts.Add(port);
            }

            this.atmelControlClient.Connect(port: this.config.AtmelControlPort);
            this.atmelControlClient.ConnectedChanged += this.OnAtmelControlClientConnectionChanged;

            if (this.config.DviLevelShifters.Count == 0)
            {
                // Create default level shifters configuration to set default values
                this.config.DviLevelShifters.Add(new DviLevelShifterConfig { Index = 1 });
                this.config.DviLevelShifters.Add(new DviLevelShifterConfig { Index = 2 });
            }

            foreach (var levelShifter in this.config.DviLevelShifters)
            {
                // levelShifter.Index is from 1 whereas we need the index from 0
                this.CreateLevelShifterPorts(levelShifter, levelShifter.Index - 1);
            }

            port = AddBinaryPort("GpsShortCircuit", this.inputOutputManager.GpsShortCircuit);
            if (port != null)
            {
                this.otherPorts.Add(port);
            }

            port = AddBinaryPort("GpsCurrentDetection", this.inputOutputManager.GpsCurrentDetection);
            if (port != null)
            {
                this.otherPorts.Add(port);
            }

            foreach (var transceiver in this.config.Transceivers)
            {
                // transceiver.Index is from 1 whereas we need the index from 0
                this.CreateProtocolPorts(transceiver, transceiver.Index - 1);
            }

            GioomClient.Instance.RegisterPort(this.ignitionPort);
            GioomClient.Instance.RegisterPort(this.backlightPort);

            this.updateTimer.Enabled = true;
        }

        /// <summary>
        /// Stops the handler
        /// </summary>
        public void Stop()
        {
            this.updateTimer.Enabled = false;
            foreach (var port in this.gpios.Values)
            {
                GioomClient.Instance.DeregisterPort(port);
            }

            foreach (var port in this.otherPorts)
            {
                GioomClient.Instance.DeregisterPort(port);
            }

            this.gpios.Clear();
            this.otherPorts.Clear();

            foreach (var port in this.levelShifterPorts)
            {
                GioomClient.Instance.DeregisterPort(port);
            }

            foreach (var port in this.protocolPorts)
            {
                GioomClient.Instance.DeregisterPort(port);
            }

            this.levelShifterPorts.Clear();
            this.protocolPorts.Clear();

            GioomClient.Instance.DeregisterPort(this.ignitionPort);
            GioomClient.Instance.DeregisterPort(this.backlightPort);
            GioomClient.Instance.DeregisterPort(this.rs485InterfacePort);

            this.atmelControlClient.ConnectedChanged -= this.OnAtmelControlClientConnectionChanged;
            this.atmelControlClient.Close();
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            if (this.displayState == null)
            {
                yield break;
            }

            foreach (var device in this.displayState.Display)
            {
                if (device == null)
                {
                    continue;
                }

                yield return
                    parent.Factory.CreateManagementProvider(
                        "Display " + device.Address, parent, new ManageableDisplay(device));
            }
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            if (this.systemState != null)
            {
                yield return new ManagementProperty<string>("Serial", this.systemState.Serial ?? string.Empty, true);
                yield return new ManagementProperty<string>("HWRef", this.systemState.HWRef ?? string.Empty, true);
                yield return
                    new ManagementProperty<string>("At91Version", this.systemState.At91Version ?? string.Empty, true);
                yield return new ManagementProperty<string>("At91Rev", this.systemState.At91Rev ?? string.Empty, true);
            }

            if (this.inputState != null)
            {
                var ignition = this.inputState.Ignition.HasValue
                                   ? this.inputState.Ignition.Value.ToString(CultureInfo.InvariantCulture)
                                   : "n/a";
                var address = this.inputState.Address.HasValue
                                   ? this.inputState.Address.Value.ToString(CultureInfo.InvariantCulture)
                                   : "n/a";
                yield return new ManagementProperty<string>("Ignition", ignition, true);
                yield return new ManagementProperty<string>("IBIS Address", address, true);
            }
        }

        private static void RegisterPort(IPort port)
        {
            Logger.Info("Port '{0}' has value {1}", port.Info.Name, port.Value);
            GioomClient.Instance.RegisterPort(port);
        }

        private static BinaryPort AddBinaryPort(string name, IOBase io, bool? initialValue = null)
        {
            if (string.IsNullOrEmpty(name) || io == null)
            {
                return null;
            }

            var port = new BinaryPort(name, io);
            port.UpdateValue(io.Read());
            if (initialValue.HasValue)
            {
                port.SetPort(initialValue.Value);
            }

            RegisterPort(port);
            return port;
        }

        private void SetBacklightValue()
        {
            if (this.displayState == null)
            {
                return;
            }

            Logger.Info("Setting backlight to {0}", this.backlightPort.Value);
            var backlightValue = this.backlightPort.Value.Equals(FlagValues.True)
                                     ? this.config.DefaultBacklightValue
                                     : 0;

            foreach (var display in this.displayState.Display)
            {
                foreach (var panel in display.Panel)
                {
                    Logger.Debug(
                        "Setting backlight of display {0}, panel {1} to {2}",
                        display.Address,
                        panel.PanelNo,
                        backlightValue);
                    try
                    {
                        this.atmelControlClient.SetDisplayBacklight(display.Address, panel.PanelNo, backlightValue);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Method is not available");
                    }
                }
            }
        }

        private void SetBacklightParameters()
        {
            var rateConfig = this.config.BacklightControlRate;
            if (rateConfig == null)
            {
                return;
            }

            foreach (var display in this.displayState.Display)
            {
                foreach (var panel in display.Panel)
                {
                    Logger.Debug(
                        "Setting backlight control rate of display {0}, panel {1} to {2}..{3} at the rate {4}",
                        display.Address,
                        panel.PanelNo,
                        rateConfig.Minimum,
                        rateConfig.Maximum,
                        rateConfig.Speed);
                    try
                    {
                        this.atmelControlClient.SetDisplayBacklightParams(
                            display.Address,
                            panel.PanelNo,
                            rateConfig.Minimum,
                            rateConfig.Maximum,
                            rateConfig.Speed);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Method is not available");
                    }
                }
            }
        }

        private void OnAtmelControlClientConnectionChanged(object sender, EventArgs args)
        {
            if (this.atmelControlClient.Connected)
            {
                Logger.Info("Connected to AtmelControl");
                this.atmelControlClient.RegisterObject<InfovisionInputState>(this.ReadInputState);
                this.atmelControlClient.RegisterObject<InfovisionSystemState>(this.ReadSystemState);
                this.atmelControlClient.RegisterObject<InfovisionDisplayState>(this.ReadDisplayState);

                if (this.config.Rs485Interface.HasValue)
                {
                    var iface = this.config.Rs485Interface.Value;
                    Logger.Info("Initially setting RS-485 port to {0}", iface);
                    switch (iface)
                    {
                        case CompactRs485Switch.Cpu:
                            try
                            {
                                this.atmelControlClient.SetRs485InterfaceConnectionSwitch(Rs485Interface.Cpu);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, "Method is not available");
                            }

                            break;
                        case CompactRs485Switch.At91:
                            try
                            {
                                this.atmelControlClient.SetRs485InterfaceConnectionSwitch(Rs485Interface.At91);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex, "Method is not available");
                            }

                            break;
                        default:
                            Logger.Warn("Unknown RS-485 port switch value: {0}", iface);
                            break;
                    }
                }
                else
                {
                    try
                    {
                        var iface = this.atmelControlClient.GetRs485InterfaceConnectionSwitch();
                        Logger.Info("RS-485 port is set to {0}", iface);
                        this.rs485InterfacePort.IntegerValue = (int)iface;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Method is not available");
                    }
                }

                GioomClient.Instance.RegisterPort(this.rs485InterfacePort);
            }
            else
            {
                Logger.Info("Disconnected from AtmelControl");
            }
        }

        private void ReadInputState(InfovisionInputState state)
        {
            this.inputState.Address = state.Address ?? this.inputState.Address;
            this.inputState.Ignition = state.Ignition ?? this.inputState.Ignition;
            this.inputState.Stop0 = state.Stop0 ?? this.inputState.Stop0;
            this.inputState.Stop1 = state.Stop1 ?? this.inputState.Stop1;
            if (state.Ignition == null)
            {
                return;
            }

            Logger.Info("Ignition changed to: {0}", state.Ignition.Value);
            this.ignitionPort.IntegerValue = state.Ignition.Value;
        }

        private void ReadSystemState(InfovisionSystemState state)
        {
            if (this.systemState == null)
            {
                Logger.Info("Serial number: {0}", state.Serial);
                Logger.Info("Hardware reference: {0}", state.HWRef);
                Logger.Info("AT91 controller version: {0}", state.At91Version);
                Logger.Info("AT91 controller firmware: {0}", state.At91Rev);
            }

            this.systemState = state;
        }

        private void ReadDisplayState(InfovisionDisplayState state)
        {
            this.displayState = state;
            if (state == null || this.initialBacklightValueSet)
            {
                return;
            }

            this.initialBacklightValueSet = true;

            Logger.Info("Setting backlight to initial value");
            this.SetBacklightValue();
            this.SetBacklightParameters();
        }

        private void UpdateTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            var values = this.inputOutputManager.ReadAllGpios();
            for (int i = 0; i < values.Length; i++)
            {
                BinaryPort port;
                if (this.gpios.TryGetValue(i, out port))
                {
                    port.UpdateValue(values[i]);
                }
            }

            foreach (var port in this.otherPorts)
            {
                port.UpdateValue(port.IO.Read());
            }
        }

        private void BacklightPortOnValueChanged(object sender, EventArgs e)
        {
            this.SetBacklightValue();
        }

        private void Rs485InterfacePortOnValueChanged(object sender, EventArgs e)
        {
            var value = (Rs485Interface)this.rs485InterfacePort.IntegerValue;
            Logger.Info("Switching RS-485 port to {0}", value);
            try
            {
                this.atmelControlClient.SetRs485InterfaceConnectionSwitch(value);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Method is not available");
            }
        }

        private void CreateLevelShifterPorts(DviLevelShifterConfig levelShifter, int index)
        {
            var trimOutput = this.inputOutputManager.GetGraphicControlOutput(index, GraphicControlPin.Trim);
            if (trimOutput != null)
            {
                var trimPort = new BinaryEnumMgiPort<TrimOptions>(
                    "DviLevelShifter" + levelShifter.Index + ".Trim", trimOutput);
                trimPort.SetPort(levelShifter.Trim);
                RegisterPort(trimPort);
                this.levelShifterPorts.Add(trimPort);
            }

            var cct1Output = this.inputOutputManager.GetGraphicControlOutput(index, GraphicControlPin.Cct1);
            var cct2Output = this.inputOutputManager.GetGraphicControlOutput(index, GraphicControlPin.Cct2);
            if (cct1Output != null && cct2Output != null)
            {
                var levelPort = new LevelShifterLevelPort(
                    "DviLevelShifter" + levelShifter.Index + ".Level", cct1Output, cct2Output);
                levelPort.SetPort(levelShifter.OutputLevel);
                RegisterPort(levelPort);
                this.levelShifterPorts.Add(levelPort);
            }
        }

        private void CreateProtocolPorts(TransceiverConfig transceiver, int index)
        {
            var typeOutput = this.inputOutputManager.GetMultiprotocolTransceiverOutput(
                index, MultiprotocolTransceiverPin.Type);
            if (typeOutput != null)
            {
                var typePort = new BinaryEnumMgiPort<TransceiverType>(
                    "Transceiver" + transceiver.Index + ".Type", typeOutput);
                typePort.SetPort(transceiver.Type);
                RegisterPort(typePort);
                this.protocolPorts.Add(typePort);
            }

            var terminationOutput = this.inputOutputManager.GetMultiprotocolTransceiverOutput(
                index, MultiprotocolTransceiverPin.Termination);
            if (terminationOutput != null)
            {
                var terminationPort = AddBinaryPort(
                    "Transceiver" + transceiver.Index + ".Termination", terminationOutput, transceiver.Termination);
                this.protocolPorts.Add(terminationPort);
            }

            var modeOutput = this.inputOutputManager.GetMultiprotocolTransceiverOutput(
                index, MultiprotocolTransceiverPin.Mode);
            if (modeOutput != null)
            {
                var modePort = new BinaryEnumMgiPort<TransceiverMode>(
                    "Transceiver" + transceiver.Index + ".Mode", modeOutput);
                modePort.SetPort(transceiver.Mode);
                RegisterPort(modePort);
                this.protocolPorts.Add(modePort);
            }
        }

        private class ManageableDisplay : IManageableObject
        {
            private readonly InfovisionDisplayDevice device;

            public ManageableDisplay(InfovisionDisplayDevice device)
            {
                this.device = device;
            }

            public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
            {
                foreach (var panel in this.device.Panel)
                {
                    if (panel == null)
                    {
                        continue;
                    }

                    yield return
                        parent.Factory.CreateManagementProvider(
                            "Panel " + panel.PanelNo, parent, new ManageablePanel(panel));
                }
            }

            public IEnumerable<ManagementProperty> GetProperties()
            {
                yield return new ManagementProperty<int>("Address", this.device.Address, true);
                yield return
                    new ManagementProperty<string>("Connection State", this.device.ConnectionState.ToString(), true);
                yield return new ManagementProperty<bool>("Ignition", this.device.Ignition_On != 0, true);
                yield return new ManagementProperty<bool>("Power Hold", this.device.PowerHold_On != 0, true);
                yield return new ManagementProperty<bool>("Power State", this.device.PowerState != 0, true);
                yield return
                    new ManagementProperty<bool>("Backlight External 1", this.device.BacklightExternal_1_OK != 0, true);
                yield return
                    new ManagementProperty<bool>("Backlight External 2", this.device.BacklightExternal_2_OK != 0, true);
                yield return
                    new ManagementProperty<bool>("Backlight Internal 1", this.device.BacklightInternal_1_OK != 0, true);
                yield return
                    new ManagementProperty<bool>("Backlight Internal 2", this.device.BacklightInternal_2_OK != 0, true);
                yield return new ManagementProperty<bool>("Backlight 24V", this.device.Backlight24V_OK != 0, true);
                yield return new ManagementProperty<bool>("Genesis 1", this.device.GenesisPresent_1 != 0, true);
                yield return new ManagementProperty<bool>("Genesis 2", this.device.GenesisPresent_2 != 0, true);
                yield return
                    new ManagementProperty<string>(
                        "Backlight Mode 1", this.device.BacklightMode_1 == 0 ? "manual" : "automatic", true);
                yield return
                    new ManagementProperty<string>(
                        "Backlight Mode 2", this.device.BacklightMode_2 == 0 ? "manual" : "automatic", true);
            }
        }

        private class ManageablePanel : IManageableObject
        {
            private readonly InfovisionDisplayPanel panel;

            public ManageablePanel(InfovisionDisplayPanel panel)
            {
                this.panel = panel;
            }

            public IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent)
            {
                yield break;
            }

            public IEnumerable<ManagementProperty> GetProperties()
            {
                yield return new ManagementProperty<int>("Panel Number", this.panel.PanelNo, true);
                yield return new ManagementProperty<int>("Temperature", this.panel.Temperature, true);
                yield return new ManagementProperty<int>("Backlight Value", this.panel.BacklightValue, true);
                yield return new ManagementProperty<int>("Backlight Min", this.panel.BacklightMin, true);
                yield return new ManagementProperty<int>("Backlight Max", this.panel.BacklightMax, true);
                yield return new ManagementProperty<int>("Backlight Speed", this.panel.BacklightSpeed, true);
                yield return new ManagementProperty<int>("Lux", this.panel.Lux, true);
                yield return new ManagementProperty<int>("EQ Level", this.panel.EqLevel, true);
                yield return new ManagementProperty<bool>("Signal Stable", this.panel.SignalStable != 0, true);
                yield return new ManagementProperty<int>("Signal Flags", this.panel.SignalFlags, true);
                yield return new ManagementProperty<string>("Contrast", GetStringValue(this.panel.Contrast), true);
                yield return new ManagementProperty<string>("Sharpness", GetStringValue(this.panel.Sharpness), true);
                yield return new ManagementProperty<string>("Color Red", GetStringValue(this.panel.ColorRed), true);
                yield return new ManagementProperty<string>("Color Green", GetStringValue(this.panel.ColorGreen), true);
                yield return new ManagementProperty<string>("Color Blue", GetStringValue(this.panel.ColorBlue), true);
            }

            private static string GetStringValue(int? value)
            {
                return value == null ? "n/a" : value.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
