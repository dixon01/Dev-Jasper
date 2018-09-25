// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Gioom.Core.Utility;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class responsible for choosing the right <see cref="HardwareManagerSetting"/>
    /// and applying them to the system.
    /// </summary>
    public partial class SettingsHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<SettingsHandler>();

        private readonly IList<HardwareManagerSetting> settings;

        private readonly Dictionary<IOConditionKey, PortListener> conditionListeners =
            new Dictionary<IOConditionKey, PortListener>();

        private readonly List<ISettingsPartHandler> settingParts = new List<ISettingsPartHandler>();

        private bool settingsApplied;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsHandler"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public SettingsHandler(IList<HardwareManagerSetting> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.settings = settings;

            foreach (var setting in settings)
            {
                foreach (var condition in setting.Conditions)
                {
                    var key = new IOConditionKey(condition);
                    if (this.conditionListeners.ContainsKey(key))
                    {
                        continue;
                    }

                    var listener =
                        new PortListener(
                            new MediAddress(
                                condition.Unit ?? MessageDispatcher.Instance.LocalAddress.Unit,
                                condition.Application ?? "*"),
                            condition.Name);
                    listener.ValueChanged += this.ListenerOnValueChanged;
                    this.conditionListeners.Add(key, listener);
                }
            }

            this.Initialize();
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        public void Start()
        {
            lock (this.conditionListeners)
            {
                foreach (var listener in this.conditionListeners.Values)
                {
                    listener.Start(TimeSpan.FromSeconds(2));
                }

                if (this.conditionListeners.Count != 0)
                {
                    return;
                }
            }

            // we get here if didn't have any condition listeners (i.e. no conditions)
            Logger.Info("No conditions found, applying default config");
            this.ApplySettings();
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            this.DisposeListeners();
        }

        partial void Initialize();

        partial void CommitChanges();

        private void DisposeListeners()
        {
            lock (this.conditionListeners)
            {
                foreach (var listener in this.conditionListeners.Values)
                {
                    listener.Dispose();
                }
            }
        }

        private void ApplySettings()
        {
            lock (this)
            {
                if (this.settingsApplied)
                {
                    return;
                }

                this.settingsApplied = true;
            }

            if (this.settings.Count == 0)
            {
                Logger.Info("No settings found");
                ThreadPool.QueueUserWorkItem(s => this.ApplySetting(new HardwareManagerSetting()));
                return;
            }

            foreach (var setting in this.settings)
            {
                if (!this.ConditionsMatch(setting.Conditions))
                {
                    continue;
                }

                var localSetting = setting;
                ThreadPool.QueueUserWorkItem(s => this.ApplySetting(localSetting));
                this.DisposeListeners();
                this.conditionListeners.Clear();
                return;
            }

            Logger.Warn("Couldn't find a valid condition to apply settings");
        }

        private bool ConditionsMatch(ICollection<IOCondition> conditions)
        {
            Logger.Trace("Testing {0} conditions", conditions.Count);
            foreach (var condition in conditions)
            {
                var key = new IOConditionKey(condition);
                var listener = this.conditionListeners[key];
                if (listener.Value == null || listener.Value.Value != condition.Value)
                {
                    Logger.Trace(
                        "Condition doesn't match: <{0},{1},{2}> ('{3}') != '{4}'",
                        condition.Unit,
                        condition.Application,
                        condition.Name,
                        condition.Value,
                        listener.Value);
                    return false;
                }

                Logger.Trace(
                    "Condition matches: <{0},{1},{2}> = '{3}'",
                    condition.Unit,
                    condition.Application,
                    condition.Name,
                    condition.Value);
            }

            Logger.Trace("All conditions matched");
            return true;
        }

        private void ApplySetting(HardwareManagerSetting setting)
        {
            var commit = false;
            foreach (var handler in this.settingParts)
            {
                try
                {
                    if (handler.ApplySetting(setting))
                    {
                        commit = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Couldn't apply setting for " + handler.GetType().Name);
                    return;
                }
            }

            if (!commit)
            {
                Logger.Info("No changes require a commit, we are done");
                return;
            }

            this.CommitChanges();
        }

        private void ListenerOnValueChanged(object sender, EventArgs eventArgs)
        {
            lock (this.conditionListeners)
            {
                foreach (var listener in this.conditionListeners)
                {
                    if (listener.Value.Port == null)
                    {
                        Logger.Trace(
                            "Port <{0};{1};{2}> was not yet found",
                            listener.Key.Unit,
                            listener.Key.Application,
                            listener.Key.Name);
                        return;
                    }
                }
            }

            Logger.Debug("Got all I/O values, applying settings");
            this.ApplySettings();
        }

        private class IOConditionKey
        {
            public IOConditionKey(string unit, string application, string name)
            {
                this.Unit = unit;
                this.Application = application;
                this.Name = name;
            }

            public IOConditionKey(IOCondition condition)
                : this(condition.Unit, condition.Application, condition.Name)
            {
            }

            public string Unit { get; private set; }

            public string Application { get; private set; }

            public string Name { get; private set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                var other = (IOConditionKey)obj;
                return string.Equals(this.Unit, other.Unit)
                       && string.Equals(this.Application, other.Application)
                       && string.Equals(this.Name, other.Name);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = this.Unit != null ? this.Unit.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ (this.Application != null ? this.Application.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (this.Name != null ? this.Name.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}
