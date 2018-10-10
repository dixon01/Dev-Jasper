// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.Core.Data;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using NLog;

    /// <summary>
    /// Central class that manages the whole presentation in Infomedia.
    /// </summary>
    public class PresentationManager : IDisposable, IManageable
    {
        private static readonly Logger Logger = LogHelper.GetLogger<PresentationManager>();

        private readonly object updateLocker = new object();

        private readonly ITimer keepAliveTimer;

        private MasterPresentationContext context;

        private ScreenChanges currentScreenChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationManager"/> class.
        /// </summary>
        public PresentationManager()
        {
            this.MasterEngines = new List<MasterPresentationEngine>();
            this.keepAliveTimer = TimerFactory.Current.CreateTimer("ComposerKeepAlive");
            this.keepAliveTimer.Interval = TimeSpan.FromSeconds(15);
            this.keepAliveTimer.AutoReset = true;
            this.keepAliveTimer.Elapsed += this.KeepAliveTimerOnElapsed;
        }

        /// <summary>
        /// Event that is fired when there are screen changes available.
        /// </summary>
        public event EventHandler<ScreenChangesEventArgs> ScreensUpdated;

        /// <summary>
        /// Gets the config context.
        /// </summary>
        public IPresentationContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// Gets the master engines.
        /// </summary>
        public List<MasterPresentationEngine> MasterEngines { get; private set; }

        /// <summary>
        /// Configures this manager with the given config context.
        /// </summary>
        /// <param name="configContext">
        /// The config context.
        /// </param>
        public void Configure(IPresentationConfigContext configContext)
        {
            if (this.MasterEngines.Count > 0)
            {
                throw new NotSupportedException("Can't configure twice");
            }

            this.context = new MasterPresentationContext(configContext);

            foreach (var screen in configContext.Config.PhysicalScreens)
            {
                var engine = new MasterPresentationEngine(screen, this.context);
                engine.CurrentRootChanged += this.EngineOnCurrentRootChanged;
                this.MasterEngines.Add(engine);
            }

            this.context.CellsChanged += this.ContextOnCellsChanged;
            this.context.NextTimeReached += this.ContextOnNextTimeReached;
        }

        /// <summary>
        /// Starts this presentation manager and loads the layouts and cycles.
        /// </summary>
        public void Start()
        {
            Logger.Info("Starting PresentationManager");

            this.context.Start();

            this.keepAliveTimer.Enabled = true;

            this.UpdateEngines(x => this.MasterEngines.ForEach(e => e.Start()), this);

            MessageDispatcher.Instance.Subscribe<ScreenRequests>(this.OnScreenRequest);
        }

        /// <summary>
        /// Stops this presentation manager and loads the layouts and cycles.
        /// </summary>
        public void Stop()
        {
            Logger.Info("Stopping PresentationManager");
            MessageDispatcher.Instance.Unsubscribe<ScreenRequests>(this.OnScreenRequest);

            if (this.context != null)
            {
                this.context.Stop();
            }

            this.keepAliveTimer.Enabled = false;

            this.MasterEngines.ForEach(e => e.Stop());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Stop();

            this.keepAliveTimer.Dispose();

            if (this.context != null)
            {
                this.context.Dispose();
            }

            foreach (var engine in this.MasterEngines)
            {
                engine.CurrentRootChanged -= this.EngineOnCurrentRootChanged;
                engine.Dispose();
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            foreach (var masterPresentationEngine in this.MasterEngines)
            {
                yield return parent.Factory.CreateManagementProvider(
                    masterPresentationEngine.ScreenConfig.Name, parent, masterPresentationEngine);
            }
        }

        private void KeepAliveTimerOnElapsed(object source, EventArgs eventArgs)
        {
            Logger.Trace("Sending empty keep-alive screen change");
            MessageDispatcher.Instance.Broadcast(new ScreenChanges());
        }

        private void EngineOnCurrentRootChanged(object sender, EventArgs e)
        {
            var engine = sender as MasterPresentationEngine;
            if (engine == null)
            {
                return;
            }

            if (this.currentScreenChanges == null)
            {
                Logger.Warn("Getting screen change from {0} while not in update", engine.ScreenConfig.Name);
                return;
            }

            this.currentScreenChanges.Changes.Add(this.CreateScreenChange(engine));
        }

        private ScreenChange CreateScreenChange(MasterPresentationEngine engine)
        {
            Logger.Debug(
                "Creating screen change #{1} for {2} with {0} items",
                engine.CurrentRoot.Items.Count,
                engine.CurrentRoot.Id,
                engine.ScreenConfig.Name);

            if (Logger.IsTraceEnabled)
            {
                foreach (var item in engine.CurrentRoot.Items)
                {
                    Logger.Trace("- {0}", item);
                }
            }

            var change = new ScreenChange
                       {
                           Screen = new ScreenId
                                        {
                                            Type = engine.ScreenConfig.Type,
                                            Id = engine.ScreenConfig.Identifier
                                        },
                           ScreenRoot = engine.CurrentScreen
                       };
            foreach (var font in this.Context.Config.Config.Fonts)
            {
                if (font.ScreenType != engine.ScreenConfig.Type)
                {
                    continue;
                }

                // Required by CF 3.5
                // ReSharper disable RedundantTypeArgumentsOfMethod
                var parts = font.Path.Split(';');
                var paths = ArrayUtil.ConvertAll<string, string>(
                    parts, this.Context.Config.GetAbsolutePathRelatedToConfig);
                change.FontFiles.Add(string.Join(";", paths));
                // ReSharper restore RedundantTypeArgumentsOfMethod
            }

            return change;
        }

        private void ContextOnCellsChanged(object sender, TableEventArgs e)
        {
            this.UpdateEngines(this.context.NotifyCellsChanged, e.NewValues);
        }

        private void ContextOnNextTimeReached(object sender, TimeEventArgs e)
        {
            this.UpdateEngines(this.context.NotifyTimeReached, e.Time);
        }

        private void UpdateEngines<T>(Action<T> action, T arg)
        {
            ScreenChanges changes;
            lock (this.updateLocker)
            {
                this.currentScreenChanges = new ScreenChanges();
                this.context.RaiseUpdating(EventArgs.Empty);
                try
                {
                    action(arg);
                }
                finally
                {
                    var ev = new PresentationUpdatedEventArgs();
                    this.context.RaiseUpdated(ev);
                    changes = this.currentScreenChanges;
                    this.currentScreenChanges = null;
                    changes.Updates = ev.Updates;
                }

                if (changes.Changes.Count == 0 && changes.Updates.Count == 0)
                {
                    return;
                }

                var changesEv = new ScreenChangesEventArgs();
                changesEv.Changes = changes.Changes;
                changesEv.Updates = changes.Updates;
                this.RaiseScreensUpdated(changesEv);
            }

            if (Logger.IsTraceEnabled)
            {
                Logger.Trace("Broadcasting screen changes:");
                Logger.Trace("+ new screens:");
                foreach (var screenChange in changes.Changes)
                {
                    Logger.Trace(
                        "  - {0}:{1}: {2}", screenChange.Screen.Type, screenChange.Screen.Id, screenChange.ScreenRoot);
                }

                Logger.Trace("+ updates:");
                foreach (var screenUpdate in changes.Updates)
                {
                    Logger.Trace("  + Screen #{0}:", screenUpdate.RootId);
                    foreach (var itemUpdate in screenUpdate.Updates)
                    {
                        Logger.Trace(
                            "    - #{0}: {1}={2}", itemUpdate.ScreenItemId, itemUpdate.Property, itemUpdate.Value);
                    }
                }
            }

            this.keepAliveTimer.Enabled = false;
            MessageDispatcher.Instance.Broadcast(changes);
            this.keepAliveTimer.Enabled = true;
        }

        private void RaiseScreensUpdated(ScreenChangesEventArgs e)
        {
            var handler = this.ScreensUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnScreenRequest(object sender, MessageEventArgs<ScreenRequests> e)
        {
            var screenChanges = new ScreenChanges();

            foreach (var screen in e.Message.Screens)
            {
                foreach (var engine in this.MasterEngines)
                {
                    if (engine.ScreenConfig.Type != screen.ScreenId.Type)
                    {
                        // we don't support this screen type
                        continue;
                    }

                    if (engine.ScreenConfig.Identifier != null && engine.ScreenConfig.Identifier != screen.ScreenId.Id)
                    {
                        // we are not responsible for the given screen id
                        continue;
                    }

                    if (engine.ScreenConfig.Width != screen.Width || engine.ScreenConfig.Height != screen.Height)
                    {
                        // we don't support the given screen resolution
                        Logger.Warn(
                            "Got request for the wrong resolution: {0}x{1} but expected {2}x{3} for {4}",
                            screen.Width,
                            screen.Height,
                            engine.ScreenConfig.Width,
                            engine.ScreenConfig.Height,
                            engine.ScreenConfig.Name);
                        continue;
                    }

                    Logger.Debug(
                        "Handling a screen request from {0} for '{1}' ({2}x{3}) with {4}",
                        e.Source,
                        screen.ScreenId,
                        screen.Width,
                        screen.Height,
                        engine.ScreenConfig.Name);
                    screenChanges.Changes.Add(this.CreateScreenChange(engine));
                    break;
                }
            }

            if (screenChanges.Changes.Count > 0)
            {
                MessageDispatcher.Instance.Send(e.Source, screenChanges);
            }
        }
    }
}
