// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemperatureObserver.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TemperatureObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// Class that is checking WMI to get and log temperature sensor readings.
    /// </summary>
    public partial class TemperatureObserver : IManageableObject
    {
        private ManagementObjectSearcher searcher;

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            foreach (var obj in this.searcher.Get())
            {
                var info = this.CreateTemperatureInfo(obj);
                yield return new ManagementProperty<double>(info.Name, info.Degrees, true);
            }
        }

        partial void Initialize()
        {
            this.searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
        }

        partial void ReadSensors(ICollection<TemperatureInfo> sensors)
        {
            foreach (var obj in this.searcher.Get())
            {
                sensors.Add(this.CreateTemperatureInfo(obj));
            }
        }

        private TemperatureInfo CreateTemperatureInfo(ManagementBaseObject obj)
        {
            var rawValue = Convert.ToInt32(obj["CurrentTemperature"]);
            var degrees = (rawValue - 2732) / 10.0;

            var name = obj["InstanceName"].ToString();
            var index = name.LastIndexOf('\\');
            return new TemperatureInfo(name.Substring(index + 1), rawValue, degrees);
        }
    }
}
