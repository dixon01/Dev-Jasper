// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShutdownCatcher.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShutdownCatcher type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Enums;

    using Microsoft.Win32;

    /// <summary>
    /// Class that catches the shutdown event from Windows (using a dedicated form).
    /// </summary>
    public partial class ShutdownCatcher
    {
        private readonly ManualResetEvent waitConsumerReady = new ManualResetEvent(false);

        private readonly Dictionary<HotKey, HotKeyUsage> registeredKeys = new Dictionary<HotKey, HotKeyUsage>();

        private CatcherForm catcherForm;

        private bool running;

        private Thread thread;

        private int keyIds;

        /// <summary>
        /// Starts this object.
        /// </summary>
        /// <param name="context">
        /// The application context or null if none is available.
        /// </param>
        public void Start(ApplicationContext context)
        {
            if (this.running)
            {
                return;
            }

            Logger.Debug("Starting");
            this.running = true;

            if (this.thread != null)
            {
                return;
            }

            this.thread = new Thread(this.Run) { IsBackground = true };
            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.Start();

            this.waitConsumerReady.WaitOne();
            Logger.Debug("Started");
        }

        /// <summary>
        /// Stops this object.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            Logger.Debug("Stopping");
            this.running = false;
            this.Close();

            SystemEvents.SessionEnding -= this.SystemEventsOnSessionEnding;
            SystemEvents.SessionEnded -= this.SystemEventsOnSessionEnded;
            SystemEvents.PowerModeChanged -= this.SystemEventsOnPowerModeChanged;
            Logger.Debug("Stopped");
        }

        /// <summary>
        /// Adds a window message filter to the application loop running the shutdown catcher.
        /// </summary>
        /// <param name="messageFilter">
        /// The message filter.
        /// </param>
        public void AddMessageFilter(IMessageFilter messageFilter)
        {
            var f = this.catcherForm;
            if (f == null)
            {
                throw new NotSupportedException("Can't add/remove message filter before calling Start()");
            }

            try
            {
                lock (f.MessageFilters)
                {
                    f.MessageFilters.Add(messageFilter);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Could not add message filters: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Removes a window message filter from the application loop running the shutdown catcher.
        /// </summary>
        /// <param name="messageFilter">
        /// The message filter.
        /// </param>
        public void RemoveMessageFilter(IMessageFilter messageFilter)
        {
            var f = this.catcherForm;
            if (f == null)
            {
                throw new NotSupportedException("Can't add/remove message filter before calling Start()");
            }

            try
            {
                lock (f.MessageFilters)
                {
                    f.MessageFilters.Remove(messageFilter);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Could not add message filters: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Registers a hotkey with a specific modifier and id
        /// </summary>
        /// <param name="modifier">
        /// A modifier is a key used in conjunction with the hotkey like the Windows key etc.
        /// </param>
        /// <param name="key">
        /// The hotkey.
        /// </param>
        public void RegisterHotKey(KeyModifier modifier, char key)
        {
            Logger.Info($"HotKey Registering modifier = {modifier}, key = {key}");
            var hotKey = new HotKey(modifier, key);
            int id;
            lock (this.registeredKeys)
            {
                HotKeyUsage usage;
                if (this.registeredKeys.TryGetValue(hotKey, out usage))
                {
                    usage.UsageCounter++;
                    Logger.Debug("Key {0} has been registered before", hotKey);
                    return;
                }

                id = ++this.keyIds;
                this.registeredKeys.Add(hotKey, new HotKeyUsage(id));
            }

            this.RegisterHotKey(id, hotKey);
        }

        /// <summary>
        /// Deregisters the hotkey based on the unique id previously used to register the hotkey.
        /// </summary>
        /// <param name="modifier">
        /// The modifier.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public void DeregisterHotKey(KeyModifier modifier, char key)
        {
            Logger.Info($"HotKey DeRegistering modifier = {modifier}, key = {key}");
            var hotKey = new HotKey(modifier, key);
            HotKeyUsage usage;
            lock (this.registeredKeys)
            {
                if (!this.registeredKeys.TryGetValue(hotKey, out usage))
                {
                    return;
                }

                usage.UsageCounter--;
                if (usage.UsageCounter > 0)
                {
                    return;
                }

                this.registeredKeys.Remove(hotKey);
            }

            this.DeregisterHotKey(usage.Id);
        }

        private void DeregisterHotKey(int id)
        {
            var f = this.catcherForm;
            if (f == null || f.IsDisposed)
            {
                return;
            }

            if (f.InvokeRequired)
            {
                f.BeginInvoke(new MethodInvoker(() => this.DeregisterHotKey(id)));
            }
            else
            {
                if (User32.UnregisterHotKey(f.Handle, id))
                {
                    Logger.Debug("The hotkey with id {0} has been deregistered", id);
                }
                else
                {
                    Logger.Debug("Could not deregister the hotkey with id: {0}", id);
                }
            }
        }

        private void RegisterHotKey(int keyId, HotKey hotKey)
        {
            var f = this.catcherForm;
            if (f == null)
            {
                return;
            }

            if (f.InvokeRequired)
            {
                f.BeginInvoke(new MethodInvoker(() => this.RegisterHotKey(keyId, hotKey)));
            }
            else
            {
                if (User32.RegisterHotKey(f.Handle, keyId, hotKey.Modifier, hotKey.Key))
                {
                    Logger.Debug("Registered hotkey: {0}", hotKey);
                }
                else
                {
                    Logger.Warn("Could not register the hotkey: {0}", hotKey);
                }
            }
        }

        private void Run()
        {
            using (this.catcherForm = new CatcherForm())
            {
                this.AttachToSystemEvents();
                this.waitConsumerReady.Set();
                Application.Run();
            }
        }

        private void Close()
        {
            if (this.thread == null)
            {
                return;
            }

            this.waitConsumerReady.Reset();

            if (!this.catcherForm.IsDisposed)
                this.catcherForm.Invoke(new ThreadStart(Application.Exit));

            this.catcherForm = null;
            this.thread = null;
        }

        private void AttachToSystemEvents()
        {
            SystemEvents.SessionEnding += this.SystemEventsOnSessionEnding;
            SystemEvents.SessionEnded += this.SystemEventsOnSessionEnded;
            SystemEvents.PowerModeChanged += this.SystemEventsOnPowerModeChanged;
        }

        private void SystemEventsOnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            Logger.Info("Got PowerModeChanged '{0}', raising ShuttingDown event", e.Mode);

            this.RaiseShuttingDown(EventArgs.Empty);
        }

        private void SystemEventsOnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            Logger.Info("Got SessionEnded '{0}', raising ShuttingDown event", e.Reason);

            this.RaiseShuttingDown(EventArgs.Empty);
        }

        private void SystemEventsOnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            Logger.Info("Got SessionEnding '{0}', raising ShuttingDown event", e.Reason);

            this.RaiseShuttingDown(EventArgs.Empty);
        }

        private sealed class CatcherForm : Form
        {
            public CatcherForm()
            {
                this.MessageFilters = new List<IMessageFilter>();
                this.CreateHandle();
            }

            public List<IMessageFilter> MessageFilters { get; private set; }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                lock (this.MessageFilters)
                {
                    foreach (var messageFilter in this.MessageFilters)
                    {
                        messageFilter.PreFilterMessage(ref m);
                    }
                }
            }
        }

        private class HotKey : IEquatable<HotKey>
        {
            public HotKey(KeyModifier modifier, char key)
            {
                this.Modifier = modifier;
                this.Key = key;
            }

            public KeyModifier Modifier { get; private set; }

            public char Key { get; private set; }

            public bool Equals(HotKey other)
            {
                return other != null && this.Modifier == other.Modifier && this.Key == other.Key;
            }

            public override int GetHashCode()
            {
                return this.Modifier.GetHashCode() ^ this.Key.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as HotKey);
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", this.Modifier, this.Key);
            }
        }

        private class HotKeyUsage
        {
            public HotKeyUsage(int id)
            {
                this.Id = id;
                this.UsageCounter = 1;
            }

            public int Id { get; private set; }

            public int UsageCounter { get; set; }
        }
    }
}