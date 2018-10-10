// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteViewerSectionViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteViewerSectionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using RemoteViewing.Vnc;

    /// <summary>
    /// The remote viewer section view model.
    /// </summary>
    public class RemoteViewerSectionViewModel : InfoSectionViewModelBase, IDisposable
    {
        private bool isConnected;

        private string password;

        private bool isInteractive;

        private bool supportsInteractive;

        private bool isShiftPressed;

        private bool isCtrlPressed;

        private bool isWinPressed;

        private bool isAltPressed;

        private bool isFitToScreen;

        private Stretch stretchMode;

        private ScrollBarVisibility scrollMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewerSectionViewModel"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public RemoteViewerSectionViewModel(UnitViewModelBase unit)
            : base(unit)
        {
            // ReSharper disable once StringLiteralTypo
            this.Password = "2wsxdr5";

            this.VncClient = new VncClient { MaxUpdateRate = 5 };

            this.PropertyChanged += this.OnPropertyChanged;

            this.IsFitToScreen = true;
        }

        /// <summary>
        /// Gets or sets the scroll mode for the remote view.
        /// </summary>
        public ScrollBarVisibility ScrollMode
        {
            get
            {
                return this.scrollMode;
            }

            set
            {
                this.SetProperty(ref this.scrollMode, value, () => this.ScrollMode);
            }
        }

        /// <summary>
        /// Gets the VNC client that will provide the remote connection.
        /// </summary>
        public VncClient VncClient { get; private set; }

        /// <summary>
        /// Gets or sets the bitmap that contains the current view of the VNC client.
        /// </summary>
        public WriteableBitmap Bitmap { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.SetProperty(ref this.password, value, () => this.Password);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the VNC viewer is connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                this.SetProperty(ref this.isConnected, value, () => this.IsConnected);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application supports interactive mode.
        /// </summary>
        public bool SupportsInteractive
        {
            get
            {
                return this.supportsInteractive && this.IsConnected;
            }

            set
            {
                this.SetProperty(ref this.supportsInteractive, value, () => this.SupportsInteractive);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is the viewer is in interactive mode.
        /// </summary>
        public bool IsInteractive
        {
            get
            {
                return this.isInteractive;
            }

            set
            {
                this.SetProperty(ref this.isInteractive, value, () => this.IsInteractive);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the virtual shift key is pressed.
        /// </summary>
        public bool IsShiftPressed
        {
            get
            {
                return this.isShiftPressed;
            }

            set
            {
                this.SetProperty(ref this.isShiftPressed, value, () => this.IsShiftPressed);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is fit to screen.
        /// </summary>
        public bool IsFitToScreen
        {
            get
            {
                return this.isFitToScreen;
            }

            set
            {
                this.SetProperty(ref this.isFitToScreen, value, () => this.IsFitToScreen);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the virtual control key is pressed.
        /// </summary>
        public bool IsCtrlPressed
        {
            get
            {
                return this.isCtrlPressed;
            }

            set
            {
                this.SetProperty(ref this.isCtrlPressed, value, () => this.IsCtrlPressed);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the virtual windows key is pressed.
        /// </summary>
        public bool IsWinPressed
        {
            get
            {
                return this.isWinPressed;
            }

            set
            {
                this.SetProperty(ref this.isWinPressed, value, () => this.IsWinPressed);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the virtual alt key is pressed.
        /// </summary>
        public bool IsAltPressed
        {
            get
            {
                return this.isAltPressed;
            }

            set
            {
                this.SetProperty(ref this.isAltPressed, value, () => this.IsAltPressed);
            }
        }

        /// <summary>
        /// Gets or sets the connect command which accepts a <see cref="VncClient"/> as an argument.
        /// </summary>
        public ICommand ConnectCommand { get; set; }

        /// <summary>
        /// Gets or sets the stretch mode.
        /// </summary>
        public Stretch StretchMode
        {
            get
            {
                return this.stretchMode;
            }

            set
            {
                this.SetProperty(ref this.stretchMode, value, () => this.StretchMode);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.VncClient.Close();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsFitToScreen"))
            {
                return;
            }

            if (this.IsFitToScreen)
            {
                this.StretchMode = Stretch.Uniform;
                this.ScrollMode = ScrollBarVisibility.Disabled;
            }
            else
            {
                this.StretchMode = Stretch.None;
                this.ScrollMode = ScrollBarVisibility.Auto;
            }
        }
    }
}
