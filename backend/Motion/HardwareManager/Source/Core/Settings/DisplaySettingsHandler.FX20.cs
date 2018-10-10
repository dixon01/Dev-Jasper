// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplaySettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DisplaySettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;
    using Gorba.Common.Utility.Win32.Api.Structs;

    using NLog;

    /// <summary>
    /// The display settings handler.
    /// </summary>
    public partial class DisplaySettingsHandler : ISettingsPartHandler
    {
        private const int PrimaryDisplayFlag =
            (int)ChangeDeviceSettings.SetPrimary | (int)ChangeDeviceSettings.UpdateRegistry |
            (int)ChangeDeviceSettings.Reset;

        private const int SecondaryDisplayFlagNoReset =
            (int)ChangeDeviceSettings.NoReset | (int)ChangeDeviceSettings.UpdateRegistry;

        private const int SecondaryDisplayFlagReset =
            (int)ChangeDeviceSettings.Reset | (int)ChangeDeviceSettings.UpdateRegistry;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Apply the given settings.
        /// </summary>
        /// <param name="setting">
        /// The setting object.
        /// </param>
        /// <returns>
        /// True if the system drive should be committed and the system rebooted.
        /// </returns>
        public bool ApplySetting(HardwareManagerSetting setting)
        {
            if (setting.Display == null)
            {
                Logger.Warn("Display appearance not configured, not changing anything");
                return false;
            }

            if (setting.Display.Screens.Count > 0)
            {
                Logger.Debug("Found display appearance setting.");
                return this.SetDisplayAppearance(setting);
            }

            Logger.Info("Setting of the display appearance is disabled");
            return false;
        }

        private bool SetDisplayAppearance(HardwareManagerSetting setting)
        {
            this.CheckAndSetAdapterValues(setting);
            ScreenConfig primaryDisplayConfig = null;
            ScreenConfig secondaryDisplayConfig = null;
            foreach (var screenConfig in setting.Display.Screens)
            {
                if (screenConfig.IsMainDisplay && primaryDisplayConfig == null)
                {
                    primaryDisplayConfig = screenConfig;
                }
                else
                {
                    secondaryDisplayConfig = screenConfig;
                }
            }

            switch (setting.Display.Mode)
            {
                case DisplayMode.Clone:
                    {
                        if (primaryDisplayConfig == null)
                        {
                            return false;
                        }

                        DevMode originalModePrimary;
                        if (!this.FoundDeviceSettings(primaryDisplayConfig, out originalModePrimary))
                        {
                            Logger.Debug(
                                "Could not get the current settings of the adapter: {0}",
                                primaryDisplayConfig.Adapter);
                            return false;
                        }

                        bool isSecondDisplayAvailable;
                        var originalModeSecondary = this.GetSecondDisplaySettings(
                            secondaryDisplayConfig, out isSecondDisplayAvailable);
                        if (isSecondDisplayAvailable)
                        {
                            if (this.IsCurrentSettingsEqualToNewSettings(
                                primaryDisplayConfig, 0, 0, originalModePrimary) &&
                                this.IsCurrentSettingsEqualToNewSettings(
                                secondaryDisplayConfig, 0, 0, originalModeSecondary))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (this.IsCurrentSettingsEqualToNewSettings(
                                primaryDisplayConfig, 0, 0, originalModePrimary))
                            {
                                return false;
                            }
                        }

                        if (Environment.OSVersion.Version.Major < 6)
                        {
                            return this.SetDisplayInCloneModeXp(
                                isSecondDisplayAvailable,
                                primaryDisplayConfig,
                                secondaryDisplayConfig,
                                originalModeSecondary);
                        }

                        return this.SetDisplayInCloneMode(primaryDisplayConfig);
                    }

                case DisplayMode.Extend:
                    {
                        return primaryDisplayConfig != null && secondaryDisplayConfig != null
                               && this.SetDisplaysInExtendMode(primaryDisplayConfig, secondaryDisplayConfig);
                    }
            }

            return false;
        }

        private void CheckAndSetAdapterValues(HardwareManagerSetting setting)
        {
            var screenConfigs = setting.Display.Screens;
            for (int i = 0; i < screenConfigs.Count; i++)
            {
                var screen = screenConfigs[i];
                var adapterOrdinal = screen.Adapter >= 0 ? screen.Adapter : i;
                screen.Adapter = adapterOrdinal;
            }
        }

        private bool SetDisplayInCloneModeXp(
            bool isSecondDisplayAvailable,
            ScreenConfig primaryDisplayConfig,
            ScreenConfig secondaryDisplayConfig,
            DevMode originalModeSecondary)
        {
            if (isSecondDisplayAvailable)
            {
                var deviceName = this.GetDeviceName(secondaryDisplayConfig.Adapter);
                originalModeSecondary.Position.X = 0;
                originalModeSecondary.Position.Y = 0;
                originalModeSecondary.PelsHeight = 0;
                originalModeSecondary.PelsWidth = 0;
                originalModeSecondary.Fields = (int)(DevModeFieldsFlags.PelsHeight | DevModeFieldsFlags.PelsWidth
                                                     | DevModeFieldsFlags.Position);
                originalModeSecondary.DeviceName = deviceName;
                var result = User32.ChangeDisplaySettingsEx(
                    deviceName,
                    ref originalModeSecondary,
                    IntPtr.Zero,
                    SecondaryDisplayFlagReset,
                    IntPtr.Zero);
                Logger.Debug("The result of deactivation of second display is: {0}", result);
                User32.ChangeDisplaySettings(IntPtr.Zero, 0);
            }

            const int PositionX = 0;
            const int PositionY = 0;
            return this.ChangeDisplaySettings(primaryDisplayConfig, PositionX, PositionY, PrimaryDisplayFlag);
        }

        private DevMode GetSecondDisplaySettings(ScreenConfig secondaryDisplayConfig, out bool isSecondDisplayAvailable)
        {
            var originalModeSecondary = new DevMode();
            isSecondDisplayAvailable = false;
            if (secondaryDisplayConfig != null)
            {
                if (!this.FoundDeviceSettings(secondaryDisplayConfig, out originalModeSecondary))
                {
                    Logger.Debug(
                        "Could not get the current settings of the adapter: {0}",
                        secondaryDisplayConfig.Adapter);
                }
                else
                {
                    isSecondDisplayAvailable = true;
                }
            }

            return originalModeSecondary;
        }

        private bool SetDisplayInCloneMode(ScreenConfig primaryDisplayConfig)
        {
            const int PositionX = 0;
            const int PositionY = 0;
            this.ChangeDisplaySettings(primaryDisplayConfig, PositionX, PositionY, PrimaryDisplayFlag);
            this.ChangeDisplaySettings(primaryDisplayConfig, PositionX, PositionY, PrimaryDisplayFlag);
            this.CloneDisplays();
            bool isPrimaryDisplaySet = this.ChangeDisplaySettings(
                primaryDisplayConfig, PositionX, PositionY, PrimaryDisplayFlag);
            this.CloneDisplays();
            return isPrimaryDisplaySet;
        }

        private bool SetDisplaysInExtendMode(ScreenConfig primaryDisplayConfig, ScreenConfig secondaryDisplayConfig)
        {
            var positionX = primaryDisplayConfig.Width + 1;
            const int PositionY = 0;

            DevMode originalModePrimary;
            if (!this.FoundDeviceSettings(primaryDisplayConfig, out originalModePrimary))
            {
                Logger.Debug(
                    "Could not get the current settings of the adapter: {0}",
                    primaryDisplayConfig.Adapter);
                return false;
            }

            var isSecondDisplayNotEqual = false;
            if (secondaryDisplayConfig != null)
            {
                DevMode originalModeSecondary;
                if (!this.FoundDeviceSettings(secondaryDisplayConfig, out originalModeSecondary))
                {
                    Logger.Debug(
                        "Could not get the current settings of the adapter: {0}",
                        secondaryDisplayConfig.Adapter);
                }
                else
                {
                    isSecondDisplayNotEqual = this.IsCurrentSettingsEqualToNewSettings(
                        secondaryDisplayConfig,
                        primaryDisplayConfig.Width,
                        0,
                        originalModeSecondary);
                }
            }

            if (this.IsCurrentSettingsEqualToNewSettings(primaryDisplayConfig, 0, 0, originalModePrimary)
                && isSecondDisplayNotEqual)
            {
                return false;
            }

            if (Environment.OSVersion.Version.Major < 6)
            {
                return this.SetDisplaysInExtendModeXp(
                    primaryDisplayConfig, secondaryDisplayConfig, originalModePrimary);
            }

            this.ExtendDisplays();
            this.ChangeDisplaySettings(secondaryDisplayConfig, positionX, PositionY, SecondaryDisplayFlagNoReset);
            this.ChangeDisplaySettings(secondaryDisplayConfig, positionX, PositionY, SecondaryDisplayFlagReset);
            this.ChangeDisplaySettings(primaryDisplayConfig, 0, 0, PrimaryDisplayFlag);
            this.ChangeDisplaySettings(primaryDisplayConfig, 0, 0, PrimaryDisplayFlag);
            this.ExtendDisplays();
            var isSecondaryDisplaySet = this.ChangeDisplaySettings(
                secondaryDisplayConfig,
                positionX,
                PositionY,
                SecondaryDisplayFlagReset);
            var isPrimaryDisplaySet = this.ChangeDisplaySettings(primaryDisplayConfig, 0, 0, PrimaryDisplayFlag);
            this.ExtendDisplays();

            return isPrimaryDisplaySet && isSecondaryDisplaySet;
        }

        private bool SetDisplaysInExtendModeXp(
            ScreenConfig primaryDisplayConfig,
            ScreenConfig secondaryDisplayConfig,
            DevMode originalModePrimary)
        {
            var positionX = primaryDisplayConfig.Width + 1;
            const int PositionY = 0;

            DevMode originalModeSecondary;
            if (!this.FoundDeviceSettings(secondaryDisplayConfig, out originalModeSecondary))
            {
                Logger.Debug(
                    "Could not get the current settings of the adapter: {0}",
                    secondaryDisplayConfig.Adapter);
                var deviceName = this.GetDeviceName(secondaryDisplayConfig.Adapter);
                originalModePrimary.Position.X = 1;
                originalModePrimary.Position.Y = 1;
                originalModePrimary.Fields = (int)DevModeFieldsFlags.Position;
                originalModePrimary.DeviceName = deviceName;
                var result = User32.ChangeDisplaySettingsEx(
                    deviceName,
                    ref originalModePrimary,
                    IntPtr.Zero,
                    SecondaryDisplayFlagReset,
                    IntPtr.Zero);
                Logger.Debug("The result of activation of second display is: {0}", result);
            }

            DevMode newModeSecondary;
            if (this.FoundDeviceSettings(secondaryDisplayConfig, out newModeSecondary))
            {
                var isSecondaryDisplaySet = this.ChangeDisplaySettings(
                    secondaryDisplayConfig, positionX, PositionY, SecondaryDisplayFlagReset);
                var isPrimaryDisplaySet = this.ChangeDisplaySettings(
                    primaryDisplayConfig, 0, 0, PrimaryDisplayFlag);

                return isPrimaryDisplaySet && isSecondaryDisplaySet;
            }

            Logger.Debug("Could not activate second display. Not set in extend mode");
            return false;
        }

        private bool IsCurrentSettingsEqualToNewSettings(
            ScreenConfig config,
            int positionX,
            int positionY,
            DevMode originalMode)
        {
            Logger.Debug(
                "Current setting of system: width = {0}, height = {1}, posX = {2}, posY = {3} orientation = {4}",
                originalMode.PelsWidth,
                originalMode.PelsHeight,
                originalMode.Position.X,
                originalMode.Position.Y,
                originalMode.DisplayOrientation);
            if (config.Width != originalMode.PelsWidth || config.Height != originalMode.PelsHeight
                || positionX != originalMode.Position.X || positionY != originalMode.Position.Y
                || (int)config.Orientation != originalMode.DisplayOrientation)
            {
                Logger.Info("Expected display appearance is different from current system settings.");
                return false;
            }

            Logger.Info("Expected display appearance matched current system settings.");
            return true;
        }

        private bool FoundDeviceSettings(ScreenConfig config, out DevMode originalMode)
        {
            var deviceName = this.GetDeviceName(config.Adapter);
            originalMode = new DevMode();
            originalMode.Size = (short)Marshal.SizeOf(originalMode);

            if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref originalMode))
            {
                return false;
            }

            return true;
        }

        private void CloneDisplays()
        {
            User32.SetDisplayConfig(
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                (uint)SetDisplayConfigFlags.SdcApply | (uint)SetDisplayConfigFlags.SdcTopologyClone);
        }

        private void ExtendDisplays()
        {
            User32.SetDisplayConfig(
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                (uint)SetDisplayConfigFlags.SdcApply | (uint)SetDisplayConfigFlags.SdcTopologyExtend);
        }

        private bool ChangeDisplaySettings(
            ScreenConfig config,
            int positionX,
            int positionY,
            int changeDeviceSettingsFlag)
        {
            var deviceName = this.GetDeviceName(config.Adapter);
            var originalMode = new DevMode();
            originalMode.Size = (short)Marshal.SizeOf(originalMode);

            if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref originalMode))
            {
                Logger.Info(
                    "Could not retrieve the current display settings for adapter: {0}, device name: {1}!",
                    config.Adapter,
                    deviceName);
                return false;
            }

            var newMode = originalMode;

            // Change the settings
            newMode.Position.X = positionX;
            newMode.Position.Y = positionY;
            newMode.PelsWidth = config.Width;
            newMode.PelsHeight = config.Height;
            newMode.DeviceName = deviceName;
            newMode.DisplayOrientation = (int)config.Orientation;
            if (Environment.OSVersion.Version.Major < 6)
            {
                newMode.Fields = (int)(
                    DevModeFieldsFlags.Position | DevModeFieldsFlags.PelsHeight | DevModeFieldsFlags.PelsWidth);
            }

            // Checks to see if the settings can be applied
            if (User32.ChangeDisplaySettings(ref newMode, (int)ChangeDeviceSettings.Test) ==
                (int)DisplaySettingResults.Failed)
            {
                Logger.Debug("Configured resolution width {0}; height {1} not applicable", config.Width, config.Height);
                return false;
            }

            var isSuccessful = false;
            var displayResult =
                (DisplaySettingResults)User32.ChangeDisplaySettings(ref newMode, changeDeviceSettingsFlag);
            Logger.Debug("Result of change of display appearance is: {0}", displayResult);
            switch (displayResult)
            {
                case DisplaySettingResults.Successful:
                case DisplaySettingResults.Restart:
                    {
                        isSuccessful = true;
                        this.LogChangedDisplaySettings(deviceName);
                    }

                    break;
                case DisplaySettingResults.Failed:
                case DisplaySettingResults.NotUpdated:
                case DisplaySettingResults.BadMode:
                case DisplaySettingResults.BadDualView:
                case DisplaySettingResults.BadFlags:
                case DisplaySettingResults.BadParam:
                    {
                        Logger.Debug("Reverting to original settings");
                        User32.ChangeDisplaySettings(ref originalMode, 0);
                    }

                    break;
            }

            return isSuccessful;
        }

        private void LogChangedDisplaySettings(string deviceName)
        {
            var changedMode = new DevMode();
            changedMode.Size = (short)Marshal.SizeOf(changedMode);
            if (0 == User32.EnumDisplaySettings(deviceName, (int)DevModeSettings.EnumCurrentSettings, ref changedMode))
            {
                Logger.Info("Could not retrieve the current display settings!");
            }
            else
            {
                Logger.Debug(
                    "Changed display setting for device:{0} = Width:{1}, Height:{2}, X:{3}, Y:{4}, Orientation:{5}",
                    deviceName,
                    changedMode.PelsWidth,
                    changedMode.PelsHeight,
                    changedMode.Position.X,
                    changedMode.Position.Y,
                    changedMode.DisplayOrientation);
            }
        }

        private string GetDeviceName(int devNum)
        {
            var displayDevice = new DisplayDevice(0);
            var result = User32.EnumDisplayDevices(IntPtr.Zero, devNum, ref displayDevice, 0);
            return result ? displayDevice.DeviceName.Trim() : string.Empty;
        }
    }
}