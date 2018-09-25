// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisSerialChannel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Protran.Core.Buffers;
    using Gorba.Motion.Protran.Ibis.Parsers;
    using Gorba.Motion.Protran.Ibis.Remote;
    using Gorba.Motion.Protran.IO.Serial;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// This class manages all the stuffs involved
    /// with the communication between Protran and
    /// the IBIS master.
    /// </summary>
    public class IbisSerialChannel : IbisChannel, IManageableObject
    {
        /// <summary>
        /// The object tasked to calculate the time elapsed
        /// from the first telegram's byte received to the
        /// last byte of a sent answer.
        /// </summary>
        protected readonly Stopwatch StopWatch;

        /// <summary>
        /// Flag that tells if the stopWatch has to be used or not.
        /// </summary>
        protected readonly bool MonitorTime;

        // protected bool RunReader;

        /// <summary>
        /// The amount of milliseconds to wait between an attempt to open the serial port
        /// and the next one.
        /// </summary>
        private const int AttemptInterval = 1000;

        /// <summary>
        /// Timer tasked to try to open the serial port, in case of errors.
        /// </summary>
        private readonly ITimer restartTimer;

        /// <summary>
        /// Container of all the information given by the user
        /// to the IBIS serial port in the config file.
        /// </summary>
        private readonly SerialPortConfig serialPortConfig;

        private readonly SerialPortIOHandler portIoHandler;

        private ILoopObserver loopObserver;

        // protected SerialPort SerialPort;

        // protected bool IsFirst = true;
        private ulong accumulator;
        private ulong counts;
        private Thread readThread;

        private bool isRestarting;

        private string answer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisSerialChannel"/> class.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public IbisSerialChannel(IIbisConfigContext configContext)
            : base(configContext)
        {
            this.MonitorTime = false;
            this.serialPortConfig = configContext.Config.Sources.SerialPort;
            this.StopWatch = new Stopwatch();
            this.IsFirst = true;
            this.RemoteComputer = new IbisMaster(this.Config.Behaviour.ConnectionTimeOut);

            this.Parser.TelegramDataReceived += this.ParserOnTelegramDataReceived;

            this.portIoHandler = new SerialPortIOHandler();
            var container = ServiceLocator.Current.GetInstance<IServiceContainer>();
            container.RegisterInstance<ISerialPortIOs>(this.serialPortConfig.ComPort.ToUpper(), this.portIoHandler);

            // Attention:
            // the "restartTimer" interval has to be at least (AttemptInterval * this.serialPortConfig.RetryCount) ms
            // otherwise during the progress of one its invocation of "ReactivateSerialPort"
            // arrives also a second one, a third one and so on, if we lose the serial port.
            this.restartTimer = TimerFactory.Current.CreateTimer("IbisSerialChannelRestart");
            this.restartTimer.AutoReset = true;
            this.restartTimer.Interval = TimeSpan.FromMilliseconds(AttemptInterval * this.serialPortConfig.RetryCount);
            this.restartTimer.Elapsed += (s, e) => this.ReactivateSerialPort();
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsFirst.
        /// </summary>
        protected bool IsFirst { get; set; }

        /// <summary>
        /// Gets or sets SerialPort.
        /// </summary>
        protected SerialPort SerialPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RunReader.
        /// </summary>
        protected bool RunReader { get; set; }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Serial Port Open", this.SerialPort.IsOpen, true);
            yield return new ManagementProperty<bool>("Ibis read loop running", this.RunReader, true);
            yield return new ManagementProperty<string>("Sent answer", this.answer, true);
        }

        /// <summary>
        /// Opens the IBIS channel.
        /// <exception cref="IOException">If the serial port doesn't exist.</exception>
        /// <exception cref="InvalidOperationException">Internal error with the serial port.</exception>
        /// <exception cref="ArgumentException">If the parameters for the serial port are invalid.</exception>
        /// <exception cref="UnauthorizedAccessException">If the serial port is already owned
        /// by another application.</exception>
        /// </summary>
        protected override void DoOpen()
        {
            var registration = ServiceLocator.Current.GetInstance<IApplicationRegistration>("Protran");
            this.loopObserver = registration.CreateLoopObserver(this.GetType().Name, TimeSpan.FromSeconds(60));

            try
            {
                // the default encoding for the
                // serial port is the ASCII encoding.
                // see:
                // http://msdn.microsoft.com/en-us/library/system.io.ports.serialport.encoding%28v=VS.80%29.aspx
                this.SerialPort = new SerialPort(
                    this.serialPortConfig.ComPort,
                    this.serialPortConfig.BaudRate,
                    this.serialPortConfig.Parity,
                    this.serialPortConfig.DataBits,
                    this.serialPortConfig.StopBits);
                this.SerialPort.ErrorReceived += this.SerialPortErrorReceived;
            }
            catch (Exception ex)
            {
                // it was impossible to get the IBIS serial port
                // basing on the configured parameters.
                this.Logger.Error(ex,"IBIS Ch. serial port error");
                throw;
            }

            // now I'll try to open the serial port
            this.OpenSerialPort();

            if (!this.SerialPort.IsOpen)
            {
                throw new IOException("Could not open serial port.");
            }

            base.DoOpen();
            this.Logger.Info("IBIS channel opened and configured with success");
            this.Logger.Info(this.serialPortConfig.ToString());
            this.StartRead();
        }

        /// <summary>
        /// Closes the IBIS channel.
        /// </summary>
        protected override void DoClose()
        {
            this.StopRead();

            base.DoClose();

            if (this.loopObserver != null)
            {
                this.loopObserver.Dispose();
                this.loopObserver = null;
            }

            this.portIoHandler.SerialPort = null;

            if (this.SerialPort != null && this.SerialPort.IsOpen)
            {
                // I close the RS232 driver.
                this.SerialPort.Close();
                this.SerialPort.Dispose();
            }

            this.Logger.Info("IBIS channel closed.");
        }

        /// <summary>
        /// Writes the given bytes to the serial port.
        /// </summary>
        /// <param name="bytes">
        /// The buffer to send.
        /// </param>
        /// <param name="offset">
        /// The offset inside the buffer.
        /// </param>
        /// <param name="length">
        /// The number of bytes to send starting from <see cref="offset"/>.
        /// </param>
        protected override void SendAnswer(byte[] bytes, int offset, int length)
        {
            this.answer = Encoding.UTF8.GetString(bytes, offset, length);
            this.SerialPort.Write(bytes, offset, length);

            if (this.MonitorTime)
            {
                this.StopWatch.Stop();
                this.IsFirst = true;
                this.accumulator += (ulong)this.StopWatch.ElapsedMilliseconds;
                this.counts++;
                Console.WriteLine(
                    "\nTime to answer (ms): {0}  average (ms): {1}",
                    this.StopWatch.ElapsedMilliseconds,
                    this.accumulator / this.counts);
                this.StopWatch.Reset();
            }
        }

        /// <summary>
        /// On Telegram Data Received
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void ParserOnTelegramDataReceived(object sender, TelegramDataEventArgs e)
        {
            this.Logger.Trace(() => "Received telegram: " + BufferUtils.FromByteArrayToHexString(e.Data));
            this.ManageTelegram(e.Data);
        }

        /// <summary>
        /// Read thread for serial port
        /// </summary>
        protected virtual void ReadThread()
        {
            // kick off JIT, so we are faster when we actually get some data
            this.PreJitMethods();
            while (this.RunReader)
            {
                try
                {
                    bool read = this.Parser.ReadFrom(this.SerialPort.BaseStream);
                    if (this.MonitorTime && this.IsFirst)
                    {
                        this.StopWatch.Start();
                        this.IsFirst = false;
                    }

                    if (read)
                    {
                        var observer = this.loopObserver;
                        if (observer != null)
                        {
                            observer.Trigger();
                        }

                        this.RemoteComputer.HasSentData = true;
                    }
                }
                catch (Exception ex)
                {
                    // an exception is occurred reading bytes from the serial port.
                    // I've to stop everything and then restart.
                    Logger.Info(ex, "Error on reading from the serial port.");
                    this.RestartSerialPort();
                }
            }
        }

        /// <summary>
        /// kick off JIT, so we are faster when we actually get some data
        /// </summary>
        protected void PreJitMethods()
        {
            foreach (var t in new[] { this.GetType(), this.Parser.GetType(), typeof(CircularBuffer) })
            {
                var type = t;
                while (type != null && type.BaseType != null)
                {
                    foreach (
                        var method in
                            type.GetMethods(
                                BindingFlags.Static | BindingFlags.Instance
                                | BindingFlags.NonPublic | BindingFlags.Public))
                    {
                        if (method.IsGenericMethod || method.IsGenericMethodDefinition || method.IsAbstract)
                        {
                            continue;
                        }

                        RuntimeHelpers.PrepareMethod(method.MethodHandle);
                    }

                    type = type.BaseType;
                }
            }
        }

        private void OpenSerialPort()
        {
            // here below I'll try to open the serial for the number of attempts: "this.configContainer.RetryCount".
            for (int attempts = 0; attempts <= this.serialPortConfig.RetryCount; attempts++)
            {
                try
                {
                    if (!this.SerialPort.IsOpen)
                    {
                        this.SerialPort.Open();
                        this.portIoHandler.SerialPort = this.SerialPort;

                        // ok, serial port opened
                        this.Logger.Info("IBIS channel opened with success.");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // I save the exception for later.
                    this.Logger.Debug(ex, "Could not open serial port");
                }

                this.Logger.Error("Ibis Ch. opening attempt {0} failed", attempts);
                Thread.Sleep(AttemptInterval);
            }

            if (this.SerialPort.IsOpen && this.restartTimer.Enabled)
            {
                this.restartTimer.Enabled = false;
                this.isRestarting = false;
            }
        }

        private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            this.Logger.Debug("Serial Port Error Detected due to: {0}", e.EventType);

            switch (this.serialPortConfig.SerialPortReopen)
            {
                case SerialPortReopen.FrameOnly:
                    {
                        if (e.EventType == SerialError.Frame)
                        {
                            this.RestartSerialPort();
                        }
                    }

                    break;
                case SerialPortReopen.All:
                    {
                        this.RestartSerialPort();
                    }

                    break;
                case SerialPortReopen.None:
                    break;
            }
        }

        private void RestartSerialPort()
        {
            if (this.isRestarting)
            {
                // the restarting process is already started.
                return;
            }

            this.isRestarting = true;

            // now it's the time to restart the serial port activities.
            this.StopRead();
            try
            {
                // the serial port is open and we have detected an error. First of all I close it.
                this.SerialPort.Close();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                // serial port lost.
                this.Logger.Info(ex, "Exception on closing the serial port.");
            }

            this.Logger.Debug("Reopening serial port after error detection.");
            this.OpenSerialPort();

            // if the "OpenSerialPort" has succeded, we can reset the "isRestarting" flag
            if (this.SerialPort.IsOpen)
            {
                // serial port opened. it's the time to restart reading from it.
                this.isRestarting = false;
                this.StartRead();
            }
            else
            {
                // the "OpenSerialPort" has failed. it's the time to start the timer.
                if (!this.restartTimer.Enabled)
                {
                    this.restartTimer.Enabled = true;
                }
            }
        }

        private void ReactivateSerialPort()
        {
            this.OpenSerialPort();
            if (this.SerialPort.IsOpen)
            {
                this.StartRead();
            }
        }

        private void StartRead()
        {
            if (this.readThread != null)
            {
                return;
            }

            this.readThread = new Thread(this.ReadThread)
            {
                Name = "IBIS_Reader",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            this.RunReader = true;
            this.readThread.Start();
        }

        private void StopRead()
        {
            this.RunReader = false;
            this.readThread = null;
        }
    }
}
