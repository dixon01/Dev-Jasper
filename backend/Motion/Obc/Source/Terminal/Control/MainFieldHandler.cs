// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainFieldHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Manages all MainFields.
//   Also handles all Screens.MFKey Enums
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Obc.Terminal.Control.Announcement;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.Data;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Control.Screens;
    using Gorba.Motion.Obc.Terminal.Core;

    using NLog;

    /// <summary>
    ///   Manages all MainFields.
    /// </summary>
    internal class MainFieldHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MainFieldHandler>();

        private readonly IUiRoot uiRoot;

        /// <summary>
        ///   Dictionary with all screen (main fields) references
        /// </summary>
        private readonly Dictionary<MainFieldKey, IScreen> mainFieldDic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFieldHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        internal MainFieldHandler(IContext context)
        {
            this.uiRoot = context.UiRoot;
            var config = context.ConfigHandler.GetConfig();

            this.mainFieldDic = new Dictionary<MainFieldKey, IScreen>();
            this.mainFieldDic.Add(MainFieldKey.Status, new StatusInfoScreen(this.uiRoot.MainStatus, context));
            this.mainFieldDic.Add(MainFieldKey.InMessages, new MessageListScreen(this.uiRoot.IconList, context));
            this.mainFieldDic.Add(MainFieldKey.Brightness, new BrightnessScreen(this.uiRoot.List, context));
            this.mainFieldDic.Add(
                MainFieldKey.PassengerCount, new PassengerCountScreen(this.uiRoot.NumberInput, context));
            this.mainFieldDic.Add(MainFieldKey.SystemCode, new SystemCodeScreen(this.uiRoot.NumberInput, context));
            this.mainFieldDic.Add(
                MainFieldKey.TripNumber, new PassengerCountTripNumberScreen(this.uiRoot.NumberInput, context));
            this.mainFieldDic.Add(
                MainFieldKey.SpecialDestinationDrive,
                new SpecialDestinationDriveScreen(this.uiRoot.SpecialDestinationDrive, context));
            this.mainFieldDic.Add(MainFieldKey.BlockDriving, new BlockDrivingScreen(this.uiRoot.BlockDrive, context));
            this.mainFieldDic.Add(
                MainFieldKey.BlockDriveWait, new BlockDriveWaitScreen(this.uiRoot.BlockDriveWait, context));
            this.mainFieldDic.Add(
                MainFieldKey.SpecialDestinationSelect, this.CreateSpecialDestinationSelectionScreen(context));
            this.mainFieldDic.Add(MainFieldKey.Menu, new MenuScreen(this.uiRoot.List, context));
            this.mainFieldDic.Add(
                MainFieldKey.BlockAutoCompletion, new BlockAutoCompletionScreen(this.uiRoot.List, context));
            this.mainFieldDic.Add(
                MainFieldKey.DriverLogin, new EnterLoginNumberScreen(this.uiRoot.LoginNumberInput, context));
            this.mainFieldDic.Add(MainFieldKey.DriveSelect, new DriveSelectionScreen(this.uiRoot.DriveSelect, context));

            if (config.DriverBlock)
            {
                this.mainFieldDic.Add(
                    MainFieldKey.BlockNumberInput,
                    new EnterDriverBlockNumberScreen(this.uiRoot.DriverBlockInput, context));
            }
            else
            {
                this.mainFieldDic.Add(
                    MainFieldKey.BlockNumberInput,
                    new EnterBlockNumberScreen(this.uiRoot.NumberInput, context));
            }

            if (context.WanManager != null && config.SpeechType.Value == SpeechType.Gsm)
            {
                this.mainFieldDic.Add(
                    MainFieldKey.SpeechGsm,
                    new PhoneCallScreen(this.uiRoot.List, context));
            }

            if (config.SpeechType.Value == SpeechType.Radio && context.IraHandler != null)
            {
                this.mainFieldDic.Add(
                    MainFieldKey.IqubeRadio,
                    new IqubeRadioScreen(this.uiRoot.IqubeRadio, context));
            }

            if (!string.IsNullOrEmpty(config.AlarmInput))
            {
                this.mainFieldDic.Add(MainFieldKey.Alarm, new AlarmScreen(this.uiRoot.List, context));
            }

            if (config.Announcement.Value != AnnouncementType.None)
            {
                this.mainFieldDic.Add(
                    MainFieldKey.Announcement,
                    new AnnouncementScreen(
                        this.uiRoot.List,
                        context,
                        new AnnouncementHandler(config.Announcement.Value)));
            }

            this.mainFieldDic.Add(MainFieldKey.Language, new LanguageScreen(this.uiRoot.List, context));
            this.mainFieldDic.Add(MainFieldKey.TtsVolume, new TtsVolumeScreen(this.uiRoot.List, context));
        }

        /// <summary>
        /// Gets the currently displayed screen.
        /// </summary>
        public IScreen CurrentScreen { get; private set; }

        /// <summary>
        /// Sets the currently shown main field.
        /// </summary>
        /// <param name="key">
        /// The main field key.
        /// </param>
        /// <returns>
        /// True if the main field was set successfully.
        /// </returns>
        public bool SetMainField(MainFieldKey key)
        {
            IScreen screen;

            if (!this.mainFieldDic.TryGetValue(key, out screen))
            {
                Logger.Warn("Key not available on mainFieldDic: {0}", key);
                return false;
            }

            return this.SetMainField(screen);
        }

        /// <summary>
        /// Gets the screen for the given key.
        /// TODO: this method should be removed since it is pretty dirty to access other screens like that.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <typeparam name="TScreen">
        /// The type of screen
        /// </typeparam>
        /// <returns>
        /// The <see cref="TScreen"/>, never null.
        /// </returns>
        public TScreen GetScreen<TScreen>(MainFieldKey key)
            where TScreen : class, IScreen
        {
            IScreen screen;
            this.mainFieldDic.TryGetValue(key, out screen);
            if (screen == null)
            {
                throw new KeyNotFoundException("Couldn't find " + key);
            }

            return screen as TScreen;
        }

        private bool SetMainField(IScreen screen)
        {
            if (screen == null)
            {
                return false;
            }

            try
            {
                if (this.CurrentScreen != null)
                {
                    if (this.CurrentScreen.MainField.IsLocked())
                    {
                        return false;
                    }

                    this.CurrentScreen.Hide();
                }

                this.CurrentScreen = screen;
                this.uiRoot.SetMainField(this.CurrentScreen.MainField);
                this.CurrentScreen.Show();
                return true;
            }
            catch (Exception ex)
            {
                Logger.ErrorException("Couldn't change main field to " + screen.MainField, ex);
                return false;
            }
        }

        private IScreen CreateSpecialDestinationSelectionScreen(IContext context)
        {
            var specDestList = new SpecialDestinationList(ConfigPaths.ExtraServiceCsv);
            if (specDestList.Count <= 12)
            {
                return new SpecialDestinationSelectionListScreen(specDestList, this.uiRoot.List, context);
            }

            return new SpecialDestinationSelectionInputScreen(specDestList, this.uiRoot.NumberInput, context);
        }
    }
}