// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Renderer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DxRenderVideoTest
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using DxRenderVideoTest.Engine;
    using DxRenderVideoTest.Event;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    using Microsoft.DirectX;
    using Microsoft.DirectX.Direct3D;
    using Microsoft.Samples.DirectX.UtilityToolkit;

    using NLog;

    using ComboBox = Microsoft.Samples.DirectX.UtilityToolkit.ComboBox;
    using Font = Microsoft.DirectX.Direct3D.Font;

    /// <summary>
    /// The video action.
    /// </summary>
    public enum VideoAction
    {
        /// <summary>
        /// The video full screen.
        /// </summary>
        VideoFullScreen,

        /// <summary>
        /// The video in one panel.
        /// </summary>
        VideoInOnePanel,

        /// <summary>
        /// The video in one form.
        /// </summary>
        VideoInOneForm,

        /// <summary>
        /// The video in two panels.
        /// </summary>
        VideoInTwoPanels,

        /// <summary>
        /// The video in two forms.
        /// </summary>
        VideoInTwoForms
    }

    /// <summary>
    /// DirectX renderer using Microsoft's sample framework.
    /// </summary>
    public class Renderer : IFrameworkCallback, IDeviceCreation
    {
        // HUD Ui Control constants
        private const int ToggleFullscreen = 1;

        private const int ToggleReference = 3;

        private const int ChangeDevice = 4;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // Variables
        private readonly Framework framework; // Framework for samples

        private readonly ModelViewerCamera camera = new ModelViewerCamera(); // A model viewing camera

        private readonly Dialog hud; // dialog for standard controls

        private readonly Dialog sampleUi; // dialog for sample specific controls

        private readonly DxRenderContext renderContext = new DxRenderContext();

        private readonly RenderManagerFactory renderManagerFactory;

        private readonly AlphaAnimator<RootRenderManager<IDxRenderContext>> rootRenderManagers =
            new AlphaAnimator<RootRenderManager<IDxRenderContext>>(null);

        private Font statsFont; // Font for drawing text

        private Sprite textSprite; // Sprite for batching text calls

        ////private Effect effect; // D3DX Effect Interface
        private bool isHelpShowing = true; // If true, renders the UI help text

        private int initialRequestTimer = -1;

        private RootItem currentScreen;

        private MdxVideoRenderer videoRenderer;
        private Panel videoPanel;
        private Panel videoPanel2;
        private Form videoForm;
        private Form videoForm2;
        private bool isFullScreen;

        private VideoAction currentVideoAction;
        private bool previousFullScreenState;

        /// <summary>
        /// Initializes a new instance of the <see cref="Renderer"/> class.
        /// </summary>
        /// <param name="framework">
        /// The framework.
        /// </param>
        public Renderer(Framework framework)
        {
            // Store framework
            this.framework = framework;

            this.renderManagerFactory = new RenderManagerFactory(framework);

            // Create dialogs
            this.hud = new Dialog(this.framework);
            this.sampleUi = new Dialog(this.framework);

            this.renderContext.Reset();
        }

        /// <summary>
        /// Initializes the application
        /// </summary>
        public void InitializeApplication()
        {
            // Set the callback functions. These functions allow the sample framework to notify
            // the application about device changes, user input, and windows messages.  The 
            // callbacks are optional so you need only set callbacks for events you're interested 
            // in. However, if you don't handle the device reset/lost callbacks then the sample 
            // framework won't be able to reset your device since the application must first 
            // release all device resources before resetting.  Likewise, if you don't handle the 
            // device created/destroyed callbacks then the sample framework won't be able to 
            // recreate your device resources.
            this.framework.Disposing += this.OnDestroyDevice;
            this.framework.DeviceLost += this.OnLostDevice;
            this.framework.DeviceCreated += this.OnCreateDevice;
            this.framework.DeviceReset += this.OnResetDevice;

            this.framework.SetWndProcCallback(this.OnMsgProc);

            this.framework.SetCallbackInterface(this);

            // Show the cursor and clip it when in full screen
            this.framework.SetCursorSettings(true, true);

            int y = 10;

            // Initialize the HUD
            var fullScreen = this.hud.AddButton(ToggleFullscreen, "Toggle full screen", 35, y, 125, 22);
            var toggleRef = this.hud.AddButton(ToggleReference, "Toggle reference (F3)", 35, y += 24, 125, 22);
            var changeDevice = this.hud.AddButton(ChangeDevice, "Change Device (F2)", 35, y += 24, 125, 22);

            // Hook the button events for when these items are clicked
            fullScreen.Click += this.OnFullscreenClicked;
            toggleRef.Click += this.OnRefClicked;
            changeDevice.Click += this.OnChangeDeviceClicked;

            // Now add the sample specific UI
            y = 10;
            const int ComboBox1 = ChangeDevice + 1;
            const int CheckBox1 = ChangeDevice + 2;
            const int CheckBox2 = ChangeDevice + 3;
            const int Radiobutton1 = ChangeDevice + 4;
            const int Radiobutton2 = ChangeDevice + 5;
            const int Radiobutton3 = ChangeDevice + 6;
            const int Button1 = ChangeDevice + 7;
            const int Button2 = ChangeDevice + 8;
            const int Radiobutton4 = ChangeDevice + 9;
            const int Radiobutton5 = ChangeDevice + 10;
            const int SliderControl = ChangeDevice + 11;

            ComboBox cb1 = this.sampleUi.AddComboBox(ComboBox1, 35, y += 24, 125, 22);
            for (int i = 0; i < 50; i++)
            {
                cb1.AddItem("Item#" + i, null);
            }

            this.sampleUi.AddCheckBox(CheckBox1, "Checkbox1", 35, y += 24, 125, 22, false);
            this.sampleUi.AddCheckBox(CheckBox2, "Checkbox2", 35, y += 24, 125, 22, false);
            this.sampleUi.AddRadioButton(Radiobutton1, 1, "Radio1G1", 35, y += 24, 125, 22, true);
            this.sampleUi.AddRadioButton(Radiobutton2, 1, "Radio2G1", 35, y += 24, 125, 22, false);
            this.sampleUi.AddRadioButton(Radiobutton3, 1, "Radio3G1", 35, y += 24, 125, 22, false);
            this.sampleUi.AddButton(Button1, "Button1", 35, y += 24, 125, 22);
            this.sampleUi.AddButton(Button2, "Button2", 35, y += 24, 125, 22);
            this.sampleUi.AddRadioButton(Radiobutton4, 2, "Radio1G2", 35, y += 24, 125, 22, true);
            this.sampleUi.AddRadioButton(Radiobutton5, 2, "Radio2G2", 35, y += 24, 125, 22, false);
            this.sampleUi.AddSlider(SliderControl, 50, y += 24, 100, 22);

            MessageDispatcher.Instance.Subscribe<ScreenChange>(this.OnScreenChange);
            MessageDispatcher.Instance.Subscribe<ScreenUpdate>(this.OnScreenUpdate);

            this.initialRequestTimer =
                this.framework.SetTimer(t => MessageDispatcher.Instance.Broadcast(new ScreenRequest()), 2);
        }

        /// <summary>
        /// Creates the framework's main window and the DirectX device.
        /// </summary>
        public void CreateWindow()
        {
            this.framework.CreateWindow("DxRender & Video Test");

            // Hook the keyboard event
            this.framework.Window.KeyDown += this.OnKeyEvent;

            this.framework.CreateDevice(0, true, Framework.DefaultSizeWidth, Framework.DefaultSizeHeight, this);

            // now it's my time to instantiate the objects:
            // 1) the panel(s) that will "host" (eventually) the video
            this.videoPanel = new Panel();
            this.videoPanel.Visible = false;
            this.videoPanel.Size = new Size(300, 200);
            this.framework.WindowForm.Controls.Add(this.videoPanel);
            this.videoPanel.Location = new Point(50, 50);

            this.videoPanel2 = new Panel();
            this.videoPanel2.Visible = false;
            this.videoPanel2.Size = new Size(400, 400);
            this.framework.WindowForm.Controls.Add(this.videoPanel2);
            this.videoPanel2.Location = new Point(100, 300);

            // 2) the window form(s) that will "host" (eventually) the video
            this.videoForm = new Form();
            this.videoForm.ShowInTaskbar = false;
            this.videoForm.Size = new Size(300, 200);
            this.videoForm.Location = new Point(50, 50);
            this.videoForm.Text = "Separate 1st Form With Video";

            this.videoForm2 = new Form();
            this.videoForm2.ShowInTaskbar = false;
            this.videoForm2.Size = new Size(300, 200);
            this.videoForm2.Location = new Point(100, 300);
            this.videoForm2.Text = "Separate 2th Form With Video";
            
            // 3) the video renderer.
            this.videoRenderer = new MdxVideoRenderer();
            this.videoRenderer.VideoEventReceived += this.OnVideoEventReceived;
            this.videoRenderer.SecondVideoEventReceived += this.OnSecondVideoEventReceived;
        }

        /// <summary>
        /// Called during device initialization, this code checks the device for some 
        /// minimum set of capabilities, and rejects those that don't pass by returning false.
        /// </summary>
        /// <param name="caps">
        /// The caps.
        /// </param>
        /// <param name="adapterFormat">
        /// The adapter format.
        /// </param>
        /// <param name="backBufferFormat">
        /// The back buffer format.
        /// </param>
        /// <param name="windowed">
        /// Are we in windowed or in full screen mode?
        /// </param>
        /// <returns>
        /// True if the device is acceptable.
        /// </returns>
        bool IDeviceCreation.IsDeviceAcceptable(Caps caps, Format adapterFormat, Format backBufferFormat, bool windowed)
        {
            // Skip back buffer formats that don't support alpha blending
            return Manager.CheckDeviceFormat(
                caps.AdapterOrdinal,
                caps.DeviceType,
                adapterFormat,
                Usage.QueryPostPixelShaderBlending,
                ResourceType.Textures,
                backBufferFormat);
        }

        /// <summary>
        /// This callback function is called immediately before a device is created to allow the 
        /// application to modify the device settings. The supplied settings parameter 
        /// contains the settings that the framework has selected for the new device, and the 
        /// application can make any desired changes directly to this structure.  Note however that 
        /// the sample framework will not correct invalid device settings so care must be taken 
        /// to return valid device settings, otherwise creating the Device will fail.  
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="caps">
        /// The caps.
        /// </param>
        void IDeviceCreation.ModifyDeviceSettings(DeviceSettings settings, Caps caps)
        {
            // If device doesn't support HW T&L or doesn't support 1.1 vertex shaders in HW 
            // then switch to SWVP.
            if ((!caps.DeviceCaps.SupportsHardwareTransformAndLight) || (caps.VertexShaderVersion < new Version(1, 1)))
            {
                settings.BehaviorFlags = CreateFlags.SoftwareVertexProcessing;
            }
            else
            {
                settings.BehaviorFlags = CreateFlags.HardwareVertexProcessing;
            }

            // This application is designed to work on a pure device by not using 
            // any get methods, so create a pure device if supported and using HWVP.
            if (caps.DeviceCaps.SupportsPureDevice
                && ((settings.BehaviorFlags & CreateFlags.HardwareVertexProcessing) != 0))
            {
                settings.BehaviorFlags |= CreateFlags.PureDevice;
            }

            // [WES] we need multi-threading
            settings.BehaviorFlags |= CreateFlags.MultiThreaded;

            // For the first device created if its a REF device, optionally display a warning dialog box
            if (settings.DeviceType == DeviceType.Reference)
            {
                Utility.DisplaySwitchingToRefWarning(this.framework, "EmptyProject");
            }
        }

        /// <summary>
        /// This callback function will be called once at the beginning of every frame. This is the
        /// best location for your application to handle updates to the scene, but is not 
        /// intended to contain actual rendering calls, which should instead be placed in the 
        /// OnFrameRender callback.  
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="appTime">
        /// The application time.
        /// </param>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        void IFrameworkCallback.OnFrameMove(Device device, double appTime, float elapsedTime)
        {
            // Update the camera's position based on user input 
            this.camera.FrameMove(elapsedTime);
        }

        /// <summary>
        /// This callback function will be called at the end of every frame to perform all the 
        /// rendering calls for the scene, and it will also be called if the window needs to be 
        /// repainted. After this function has returned, the sample framework will call 
        /// Device.Present to display the contents of the next buffer in the swap chain
        /// </summary>
        /// <param name="device">
        /// The device.
        /// </param>
        /// <param name="appTime">
        /// The application time.
        /// </param>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        void IFrameworkCallback.OnFrameRender(Device device, double appTime, float elapsedTime)
        {
            var screen = this.currentScreen;
            var parameters = this.framework.DeviceSettings.presentParams;
            if (screen != null && parameters.Windowed
                && (screen.Width != parameters.BackBufferWidth && // TODO: change back to OR!!!
                    screen.Height != parameters.BackBufferHeight))
            {
                var newSettings = this.framework.DeviceSettings.Clone();
                newSettings.presentParams.BackBufferWidth = screen.Width;
                newSettings.presentParams.BackBufferHeight = screen.Height;

                this.framework.CreateDeviceFromSettings(newSettings);
            }

            // Clear the render target and the zbuffer 
            device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1.0f, 0);
            device.BeginScene();
            try
            {
                // Update the effect's variables.  Instead of using strings, it would 
                // be more efficient to cache a handle to the parameter by calling 
                // Effect.GetParameter
                /* [WES]
                effect.SetValue("worldViewProjection", camera.WorldMatrix * camera.ViewMatrix * camera.ProjectionMatrix);
                effect.SetValue("worldMatrix", camera.WorldMatrix);
                effect.SetValue("appTime", (float)appTime);
                */

                // Show frame rate
                ////this.RenderText();

                this.Render();

                // Show UI
                ////this.hud.OnRender(elapsedTime);
                ////this.sampleUi.OnRender(elapsedTime);
            }
            finally
            {
                device.EndScene();
            }
        }

        private void OnScreenChange(object sender, MessageEventArgs<ScreenChange> e)
        {
            if (this.initialRequestTimer >= 0)
            {
                this.framework.KillTimer(this.initialRequestTimer);
                this.initialRequestTimer = -1;
            }

            this.LoadScreen(e.Message);
        }

        private void OnScreenUpdate(object sender, MessageEventArgs<ScreenUpdate> e)
        {
            lock (this.rootRenderManagers)
            {
                this.rootRenderManagers.DoWithValues((m, a) => m.Update(e.Message));
            }
        }

        private void LoadScreen(ScreenChange change)
        {
            lock (this.rootRenderManagers)
            {
                var manager = this.renderManagerFactory.CreateRenderManager(change.Root);
                this.rootRenderManagers.Animate(change.Animation, manager);
                this.currentScreen = change.Root;

                // TODO: make it configurable if we should reset the context here
                this.renderContext.Reset();
            }
        }

        private void Render()
        {
            lock (this.rootRenderManagers)
            {
                this.renderContext.Update();
                this.rootRenderManagers.Update(this.renderContext);

                this.rootRenderManagers.DoWithValues((m, a) => m.Render(a, this.renderContext));
            }
        }

        /// <summary>
        /// This event will be fired immediately after the Direct3D device has been 
        /// created, which will happen during application initialization and windowed/full screen 
        /// toggles. This is the best location to create Pool.Managed resources since these 
        /// resources need to be reloaded whenever the device is destroyed. Resources created  
        /// here should be released in the Disposing event. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnCreateDevice(object sender, DeviceEventArgs e)
        {
            // Initialize the stats font
            this.statsFont = ResourceCache.GetGlobalInstance()
                                          .CreateFont(
                                              e.Device,
                                              15,
                                              0,
                                              FontWeight.Bold,
                                              1,
                                              false,
                                              CharacterSet.Default,
                                              Precision.Default,
                                              FontQuality.Default,
                                              PitchAndFamily.FamilyDoNotCare | PitchAndFamily.DefaultPitch,
                                              "Arial");

            // Read the D3DX effect file
            /* [WES]
            ShaderFlags shaderFlags = ShaderFlags.NotCloneable;
            string path = Utility.FindMediaFile("EmptyProject.fx");
            effect = ResourceCache.GetGlobalInstance().CreateEffectFromFile(e.Device,
                path, null, null, shaderFlags, null);*/

            // Setup the camera's view parameters
            this.camera.SetViewParameters(new Vector3(0.0f, 0.0f, -5.0f), Vector3.Empty);
        }

        /// <summary>
        /// This event will be fired immediately after the Direct3D device has been 
        /// reset, which will happen after a lost device scenario. This is the best location to 
        /// create Pool.Default resources since these resources need to be reloaded whenever 
        /// the device is lost. Resources created here should be released in the OnLostDevice 
        /// event. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnResetDevice(object sender, DeviceEventArgs e)
        {
            SurfaceDescription desc = e.BackBufferDescription;

            // Create a sprite to help batch calls when drawing many lines of text
            this.textSprite = new Sprite(e.Device);

            // Setup the camera's projection parameters
            float aspectRatio = desc.Width / (float)desc.Height;
            this.camera.SetProjectionParameters((float)Math.PI / 4, aspectRatio, 0.1f, 1000.0f);
            this.camera.SetWindow(desc.Width, desc.Height);

            // Setup UI locations
            this.hud.SetLocation(desc.Width - 170, 0);
            this.hud.SetSize(170, 170);
            this.sampleUi.SetLocation(desc.Width - 170, desc.Height - 350);
            this.sampleUi.SetSize(170, 300);
        }

        /// <summary>
        /// This event function will be called fired after the Direct3D device has 
        /// entered a lost state and before Device.Reset() is called. Resources created
        /// in the OnResetDevice callback should be released here, which generally includes all 
        /// Pool.Default resources. See the "Lost Devices" section of the documentation for 
        /// information about lost devices.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnLostDevice(object sender, EventArgs e)
        {
            if (this.textSprite != null)
            {
                this.textSprite.Dispose();
                this.textSprite = null;
            }
        }

        /// <summary>
        /// This callback function will be called immediately after the Direct3D device has 
        /// been destroyed, which generally happens as a result of application termination or 
        /// windowed/full screen toggles. Resources created in the OnCreateDevice callback 
        /// should be released here, which generally includes all Pool.Managed resources. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnDestroyDevice(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Render the help and statistics text. This function uses the Font object for 
        /// efficient text rendering.
        /// </summary>
        private void RenderText()
        {
            var txtHelper = new TextHelper(this.statsFont, this.textSprite, 15);

            // Output statistics
            txtHelper.Begin();
            txtHelper.SetInsertionPoint(5, 5);
            txtHelper.SetForegroundColor(Color.Yellow);
            txtHelper.DrawTextLine(this.framework.FrameStats);
            txtHelper.DrawTextLine(this.framework.DeviceStats);

            txtHelper.SetForegroundColor(Color.White);
            txtHelper.DrawTextLine("Put some status text here.");

            // Draw help
            if (this.isHelpShowing)
            {
                txtHelper.SetInsertionPoint(10, this.framework.BackBufferSurfaceDescription.Height - (15 * 6));
                txtHelper.SetForegroundColor(Color.DarkOrange);
                txtHelper.DrawTextLine("Controls (F1 to hide):");

                txtHelper.SetInsertionPoint(40, this.framework.BackBufferSurfaceDescription.Height - (15 * 5));
                txtHelper.DrawTextLine("Help Item Misc: X");
                txtHelper.DrawTextLine("Quit: Esc");
                txtHelper.DrawTextLine("Hide help: F1");
            }
            else
            {
                txtHelper.SetInsertionPoint(10, this.framework.BackBufferSurfaceDescription.Height - (15 * 2));
                txtHelper.SetForegroundColor(Color.White);
                txtHelper.DrawTextLine("Press F1 for help");
            }

            txtHelper.End();
        }

        /// <summary>
        /// As a convenience, the sample framework inspects the incoming windows messages for
        /// keystroke messages and decodes the message parameters to pass relevant keyboard
        /// messages to the application.  The framework does not remove the underlying keystroke 
        /// messages, which are still passed to the application's MsgProc callback.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnKeyEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.F1:
                    this.isHelpShowing = !this.isHelpShowing;
                    break;

                case Keys.Q:
                    {
                        // the user wants to toggle the DX form's full screen feature.
                        this.framework.ToggleFullscreen();
                        this.isFullScreen = !this.isFullScreen;
                    }

                    break;

                case Keys.W:
                    {
                        if (!this.videoRenderer.IsPlaying)
                        {
                            // the user wants to play the video
                            this.previousFullScreenState = this.isFullScreen;
                            if (this.isFullScreen)
                            {
                                this.framework.ToggleFullscreen();
                                this.isFullScreen = !this.isFullScreen;
                            }

                            this.currentVideoAction = VideoAction.VideoFullScreen;
                            this.videoRenderer.Play();
                        }
                        else
                        {
                            // the user wants to stop the video
                            this.videoRenderer.Stop();

                            if (this.previousFullScreenState)
                            {
                                this.framework.ToggleFullscreen();
                                this.isFullScreen = !this.isFullScreen;
                            }
                        }
                    }

                    break;

                case Keys.E:
                    {
                        if (!this.videoRenderer.IsPlaying)
                        {
                            // the user wants to play the video in a panel
                            this.currentVideoAction = VideoAction.VideoInOnePanel;
                            this.PlayVideoInPanel();
                        }
                    }

                    break;

                case Keys.R:
                    {
                        if (!this.videoRenderer.IsPlaying)
                        {
                            // the user wants to play the video in a separate window form
                            this.currentVideoAction = VideoAction.VideoInOneForm;
                            this.PlayVideoInSeparateForm();
                        }
                    }

                    break;

                case Keys.T:
                    {
                        if (!this.videoRenderer.IsPlaying)
                        {
                            // the user wants to play the video in two embedded panels
                            this.currentVideoAction = VideoAction.VideoInTwoPanels;
                            this.PlayVideoInSeparatePanels();
                        }
                    }
                    break;


                case Keys.Z:
                    {
                        if (!this.videoRenderer.IsPlaying)
                        {
                            // the user wants to play the videos in two separate window forms
                            this.currentVideoAction = VideoAction.VideoInTwoForms;
                            this.PlayVideoInSeparateForms();
                        }
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Plays the video in a panel embedded in the DirectX form
        /// (if it's not already running).
        /// </summary>
        private void PlayVideoInPanel()
        {
            this.videoPanel.Visible = true;
            this.videoPanel.BringToFront();
            this.videoRenderer.Play(this.videoPanel);
        }

        /// <summary>
        /// Plays the video in a window form separate from the main one
        /// (if it's not already running).
        /// </summary>
        private void PlayVideoInSeparateForm()
        {
            this.videoForm.Visible = true;
            this.videoForm.WindowState = FormWindowState.Normal;
            this.videoForm.BringToFront();
            this.videoRenderer.Play(this.videoForm);
        }

        /// <summary>
        /// Plays the video in two panels embedded in the DirectX form
        /// (if it's not already running).
        /// </summary>
        private void PlayVideoInSeparatePanels()
        {
            this.PlayVideoInPanel();

            this.videoPanel2.Visible = true;
            this.videoPanel2.BringToFront();
            this.videoRenderer.PlaySecondVideo(this.videoPanel2);
        }

        /// <summary>
        /// Plays the video in two window forms separated from the main one
        /// (if it's not already running).
        /// </summary>
        private void PlayVideoInSeparateForms()
        {
            this.PlayVideoInSeparateForm();

            this.videoForm2.Visible = true;
            this.videoForm2.WindowState = FormWindowState.Normal;
            this.videoForm2.BringToFront();
            this.videoRenderer.PlaySecondVideo(this.videoForm2);
        }

        private void OnVideoEventReceived(object sender, VideoEventArgs e)
        {
            if (e.VideoState == VideoEvent.VideoStopped)
            {
                // I make sure that the video is stopped.
                if (this.currentVideoAction == VideoAction.VideoFullScreen)
                {
                    // the user wants to stop the video in fullscreen
                    this.videoRenderer.Stop();
                }

                if (this.currentVideoAction == VideoAction.VideoInOnePanel)
                {
                    // the user wants to stop the video in the embedded panel
                    this.videoPanel.Visible = false;
                    this.videoPanel.SendToBack();
                    this.videoRenderer.Stop();
                }

                if (this.currentVideoAction == VideoAction.VideoInOneForm)
                {
                    // the user wants to stop the video in the separate form
                    this.videoForm.Visible = false;
                    this.videoForm.WindowState = FormWindowState.Minimized;
                    this.videoRenderer.Stop();
                }

                if (this.currentVideoAction == VideoAction.VideoInTwoPanels)
                {
                    // the user wants to stop the videos in the two embedded panels 
                    this.videoPanel.Visible = false;
                    this.videoPanel.SendToBack();
                    this.videoRenderer.Stop();

                    this.videoPanel2.Visible = false;
                    this.videoPanel2.SendToBack();
                    this.videoRenderer.StopSecondVideo();
                }

                if (this.currentVideoAction == VideoAction.VideoInTwoForms)
                {
                    // the user wants to stop the videos in the two separate forms.
                    this.videoForm.Visible = false;
                    this.videoForm.WindowState = FormWindowState.Minimized;
                    this.videoRenderer.Stop();

                    this.videoForm2.Visible = false;
                    this.videoForm2.WindowState = FormWindowState.Minimized;
                    this.videoRenderer.StopSecondVideo();
                }

                if (this.previousFullScreenState)
                {
                    this.framework.ToggleFullscreen();
                    this.isFullScreen = !this.isFullScreen;
                }
            }
        }

        private void OnSecondVideoEventReceived(object sender, VideoEventArgs e)
        {
            if (e.VideoState == VideoEvent.VideoStopped)
            {
                // I make sure that the video is stopped.
                this.videoRenderer.StopSecondVideo();

                if (this.previousFullScreenState)
                {
                    this.framework.ToggleFullscreen();
                    this.isFullScreen = !this.isFullScreen;
                }
            }
        }


        /// <summary>
        /// Before handling window messages, the sample framework passes incoming windows 
        /// messages to the application through this callback function. If the application sets 
        /// endProcessing to true, the sample framework will not process the message
        /// </summary>
        /// <param name="windowHandle">
        /// The hWindow.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <param name="wordParam">
        /// The word param.
        /// </param>
        /// <param name="longParam">
        /// The long param.
        /// </param>
        /// <param name="endProcessing">
        /// The no further processing.
        /// </param>
        /// <returns>
        /// Always null.
        /// </returns>
        private IntPtr OnMsgProc(
            IntPtr windowHandle,
            NativeMethods.WindowMessage msg,
            IntPtr wordParam,
            IntPtr longParam,
            ref bool endProcessing)
        {
            if (endProcessing)
            {
                return IntPtr.Zero;
            }

            // Give the dialog a chance to handle the message first
            endProcessing = this.hud.MessageProc(windowHandle, msg, wordParam, longParam);
            if (endProcessing)
            {
                return IntPtr.Zero;
            }

            endProcessing = this.sampleUi.MessageProc(windowHandle, msg, wordParam, longParam);
            if (endProcessing)
            {
                return IntPtr.Zero;
            }

            // Pass all remaining windows messages to camera so it can respond to user input
            this.camera.HandleMessages(windowHandle, msg, wordParam, longParam);

            return IntPtr.Zero;
        }

        /// <summary>
        /// Called when the change device button is clicked
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnChangeDeviceClicked(object sender, EventArgs e)
        {
            this.framework.ShowSettingsDialog(!this.framework.IsD3DSettingsDialogShowing);
        }

        /// <summary>
        /// Called when the full screen button is clicked
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnFullscreenClicked(object sender, EventArgs e)
        {
            this.framework.ToggleFullscreen();
        }

        /// <summary>
        /// Called when the ref button is clicked
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnRefClicked(object sender, EventArgs e)
        {
            this.framework.ToggleReference();
        }

        private class DxRenderContext : IDxRenderContext
        {
            private int firstTime;

            public bool BlinkOn { get; private set; }

            public int AlternationCounter { get; private set; }

            public int ScrollCounter { get; private set; }

            public int MillisecondsCounter { get; private set; }

            public void Reset()
            {
                this.firstTime = Environment.TickCount;
            }

            public void Update()
            {
                this.MillisecondsCounter = Environment.TickCount;

                // TODO: make 3 seconds alt and 0.5 seconds blink configurable
                int diff = this.MillisecondsCounter - this.firstTime;
                this.AlternationCounter = diff / 3000;
                this.BlinkOn = ((diff / 500) % 2) == 0;
                this.ScrollCounter = diff;
            }
        }
    }
}