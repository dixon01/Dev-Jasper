// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SignHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Gorba.Common.Configuration.Infomedia.AhdlcRenderer;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Renderer;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Signs;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using NLog;

    /// <summary>
    /// The handler responsible for a single sign.
    /// This handler manages all screen changes as well as all frames to and from the sign.
    /// </summary>
    public class SignHandler
    {
        private readonly object renderManagerLock = new object();
        private readonly object renderLock = new object();

        private readonly RenderManagerFactory renderManagerFactory;

        private readonly ITimer statusRequestTimer;

        private readonly ITimer alternationTimer;

        private readonly Dictionary<string, FontInfo> defaultFonts = new Dictionary<string, FontInfo>();
        private readonly Dictionary<string, FontInfo> fonts = new Dictionary<string, FontInfo>();

        private SignConfig config;

        private Logger logger;

        private SignRendererBase renderer;

        private AhdlcRenderContext context;

        private ScreenRootRenderManager<IAhdlcRenderContext> currentRenderManager;

        private bool signDimensionsVerified;

        private bool receivedStatusResponse;

        private bool sentInitialContent;

        private bool hadTimeout;

        private AhdlcRendererConfig rendererConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignHandler"/> class.
        /// </summary>
        public SignHandler()
        {
            this.renderManagerFactory = new RenderManagerFactory();

            this.statusRequestTimer = TimerFactory.Current.CreateTimer("SignStatusRequest");
            this.statusRequestTimer.Interval = TimeSpan.FromSeconds(20);
            this.statusRequestTimer.AutoReset = true;
            this.statusRequestTimer.Elapsed += this.StatusRequestTimerOnElapsed;

            this.alternationTimer = TimerFactory.Current.CreateTimer("Alternation");
            this.alternationTimer.AutoReset = false;
            this.alternationTimer.Elapsed += this.AlternationTimerOnElapsed;

            // load the default fonts from the resource
            this.LoadDefaultFont("G07X03_1.FON");
            this.LoadDefaultFont("G07X04_1.FON");
            this.LoadDefaultFont("G08X05U1.FON");
            this.LoadDefaultFont("G12X07U2.FON");
            this.LoadDefaultFont("G15X07_2.FON");
        }

        /// <summary>
        /// Event that is fired whenever new frames are created by this handler.
        /// </summary>
        public event EventHandler<FramesEventArgs> FramesCreated;

        /// <summary>
        /// Gets the screen id of this handler.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the actual width of the sign handled by this handler.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the actual height of the sign handled by this handler.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the AHDLC address of the sign handled by this class.
        /// </summary>
        public int Address { get; private set; }

        /// <summary>
        /// Configures this handler.
        /// </summary>
        /// <param name="signConfig">
        /// The sign config.
        /// </param>
        /// <param name="ahdlcRendererConfig">
        /// The renderer config.
        /// </param>
        public void Configure(SignConfig signConfig, AhdlcRendererConfig ahdlcRendererConfig)
        {
            this.config = signConfig;
            this.rendererConfig = ahdlcRendererConfig;
            this.Id = string.IsNullOrEmpty(this.config.Id)
                          ? this.config.Address.ToString(CultureInfo.InvariantCulture)
                          : this.config.Id;

            this.Width = this.config.Width;
            this.Height = this.config.Height;
            this.Address = this.config.Address;

            this.logger = LogManager.GetLogger(this.GetType().FullName + "-" + this.Id);
            this.renderer = SignRendererBase.Create(this.config);

            this.context = new AhdlcRenderContext(this);
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        public void Start()
        {
            this.logger.Info("Starting for address {0} ({1}x{2})", this.config.Address, this.Width, this.Height);
            this.sentInitialContent = false;
            this.statusRequestTimer.Enabled = true;
            this.SendStatusRequest();

            this.LoadScreen(
                new ScreenChange
                    {
                        Screen = new ScreenId { Type = PhysicalScreenType.LED, Id = this.Id },
                        ScreenRoot = this.CreateSplashScreen()
                    });
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            this.statusRequestTimer.Enabled = false;
            this.alternationTimer.Enabled = false;
        }

        /// <summary>
        /// Loads a new screen from the given <paramref name="change"/> (if the change is for us).
        /// </summary>
        /// <param name="change">
        /// The change.
        /// </param>
        /// <returns>
        /// True if the change was meant for us and we have handled it. Otherwise false.
        /// </returns>
        public bool LoadScreen(ScreenChange change)
        {
            if (change.Screen.Type != PhysicalScreenType.LED || change.Screen.Id != this.Id)
            {
                return false;
            }

            if (this.Width != change.ScreenRoot.Root.Width)
            {
                this.logger.Warn(
                    "Received a screen change with wrong width {0} instead of {1}",
                    change.ScreenRoot.Root.Width,
                    this.Width);
                return false;
            }

            if (this.Height != change.ScreenRoot.Root.Height)
            {
                this.logger.Warn(
                    "Received a screen change with wrong height {0} instead of {1}",
                    change.ScreenRoot.Root.Height,
                    this.Height);
                return false;
            }

            var root = (ScreenRoot)change.ScreenRoot.Clone();

            this.logger.Info("Screen changed. New screen will be shown: #{0}", change.ScreenRoot.Id);
            this.logger.Trace("New screen: {0}", root);

            // load (new) fonts
            foreach (var filename in change.FontFiles)
            {
                var found = false;
                foreach (var font in this.fonts.Values)
                {
                    if (filename.Equals(font.Filename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    continue;
                }

                var fontInfo = new FontInfo(filename);
                this.fonts[fontInfo.Name.Trim()] = fontInfo;

                this.logger.Debug("Loaded font '{0}' from {1}", fontInfo.Name, filename);
            }

            var manager = this.renderManagerFactory.CreateRenderManager(root);

            this.logger.Trace("Created new root render manager");

            this.ChangeRenderManager(manager);

            this.logger.Trace("Finished switching to new screen");
            return true;
        }

        /// <summary>
        /// Updates the sign handled by this handler with the given updates.
        /// </summary>
        /// <param name="updates">
        /// The updates.
        /// </param>
        public void UpdateScreen(IList<ScreenUpdate> updates)
        {
            this.logger.Trace("Updating render manager with {0} updates", updates.Count);

            lock (this.renderManagerLock)
            {
                if (this.currentRenderManager == null)
                {
                    return;
                }

                if (!this.currentRenderManager.Update(updates))
                {
                    this.logger.Trace("Render manager not updated");
                    return;
                }
            }

            this.Render(false);

            this.logger.Trace("Finished updating render manager");
        }

        /// <summary>
        /// Handles the status response received from the sign.
        /// </summary>
        /// <param name="statusResponse">
        /// The status response.
        /// </param>
        public void HandleStatusResponse(StatusResponseFrame statusResponse)
        {
            this.receivedStatusResponse = true;

            if (statusResponse.Temperature.HasValue)
            {
                var level = this.signDimensionsVerified ? LogLevel.Trace : LogLevel.Info;
                this.logger.Log(level, "Sign temperature: {0}°C", statusResponse.Temperature.Value);
            }

            if (this.signDimensionsVerified && !this.hadTimeout)
            {
                return;
            }

            this.signDimensionsVerified = true;
            this.VerifyDimensions(statusResponse);

            if (!this.hadTimeout)
            {
                return;
            }

            this.hadTimeout = false;
            this.Render(true);
        }

        /// <summary>
        /// Raises the <see cref="FramesCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseFrameCreated(FramesEventArgs e)
        {
            var handler = this.FramesCreated;
            if (handler != null)
            {
                this.statusRequestTimer.Enabled = false;
                try
                {
                    handler(this, e);
                }
                finally
                {
                    this.statusRequestTimer.Enabled = true;
                }
            }
        }

        private void LoadDefaultFont(string resourceName)
        {
            var font = FontInfo.FromResource(resourceName);
            if (font != null)
            {
                this.defaultFonts.Add(font.Name, font);
            }
        }

        private ScreenRoot CreateSplashScreen()
        {
            var splashScreen = new ScreenRoot { Root = new RootItem { Width = this.Width, Height = this.Height } };
            if (this.defaultFonts.Count == 0)
            {
                this.logger.Warn("Couldn't find any default font, creating empty splash screen.");
                return splashScreen;
            }

            string text;
            var version = new Version(ApplicationHelper.GetApplicationFileVersion()).ToString(3);
            var overflow = TextOverflow.WrapScale;
            if (this.config.Mode == SignMode.Text)
            {
                text = "GORBA AG - AHDLC  " + version;
                overflow = TextOverflow.Scroll;
            }
            else if (this.Width < 50)
            {
                text = "AHDLC[br]" + version;
            }
            else
            {
                text = "GORBA AG[br]AHDLC  " + version;
            }

            var fontNames = new string[this.defaultFonts.Count];
            this.defaultFonts.Keys.CopyTo(fontNames, 0);
            var fontFaces = string.Join(";", fontNames);
            splashScreen.Root.Items.Add(
                new TextItem
                    {
                        Text = text,
                        Align = HorizontalAlignment.Center,
                        VAlign = VerticalAlignment.Middle,
                        Overflow = overflow,
                        ScrollSpeed = 3,
                        Font = new Font { Face = fontFaces, Color = "White" },
                        X = 0,
                        Y = 0,
                        Width = this.Width,
                        Height = this.Height
                    });

            return splashScreen;
        }

        private void ChangeRenderManager(ScreenRootRenderManager<IAhdlcRenderContext> manager)
        {
            lock (this.renderManagerLock)
            {
                if (this.currentRenderManager != null)
                {
                    this.currentRenderManager.Dispose();
                }

                this.currentRenderManager = manager;
            }

            this.Render(true);
        }

        private void VerifyDimensions(StatusResponseFrame statusResponse)
        {
            this.logger.Info("Sign has version {0}", statusResponse.Version);
            this.logger.Info("Sign has DIP switches {0}", statusResponse.DipSwitch);

            if (this.Width == 0)
            {
                if (statusResponse.Width.HasValue)
                {
                    this.Width = statusResponse.Width.Value;
                }
                else
                {
                    this.logger.Warn("No width was defined and sign didn't report the width");
                }
            }

            if (this.Height == 0)
            {
                if (statusResponse.Height.HasValue)
                {
                    this.Height = statusResponse.Height.Value;
                }
                else
                {
                    this.logger.Warn("No height was defined and sign didn't report the height");
                }
            }

            if (statusResponse.Width.HasValue && this.Width != statusResponse.Width.Value)
            {
                this.logger.Warn(
                    "Received wrong width in status frame: {0} (expected {1})", statusResponse.Width, this.Width);
            }

            if (statusResponse.Height.HasValue && this.Height != statusResponse.Height.Value)
            {
                this.logger.Warn(
                    "Received wrong height in status frame: {0} (expected {1})", statusResponse.Height, this.Height);
            }

            if (statusResponse.SignType == StatusResponseFrame.SignTypes.Unknown)
            {
                this.logger.Warn("Sign type is unkown");
                return;
            }

            this.logger.Info("Sign type is {0}", statusResponse.SignType);
            if (statusResponse.SignType == StatusResponseFrame.SignTypes.Led && this.config.Mode == SignMode.Color)
            {
                this.logger.Warn("Sign doesn't support color but configured mode is {0}", this.config.Mode);
            }
        }

        private void Render(bool force)
        {
            var manager = this.currentRenderManager;
            if (manager == null)
            {
                return;
            }

            IFrameProvider provider = null;
            lock (this.renderLock)
            {
                this.context.Clear();
                manager.Update(this.context);
                manager.Render(1, this.context);

                if (!this.sentInitialContent || force || this.context.Changed)
                {
                    this.alternationTimer.Enabled = false;
                    provider = this.renderer.Render(this.context.Items, this.context);
                    this.sentInitialContent = true;
                }

                if (this.context.AlternationInterval.HasValue && this.context.AlternationInterval.Value > TimeSpan.Zero)
                {
                    this.alternationTimer.Interval = this.context.AlternationInterval.Value;
                    this.alternationTimer.Enabled = true;
                    this.context.AlternationCounter++;
                }
                else
                {
                    this.alternationTimer.Enabled = false;
                    this.context.AlternationCounter = 0;
                }
            }

            if (provider != null)
            {
                this.RaiseFrameCreated(new FramesEventArgs(provider));
            }
        }

        private void SendStatusRequest()
        {
            this.receivedStatusResponse = false;
            this.RaiseFrameCreated(new FramesEventArgs(new StatusRequestFrame { Address = this.config.Address }));
        }

        private void StatusRequestTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            if (!this.receivedStatusResponse)
            {
                this.logger.Warn("Didn't get status response");
                this.hadTimeout = true;
            }

            this.SendStatusRequest();
        }

        private void AlternationTimerOnElapsed(object sender, EventArgs e)
        {
            this.Render(true);
        }

        private class AhdlcRenderContext : IAhdlcRenderContext
        {
            private static readonly Logger Logger = LogHelper.GetLogger<AhdlcRenderContext>();

            private readonly SignHandler handler;

            private readonly Dictionary<string, OutlinedColoredFont> outlineFonts =
                new Dictionary<string, OutlinedColoredFont>();

            private readonly List<ComponentBase> items = new List<ComponentBase>();

            public AhdlcRenderContext(SignHandler handler)
            {
                this.handler = handler;
                this.Config = handler.rendererConfig;
            }

            public AhdlcRendererConfig Config { get; private set; }

            public long MillisecondsCounter { get; private set; }

            public int AlternationCounter { get; set; }

            public TimeSpan? AlternationInterval { get; set; }

            public bool Changed { get; private set; }

            public ICollection<ComponentBase> Items
            {
                get
                {
                    return this.items;
                }
            }

            public void Clear()
            {
                this.Changed = false;
                this.items.Clear();
                this.AlternationInterval = null;
                this.MillisecondsCounter = TimeProvider.Current.TickCount;
            }

            public void AddItem(ComponentBase component, bool changed)
            {
                this.items.Add(component);
                this.items.Sort((x, y) => x.ZIndex - y.ZIndex);
                if (changed)
                {
                    this.Changed = true;
                }
            }

            public IFont GetFont(Font font)
            {
                var mode = this.handler.config.Mode;
                if (mode == SignMode.Text)
                {
                    return null;
                }

                FontInfo fontInfo;
                if (!this.handler.fonts.TryGetValue(font.Face.Trim(), out fontInfo))
                {
                    if (!this.handler.defaultFonts.TryGetValue(font.Face.Trim(), out fontInfo))
                    {
                        Logger.Warn("Couldn't find font '{0}'", font.Face);
                        return null;
                    }
                }

                // check if the file changed and reload it if needed
                var update = fontInfo.CheckChanged();

                var outline = font.GetOutlineColor();
                if (mode == SignMode.Monochrome || !outline.HasValue)
                {
                    return new ColoredFont(fontInfo.Font, font.GetColor());
                }

                // special handling for OutlinedColoredFont since those fonts
                // cache their characters and are therefore expensive objects
                var fullName = string.Format("{0}<#>{1}<$>{2}", font.Face, font.Color, font.OutlineColor);
                OutlinedColoredFont outlineFont;
                if (!update && this.outlineFonts.TryGetValue(fullName, out outlineFont))
                {
                    return outlineFont;
                }

                outlineFont = new OutlinedColoredFont(fontInfo.Font, font.GetColor(), outline.Value);
                this.outlineFonts.Add(fullName, outlineFont);
                return outlineFont;
            }
        }
    }
}