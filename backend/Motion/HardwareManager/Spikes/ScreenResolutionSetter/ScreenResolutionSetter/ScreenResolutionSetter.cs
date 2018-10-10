// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenResolutionSetter.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenResolutionSetter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ScreenResolutionSetter
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Api.Structs;

    /// <summary>
    /// The screen resolution setter.
    /// </summary>
    public partial class ScreenResolutionSetter : Form
    {
        private const int PrimaryDisplayFlag =
                (int)ChangeDeviceSettings.SetPrimary | (int)ChangeDeviceSettings.UpdateRegistry | (int)ChangeDeviceSettings.Reset;

        private const int SecondaryDisplayFlag = (int)ChangeDeviceSettings.NoReset | (int)ChangeDeviceSettings.UpdateRegistry;

        private const string EwfManagerPath = "ewfmgr.exe";
        private const string CommitArguments = "c: -commit";

        private const UInt32 SDC_TOPOLOGY_INTERNAL = 0x00000001;
        private const UInt32 SDC_TOPOLOGY_CLONE = 0x00000002;
        private const UInt32 SDC_TOPOLOGY_EXTEND = 0x00000004;
        private const UInt32 SDC_TOPOLOGY_EXTERNAL = 0x00000008;
        private const UInt32 SDC_APPLY = 0x00000080;

        private DevMode originalModePrimary;
        private DevMode originalModeSecondary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenResolutionSetter"/> class.
        /// </summary>
        public ScreenResolutionSetter()
        {
            this.InitializeComponent();
        }

        private void CloneDisplays()
        {
            SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, (SDC_APPLY | SDC_TOPOLOGY_CLONE));
        }

        private void ExtendDisplays()
        {
            SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, (SDC_APPLY | SDC_TOPOLOGY_EXTEND));
        }

        private class DisplayConfig
        {
            public int Adapter;

            public int Width;

            public int Height;

            public int Orientation;

            public int PositionX;

            public int PositionY;
        }

        private void Button6Click(object sender, EventArgs e)
        {
            this.originalModePrimary = new DevMode();
            this.originalModeSecondary = new DevMode();
            switch (this.comboBox3.Text)
            {
                case "Clone":
                {
                    //this.GetDisplayInfo();

                    var primaryDisplayAppearance = this.GetPrimaryDisplayAppearance();
                    var success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                    success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                    this.ShowMessage(
                        success
                            ? string.Format("The primary display setting was successful")
                            : string.Format("The primary display setting was not successful"));

                    this.CloneDisplays();
                    success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                    this.CloneDisplays();
                    break;
                }

                case "Extend":
                    {
                        this.ExtendDisplays();
                        var secondaryDisplayAppearance = this.SetSecondaryDisplayAppearanceExtendMode();
                        var success = this.ChangeDisplaySettings(secondaryDisplayAppearance, (int)ChangeDeviceSettings.NoReset | (int)ChangeDeviceSettings.UpdateRegistry);
                        success = this.ChangeDisplaySettings(secondaryDisplayAppearance, (int)ChangeDeviceSettings.Reset | (int)ChangeDeviceSettings.UpdateRegistry);
                        this.ShowMessage(
                            success
                                ? string.Format("The secondary display setting was successful")
                                : string.Format("The secondary display setting was not successful"));
                        var primaryDisplayAppearance = this.GetPrimaryDisplayAppearance();
                        success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                        success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                        this.ShowMessage(
                            success
                                ? string.Format("The primary display setting was successful")
                                : string.Format("The primary display setting was not successful"));
                        this.ExtendDisplays();
                        success = this.ChangeDisplaySettings(secondaryDisplayAppearance, (int)ChangeDeviceSettings.Reset | (int)ChangeDeviceSettings.UpdateRegistry);
                        success = this.ChangeDisplaySettings(primaryDisplayAppearance, PrimaryDisplayFlag);
                        this.ExtendDisplays();

                        break;
                    }
            }
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private DisplayConfig GetPrimaryDisplayAppearance()
        {
            int orientation = -1;
            int positionX = 0;
            int positionY = 0;
            switch (this.comboBox1.Text)
            {
                case "Landscape":
                    positionX = 0;
                    positionY = 0;
                    orientation = 0;
                    break;
                case "Portrait":
                    positionX = 0;
                    positionY = 0;
                    orientation = 1;
                    break;
            }

            var primaryDisplayConfig = new DisplayConfig
            {
                Adapter = 0,
                Width = int.Parse(textBox2.Text),
                Height = int.Parse(textBox3.Text),
                Orientation = orientation,
                PositionX = positionX,
                PositionY = positionY
            };

            return primaryDisplayConfig;
        }

        private DisplayConfig SetSecondaryDisplayAppearanceExtendMode()
        {
            int orientation = -1;
            int positionX = 0;
            int positionY = 0;
            switch (this.comboBox2.Text)
            {
                case "Landscape":
                    positionX = 1920;
                    positionY = 0;
                    orientation = 0;
                    break;
                case "Portrait":
                    positionX = 1081;
                    positionY = 0;
                    orientation = 1;
                    break;
            }

            var secondaryDisplayConfig = new DisplayConfig
            {
                Adapter = int.Parse(this.textBox6.Text),
                Width = int.Parse(textBox5.Text),
                Height = int.Parse(textBox4.Text),
                Orientation = orientation,
                PositionX = positionX,
                PositionY = positionY
            };

            return secondaryDisplayConfig;
        }

        private bool ChangeDisplaySettings(DisplayConfig config, int changeDeviceSettingsFlag)
        {
            var deviceName = this.GetDeviceName(config.Adapter);
            var isSuccessful = false;
            var originalMode = new DevMode();
            originalMode.Size =
                (short)Marshal.SizeOf(originalMode);

            this.ShowMessage(string.Format("The display name for the adapter {0} is: {1}", config.Adapter, deviceName));
            if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref originalMode))
            {
                this.ShowMessage("Could not retrieve the current display settings!");
                return false;
            }

            var newMode = originalMode;

            // Change the settings
            newMode.Position.X = config.PositionX;
            newMode.Position.Y = config.PositionY;
            newMode.PelsWidth = config.Width;
            newMode.PelsHeight = config.Height;
            newMode.DeviceName = deviceName;
            newMode.DisplayOrientation = (int)config.Orientation;

            // Checks to see if the settings can be applied
            var result = User32.ChangeDisplaySettings(ref newMode, (int)ChangeDeviceSettings.Test);

            if (result == (int)DisplaySettingResults.Failed)
            {
                this.ShowMessage("Change of device appearance failed!");
                this.ShowMessage(string.Format("Configured resolution width {0}; height {1} not applicable", config.Width, config.Height));
                return false;
            }

            result = User32.ChangeDisplaySettings(ref newMode, changeDeviceSettingsFlag);

            var displayResult = (DisplaySettingResults)result;
            switch (displayResult)
            {
                case DisplaySettingResults.Successful:
                    this.ShowMessage("The change of display appearance was successful");
                    isSuccessful = true;
                    break;
                case DisplaySettingResults.Restart:
                    this.ShowMessage("The change of display requires a restart");
                    isSuccessful = true;
                    break;
                case DisplaySettingResults.Failed:
                    this.ShowMessage("Change of display appearance failed");
                    break;
                case DisplaySettingResults.NotUpdated:
                    this.ShowMessage("Change of display appearance not updated");
                    break;
                case DisplaySettingResults.BadMode:
                    this.ShowMessage("Change of display appearance not updated due to BadMode");
                    break;
                case DisplaySettingResults.BadDualView:
                    this.ShowMessage("Change of display appearance not updated due to BadDualView");
                    break;
                case DisplaySettingResults.BadFlags:
                    this.ShowMessage("Change of display appearance not updated due to BadFlags");
                    break;
                case DisplaySettingResults.BadParam:
                    this.ShowMessage("Change of display appearance not updated due to BadParam");
                    break;
            }

            if (!isSuccessful)
            {
                this.ShowMessage("Reverting to original settings");
                User32.ChangeDisplaySettings(ref originalMode, 0);
            }
            else
            {
                this.ShowMessage(string.Format("New width {0}; height {1} have been set", config.Width, config.Height));
                var changedMode = new DevMode();
                originalMode.Size =
                    (short)Marshal.SizeOf(changedMode);
                if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref changedMode))
                {
                    this.ShowMessage("Could not retrieve the current display settings!");
                }
                else
                {
                    this.ShowMessage(string.Format("Current setting of system: width:{0}, height:{1}, posX:{2}, posY:{3}, orientation:{4}",
                        changedMode.PelsWidth,
                        changedMode.PelsHeight,
                        changedMode.Position.X,
                        changedMode.Position.Y,
                        changedMode.DisplayOrientation));
                }
            }

            return isSuccessful;
        }

        private string GetDeviceName(int devNum)
        {
            var displayDevice = new DisplayDevice(0);
            var result = User32.EnumDisplayDevices(IntPtr.Zero, devNum, ref displayDevice, 0);
            return result ? displayDevice.DeviceName.Trim() : string.Empty;
        }

        private void Button3Click(object sender, EventArgs e)
        {
            try
            {
                var process = Process.Start(EwfManagerPath, CommitArguments);
                if (process == null)
                {
                    this.ShowMessage(string.Format("Couldn't start {0}", EwfManagerPath));
                    return;
                }

                if (!process.WaitForExit(20 * 1000))
                {
                    this.ShowMessage("{0} didn't exit as expected, restarting anyway");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage(string.Format("Couldn't commit settings changes, restarting anyway", ex));
            }
        }

        private void Button4Click(object sender, EventArgs e)
        {
            this.GetCurrentSettings(0);
        }

        private void GetCurrentSettings(int adapter)
        {
            var deviceName = this.GetDeviceName(adapter);
            var originalMode = new DevMode();
            originalMode.Size = (short)Marshal.SizeOf(originalMode);

            this.ShowMessage(string.Format("The display name for the adapter {0} is: {1}", adapter, deviceName));
            if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref originalMode))
            {
                this.ShowMessage("Could not retrieve the current display settings!");
                this.GetCurrentSettings(0);
            }
            else
            {
                this.ShowMessage(string.Format("Current setting of system: width:{0}, height:{1}, posX:{2}, posY:{3}, orientation:{4}",
                originalMode.PelsWidth,
                originalMode.PelsHeight,
                originalMode.Position.X,
                originalMode.Position.Y,
                originalMode.DisplayOrientation));
            }
        }

        private void Button5Click(object sender, EventArgs e)
        {
            this.GetCurrentSettings(1);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern long SetDisplayConfig(uint numPathArrayElements,
            IntPtr pathArray,
            uint numModeArrayElements,
            IntPtr modeArray,
            uint flags);
    }
}

