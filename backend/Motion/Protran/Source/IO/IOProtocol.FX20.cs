// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOProtocol.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOProtocol type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Protran.IO.Serial;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The I/O protocol which can handle serial ports and use I/O's for Ximple messages.
    /// </summary>
    public partial class IOProtocol
    {
        private readonly List<SerialPortController> serialPortControllers = new List<SerialPortController>();

        partial void ConfigureSerialPorts()
        {
            var serialPortFactory = ServiceLocator.Current.GetInstance<SerialPortFactory>();
            foreach (var port in this.config.SerialPorts)
            {
                var serialPort = serialPortFactory.CreateSerialPortController(port);
                if (serialPort != null)
                {
                    this.serialPortControllers.Add(serialPort);
                }
            }
        }

        partial void StartSerialPorts()
        {
            foreach (var serialPortController in this.serialPortControllers)
            {
                try
                {
                    serialPortController.Start();
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't open port {0}", serialPortController.Name);
                }
            }
        }

        partial void StopSerialPorts()
        {
            foreach (var serialPortController in this.serialPortControllers)
            {
                serialPortController.Stop();
            }

            this.serialPortControllers.Clear();
        }
    }
}
