namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System;
    using System.Collections;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Holds data for the Framework class, and all of the properties
    /// </summary>
    internal class FrameworkData
    {
        #region Instance Data
        private Device device; // the D3D rendering device

        private DeviceSettings  currentDeviceSettings; // current device settings
        private SurfaceDescription backBufferSurfaceDesc; // back buffer surface description
        private Caps caps; // D3D caps for current device

        private System.Windows.Forms.Control windowFocus; // the main app focus window
        private System.Windows.Forms.Control windowDeviceFullScreen; // the main app device window in fullscreen mode
        private System.Windows.Forms.Control windowDeviceWindowed; // the main app device window in windowed mode
        private IntPtr adapterMonitor; // the monitor of the adapter 
        private double currentTime; // current time in seconds
        private float elapsedTime; // time elapsed since last frame

        private System.Windows.Forms.FormStartPosition defaultStartingLocation; // default starting location of the window
        private System.Drawing.Rectangle clientRect; // client rect of window
        private System.Drawing.Rectangle fullScreenClientRect; // client rect of window when fullscreen
        private System.Drawing.Rectangle windowBoundsRect; // window rect of window
        private System.Drawing.Point windowLocation; // Location of the window
        
        private System.Windows.Forms.MainMenu windowMenu; // menu of app
        private double lastStatsUpdateTime; // last time the stats were updated
        private uint lastStatsUpdateFrames; // frames count since last time the stats were updated
        private float frameRate; // frames per second
        private int currentFrameNumber; // the current frame number

        private bool isHandlingDefaultHotkeys; // if true, the sample framework will handle some default hotkeys
        private bool isShowingMsgBoxOnError; // if true, then msgboxes are displayed upon errors
        private bool isClipCursorWhenFullScreen; // if true, then the sample framework will keep the cursor from going outside the window when full screen
        private bool isShowingCursorWhenFullScreen; // if true, then the sample framework will show a cursor when full screen
        private bool isConstantFrameTime; // if true, then elapsed frame time will always be 0.05f seconds which is good for debugging or automated capture
        private float timePerFrame; // the constant time per frame in seconds, only valid if isConstantFrameTime==true
        private bool isInWireframeMode; // if true, then RenderState.FillMode==FillMode.WireFrame else RenderState.FillMode==FillMode.Solid
        private bool canAutoChangeAdapter; // if true, then the adapter will automatically change if the window is different monitor
        private bool isWindowCreatedWithDefaultPositions; // if true, then default was used and the window should be moved to the right adapter
        private int applicationExitCode; // the exit code to be returned to the command line
        private bool isHidingStats; // if true, then stats are being hidden

        private bool isInited; // if true, then Init() has succeeded
        private bool wasWindowCreated; // if true, then CreateWindow() or SetWindow() has succeeded
        private bool wasDeviceCreated; // if true, then CreateDevice*() or SetDevice() has succeeded

        private bool isInitCalled; // if true, then Init() was called
        private bool isWindowCreateCalled; // if true, then CreateWindow() or SetWindow() was called
        private bool isDeviceCreateCalled; // if true, then CreateDevice*() or SetDevice() was called

        private bool isDeviceObjectsCreated; // if true, then DeviceCreated callback has been called (if non-NULL)
        private bool isDeviceObjectsReset; // if true, then DeviceReset callback has been called (if non-NULL)
        private bool isInsideDeviceCallback; // if true, then the framework is inside an app device callback
        private bool isInsideMainloop; // if true, then the framework is inside the main loop
        private bool isActive; // if true, then the app is the active top level window
        private bool isTimePaused; // if true, then time is paused
        private bool isRenderingPaused; // if true, then rendering is paused
        private int pauseRenderingCount; // pause rendering ref count
        private int pauseTimeCount; // pause time ref count
        private bool isDeviceLost; // if true, then the device is lost and needs to be reset
        private bool isMinimized; // if true, then the window is minimized
        private bool isMaximized; // if true, then the window is maximized
        private bool isSizeChangesIgnored; // if true, the sample framework won't reset the device upon window size change (for public use only)
        private bool isNotifyOnMouseMove; // if true, include WM_MOUSEMOVE in mousecallback

        private int overrideAdapterOrdinal; // if != -1, then override to use this adapter ordinal
        private bool overrideWindowed; // if true, then force to start windowed
        private bool overrideFullScreen; // if true, then force to start full screen
        private int overrideStartX; // if != -1, then override to this X position of the window
        private int overrideStartY; // if != -1, then override to this Y position of the window
        private int overrideWidth; // if != 0, then override to this width
        private int overrideHeight; // if != 0, then override to this height
        private bool overrideForceHAL; // if true, then force to Hardware device (failing if one doesn't exist)
        private bool overrideForceREF; // if true, then force to Reference device (failing if one doesn't exist)
        private bool overrideForcePureHWVP; // if true, then force to use pure Hardware VertexProcessing (failing if device doesn't support it)
        private bool overrideForceHWVP; // if true, then force to use Hardware VertexProcessing (failing if device doesn't support it)
        private bool overrideForceSWVP; // if true, then force to use Software VertexProcessing 
        private bool overrideConstantFrameTime; // if true, then force to constant frame time
        private float overrideConstantTimePerFrame; // the constant time per frame in seconds if overrideConstantFrameTime==true
        private int overrideQuitAfterFrame; // if != 0, then it will force the app to quit after that frame

        private IDeviceCreation deviceCallback; // Callback for device creation and acceptability
        private IFrameworkCallback frameworkCallback; // Framework callback interface
        private WndProcCallback wndFunc; // window messages callback

        private SettingsDialog settings; // The settings dialog
        private bool isShowingD3DSettingsDlg; // if true, then show the settings dialog

        private ArrayList timerList = new ArrayList(); // list of TimerData structs
        private string staticFrameStats; // static part of frames stats 
        private string frameStats; // frame stats (fps, width, etc)
        private string deviceStats; // device stats (description, device type, etc)
        private string windowTitle; // window title

        #endregion

        #region Properties
        public Device Device { get { return this.device; } set {this.device = value; } }
        public DeviceSettings CurrentDeviceSettings { get { return this.currentDeviceSettings; } set {this.currentDeviceSettings = value; } }
        public SurfaceDescription BackBufferSurfaceDesc { get { return this.backBufferSurfaceDesc; } set {this.backBufferSurfaceDesc = value; } }
        public Caps Caps { get { return this.caps; } set {this.caps = value; } }

        public System.Windows.Forms.Control WindowFocus { get { return this.windowFocus; } set {this.windowFocus = value; } }
        public System.Windows.Forms.Control WindowDeviceFullScreen { get { return this.windowDeviceFullScreen; } set {this.windowDeviceFullScreen = value; } }
        public System.Windows.Forms.Control WindowDeviceWindowed { get { return this.windowDeviceWindowed; } set {this.windowDeviceWindowed = value; } }
        public IntPtr AdapterMonitor { get { return this.adapterMonitor; } set {this.adapterMonitor = value; } }
        public double CurrentTime { get { return this.currentTime; } set {this.currentTime = value; } }
        public float ElapsedTime { get { return this.elapsedTime; } set {this.elapsedTime = value; } }

        public System.Windows.Forms.FormStartPosition DefaultStartingLocation { get { return this.defaultStartingLocation; } set {this.defaultStartingLocation = value; } }
        public System.Drawing.Rectangle ClientRectangle { get { return this.clientRect; } set {this.clientRect = value; } }
        public System.Drawing.Rectangle FullScreenClientRectangle { get { return this.fullScreenClientRect; } set {this.fullScreenClientRect = value; } }
        public System.Drawing.Rectangle WindowBoundsRectangle { get { return this.windowBoundsRect; } set {this.windowBoundsRect = value; } }
        public System.Drawing.Point ClientLocation { get { return this.windowLocation; } set {this.windowLocation = value; } }
        public System.Windows.Forms.MainMenu Menu { get { return this.windowMenu; } set {this.windowMenu = value; } }
        public double LastStatsUpdateTime { get { return this.lastStatsUpdateTime; } set {this.lastStatsUpdateTime = value; } }
        public uint LastStatsUpdateFrames { get { return this.lastStatsUpdateFrames; } set {this.lastStatsUpdateFrames = value; } }
        public float CurrentFrameRate { get { return this.frameRate; } set {this.frameRate = value; } }
        public int CurrentFrameNumber { get { return this.currentFrameNumber; } set {this.currentFrameNumber = value; } }

        public bool IsHandlingDefaultHotkeys { get { return this.isHandlingDefaultHotkeys; } set {this.isHandlingDefaultHotkeys = value; } }
        public bool IsShowingMsgBoxOnError { get { return this.isShowingMsgBoxOnError; } set {this.isShowingMsgBoxOnError = value; } }
        public bool AreStatsHidden { get { return this.isHidingStats; } set {this.isHidingStats = value; } }      
        public bool IsCursorClippedWhenFullScreen { get { return this.isClipCursorWhenFullScreen; } set {this.isClipCursorWhenFullScreen = value; } }
        public bool IsShowingCursorWhenFullScreen { get { return this.isShowingCursorWhenFullScreen; } set {this.isShowingCursorWhenFullScreen = value; } }
        public bool IsUsingConstantFrameTime { get { return this.isConstantFrameTime; } set {this.isConstantFrameTime = value; } }
        public float TimePerFrame { get { return this.timePerFrame; } set {this.timePerFrame = value; } }
        public bool IsInWireframeMode { get { return this.isInWireframeMode; } set {this.isInWireframeMode = value; } }
        public bool CanAutoChangeAdapter { get { return this.canAutoChangeAdapter; } set {this.canAutoChangeAdapter = value; } }
        public bool IsWindowCreatedWithDefaultPositions { get { return this.isWindowCreatedWithDefaultPositions; } set {this.isWindowCreatedWithDefaultPositions = value; } }
        public int ApplicationExitCode { get { return this.applicationExitCode; } set {this.applicationExitCode = value; } }

        public bool IsInited { get { return this.isInited; } set {this.isInited = value; } }
        public bool WasWindowCreated { get { return this.wasWindowCreated; } set {this.wasWindowCreated = value; } }
        public bool WasDeviceCreated { get { return this.wasDeviceCreated; } set {this.wasDeviceCreated = value; } }

        public bool WasInitCalled { get { return this.isInitCalled; } set {this.isInitCalled = value; } }
        public bool WasWindowCreateCalled { get { return this.isWindowCreateCalled; } set {this.isWindowCreateCalled = value; } }
        public bool WasDeviceCreateCalled { get { return this.isDeviceCreateCalled; } set {this.isDeviceCreateCalled = value; } }

        public bool AreDeviceObjectsCreated { get { return this.isDeviceObjectsCreated; } set {this.isDeviceObjectsCreated = value; } }
        public bool AreDeviceObjectsReset { get { return this.isDeviceObjectsReset; } set {this.isDeviceObjectsReset = value; } }
        public bool IsInsideDeviceCallback { get { return this.isInsideDeviceCallback; } set {this.isInsideDeviceCallback = value; } }
        public bool IsInsideMainloop { get { return this.isInsideMainloop; } set {this.isInsideMainloop = value; } }
        public bool IsActive { get { return this.isActive; } set {this.isActive = value; } }
        public bool IsTimePaused { get { return this.isTimePaused; } set {this.isTimePaused = value; } }
        public bool IsRenderingPaused { get { return this.isRenderingPaused; } set {this.isRenderingPaused = value; } }
        public int PauseRenderingCount { get { return this.pauseRenderingCount; } set {this.pauseRenderingCount = value; } }
        public int PauseTimeCount { get { return this.pauseTimeCount; } set {this.pauseTimeCount = value; } }
        public bool IsDeviceLost { get { return this.isDeviceLost; } set {this.isDeviceLost = value; } }
        public bool IsMinimized { get { return this.isMinimized; } set {this.isMinimized = value; } }
        public bool IsMaximized { get { return this.isMaximized; } set {this.isMaximized = value; } }
        public bool AreSizeChangesIgnored { get { return this.isSizeChangesIgnored; } set {this.isSizeChangesIgnored = value; } }
        public bool IsNotifiedOnMouseMove { get { return this.isNotifyOnMouseMove; } set {this.isNotifyOnMouseMove = value; } }

        public int OverrideAdapterOrdinal { get { return this.overrideAdapterOrdinal; } set {this.overrideAdapterOrdinal = value; } }
        public bool IsOverridingWindowed { get { return this.overrideWindowed; } set {this.overrideWindowed = value; } }
        public bool IsOverridingFullScreen { get { return this.overrideFullScreen; } set {this.overrideFullScreen = value; } }
        public int OverrideStartX { get { return this.overrideStartX; } set {this.overrideStartX = value; } }
        public int OverrideStartY { get { return this.overrideStartY; } set {this.overrideStartY = value; } }
        public int OverrideWidth { get { return this.overrideWidth; } set {this.overrideWidth = value; } }
        public int OverrideHeight { get { return this.overrideHeight; } set {this.overrideHeight = value; } }
        public bool IsOverridingForceHardware { get { return this.overrideForceHAL; } set {this.overrideForceHAL = value; } }
        public bool IsOverridingForceReference { get { return this.overrideForceREF; } set {this.overrideForceREF = value; } }
        public bool IsOverridingForcePureHardwareVertexProcessing { get { return this.overrideForcePureHWVP; } set {this.overrideForcePureHWVP = value; } }
        public bool IsOverridingForceHardwareVertexProcessing { get { return this.overrideForceHWVP; } set {this.overrideForceHWVP = value; } }
        public bool IsOverridingForceSoftwareVertexProcessing { get { return this.overrideForceSWVP; } set {this.overrideForceSWVP = value; } }
        public bool IsOverridingConstantFrameTime { get { return this.overrideConstantFrameTime; } set {this.overrideConstantFrameTime = value; } }
        public float OverrideConstantTimePerFrame { get { return this.overrideConstantTimePerFrame; } set {this.overrideConstantTimePerFrame = value; } }
        public int OverrideQuitAfterFrame { get { return this.overrideQuitAfterFrame; } set {this.overrideQuitAfterFrame = value; } }

        public IDeviceCreation DeviceCreationInterface { get { return this.deviceCallback; } set { this.deviceCallback = value; } }
        public IFrameworkCallback CallbackInterface { get { return this.frameworkCallback; } set {this.frameworkCallback = value; } }
        public WndProcCallback WndProcFunction { get { return this.wndFunc; } set {this.wndFunc = value; } }
        
        public SettingsDialog Settings { get { return this.settings; } set {this.settings = value; } }
        public bool IsD3DSettingsDialogShowing { get { return this.isShowingD3DSettingsDlg; } set {this.isShowingD3DSettingsDlg = value; } }

        public ArrayList Timers { get { return this.timerList; } set {this.timerList = value; } }
        public string StaticFrameStats { get { return this.staticFrameStats; } set {this.staticFrameStats = value; } }
        public string FrameStats { get { return this.frameStats; } set {this.frameStats = value; } }
        public string DeviceStats { get { return this.deviceStats; } set {this.deviceStats = value; } }
        public string WindowTitle { get { return this.windowTitle; } set {this.windowTitle = value; } }
        #endregion

        /// <summary>
        /// Initialize data
        /// </summary>
        public FrameworkData()
        {
            // Set some initial data
            this.overrideStartX = -1;
            this.overrideStartY = -1;
            this.overrideAdapterOrdinal = -1;
            this.canAutoChangeAdapter = true;
            this.isShowingMsgBoxOnError = true;
            this.isActive = true;
            this.defaultStartingLocation = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
        }
    }
}