// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaPlayer.xaml.cs" company="">
//   Author: Natraj Bontha
// </copyright>
// <summary>
//   Interaction logic for MediaPlayer.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.UIFramework.Common.MediaPlayer
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Win32;

    /// <summary>
    ///     Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        #region Static Fields

        /// <summary>
        ///     The media source property.
        /// </summary>
        public static readonly DependencyProperty MediaSourceProperty = DependencyProperty.Register(
            "MediaSource", typeof(string), typeof(MediaPlayer), new PropertyMetadata(string.Empty, OnMediaSourceChanged));

        /// <summary>
        /// The show media controls property.
        /// </summary>
        public static readonly DependencyProperty ShowMediaControlsProperty = DependencyProperty.Register(
            "ShowMediaControls", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, (dp, e) => ((MediaPlayer)dp).OnShowMediaControlsChanged((bool)e.NewValue)));

        /// <summary>
        ///     The show open folder property.
        /// </summary>
        public static readonly DependencyProperty ShowOpenFolderProperty = DependencyProperty.Register(
            "ShowOpenFolder", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, OnShowOpenFolderChanged));

        /// <summary>
        ///     The show position controls property.
        /// </summary>
        public static readonly DependencyProperty ShowPositionControlsProperty = DependencyProperty.Register(
            "ShowPositionControls", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, (dp, e) => ((MediaPlayer)dp).OnShowPositionControlsChanged((bool)e.NewValue)));

        // Using a DependencyProperty as the backing store for ShowFullScreenButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFullScreenProperty =
            DependencyProperty.Register("ShowFullScreen", typeof(bool), typeof(MediaPlayer), new PropertyMetadata(false, (dp, e) => ((MediaPlayer)dp).OnShowFullScreenChanged((bool)e.NewValue)));



        #endregion

        #region Fields

        /// <summary>
        ///     The _position timer.
        /// </summary>
        private readonly DispatcherTimer positionTimer;

        /// <summary>
        ///     The _time span tick.
        /// </summary>
        private readonly TimeSpan timeSpanTick = new TimeSpan(0, 0, 0, 0, (int)((1.0 / 29.97) * 1000));

        /// <summary>
        ///     The full screen.
        /// </summary>
        private bool fullScreen;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MediaPlayer" /> class.
        /// </summary>
        public MediaPlayer()
        {
            this.InitializeComponent();
            this.positionTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 1000), DispatcherPriority.Normal, this.DispatcherTimerTick, this.Dispatcher);
            this.ShowOpenFolder = true;
            this.ShowMediaControls = true;
            this.ShowPositionControls = true;
            this.ShowFullScreen = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the media source.
        /// </summary>
        [Description("MediaSource")]
        [Category("Control Settings")]
        public string MediaSource
        {
            get
            {
                return (string)this.GetValue(MediaSourceProperty);
            }

            set
            {
                this.SetValue(MediaSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show media controls.
        /// </summary>
        [Description("ShowMediaControls")]
        [Category("Control Settings")]
        public bool ShowMediaControls
        {
            get
            {
                return (bool)this.GetValue(ShowMediaControlsProperty);
            }

            set
            {
                this.SetValue(ShowMediaControlsProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether show open folder.
        /// </summary>
        [Description("ShowOpenFolder")]
        [Category("Control Settings")]
        public bool ShowOpenFolder
        {
            get
            {
                return (bool)this.GetValue(ShowOpenFolderProperty);
            }

            set
            {
                this.SetValue(ShowOpenFolderProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether show position controls.
        /// </summary>
        [Description("ShowPositionControls")]
        [Category("Control Settings")]
        public bool ShowPositionControls
        {
            get
            {
                return (bool)this.GetValue(ShowPositionControlsProperty);
            }

            set
            {
                this.SetValue(ShowPositionControlsProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether show position controls.
        /// </summary>
        [Description("ShowFullScreen")]
        [Category("Control Settings")]
        public bool ShowFullScreen
        {
            get
            {
                return (bool)this.GetValue(ShowFullScreenProperty);
            }
            set
            {
                this.SetValue(ShowFullScreenProperty, value);
            }
        }


        public void OnShowFullScreenChanged(bool value)
        {
            this.FullScreenMode.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The on media source changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependency object.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        public static void OnMediaSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var mediaPlayer = dependencyObject as MediaPlayer;
            if (eventArgs.NewValue != null)
            {
                var val = eventArgs.NewValue as string;
                if (mediaPlayer != null && val != null)
                {
                    mediaPlayer.VideoElement.Source = new Uri(val);
                }
            }
        }

        /// <summary>
        /// The on show open folder changed.
        /// </summary>
        /// <param name="dependencyObject">
        /// The dependencyObject.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        public static void OnShowOpenFolderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var mediaPlayer = dependencyObject as MediaPlayer;
            var val = (bool)eventArgs.NewValue;
            if (mediaPlayer != null)
            {
                mediaPlayer.OpenFile.Visibility = val ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The on show media controls changed.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnShowMediaControlsChanged(bool value)
        {
            this.MediaControls.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// The on show position controls changed.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnShowPositionControlsChanged(bool value)
        {
            this.PositionControlsGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispatcher timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            this.videoElementTime.Text = this.VideoElement.Position.ToString();
            if (!this.TimeLineSlider.IsFocused)
            {
                this.TimeLineSlider.Value = this.VideoElement.Position.TotalMilliseconds;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// The resume click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FullScreenClick(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (!this.fullScreen)
            {
                this.Background = new SolidColorBrush(Colors.Black);
                if (window != null)
                {
                    window.WindowStyle = WindowStyle.None;
                    window.WindowState = WindowState.Maximized;
                }

                this.VideoElement.Position = TimeSpan.FromSeconds(this.VideoElement.Position.TotalSeconds);
            }
            else
            {
                this.Background = new SolidColorBrush(Colors.SlateGray);
                if (window != null)
                {
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.WindowState = WindowState.Normal;
                }

                this.VideoElement.Position = TimeSpan.FromSeconds(this.VideoElement.Position.TotalSeconds);
            }

            this.fullScreen = !this.fullScreen;
        }

        /// <summary>
        /// The open click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.Filter = "mp3 files (*.mp3)|*.mp3|avi files (*.avi)|*.avi|wmv files (*.wmv)|*.wmv|All files (*.*)|*.*";
            dlg.Title = "Select Video File";
            dlg.InitialDirectory = Environment.SpecialFolder.MyVideos.ToString();

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                this.VideoElement.Source = new Uri(filename);
            }
        }

        /// <summary>
        /// The pause click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            this.VideoElement.Pause();
        }

        /// <summary>
        /// The play click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            this.VideoElement.Play();
        }

        /// <summary>
        /// The seek to media position.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void SeekToMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            var sliderValue = (int)this.TimeLineSlider.Value;

            // Overloaded constructor takes the arguments days, hours, minutes, seconds, miniseconds. 
            // Create a TimeSpan with miliseconds equal to the slider value.
            var ts = new TimeSpan(0, 0, 0, 0, sliderValue);
            this.VideoElement.Position = ts;
            this.videoElementTime.Text = this.VideoElement.Position.ToString();
        }

        /// <summary>
        /// The stop click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void StopClick(object sender, RoutedEventArgs e)
        {
            this.VideoElement.Stop();
        }

        /// <summary>
        /// The video element_ on media ended.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            this.VideoElement.Stop();
            this.VideoElement.Play();
        }

        /// <summary>
        /// The video element_ on media opened.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoElement_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            this.TimeLineSlider.Maximum = this.VideoElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        #endregion

        // Jump to different parts of the media (seek to).  
    }
}