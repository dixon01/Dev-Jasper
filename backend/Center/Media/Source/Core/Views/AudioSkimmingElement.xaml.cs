// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioSkimmingElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AudioSkimmingElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for AudioSkimmingElement.xaml
    /// </summary>
    public partial class AudioSkimmingElement
    {
        /// <summary>
        /// The is paused property.
        /// </summary>
        public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register(
            "IsPaused",
            typeof(bool),
            typeof(AudioSkimmingElement),
            new PropertyMetadata(false));

        /// <summary>
        /// The is playing property.
        /// </summary>
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
            "IsPlaying",
            typeof(bool),
            typeof(AudioSkimmingElement),
            new PropertyMetadata(false));

        private readonly DispatcherTimer sliderUpdateTimer;

        private TimeSpan totalTime;

        private bool timerWasEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSkimmingElement"/> class.
        /// </summary>
        public AudioSkimmingElement()
        {
            this.InitializeComponent();

            this.Loaded += (sender, args) =>
            {
                this.AudioElement.MediaOpened += this.AudioElementOnMediaOpened;
                this.AudioElement.MediaEnded += this.AudioElementOnMediaEnded;
            };

            this.sliderUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.05) };
            this.sliderUpdateTimer.Tick += this.SliderUpdateTick;

            this.TimeLineSlider.AddHandler(
                MouseLeftButtonUpEvent,
                new MouseButtonEventHandler(this.TimeLineSliderMouseLeftButtonUp),
                true);

            this.TimeLineSlider.AddHandler(
                MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(this.TimeLineSliderMouseLeftButtonDown),
                true);

            AudioElement.Stop();
        }

        /// <summary>
        /// Gets or sets a value indicating whether is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return (bool)GetValue(IsPausedProperty);
            }

            set
            {
                SetValue(IsPausedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is playing.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return (bool)GetValue(IsPlayingProperty);
            }

            set
            {
                SetValue(IsPlayingProperty, value);
            }
        }

        private void SliderValueChanged(
            object sender, RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs)
        {
            this.CurrentTime.Text = string.Format("{0:m\\:ss}", this.AudioElement.Position);
        }

        private void TimeLineSliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.totalTime.TotalSeconds > 0)
            {
                this.AudioElement.Position = TimeSpan.FromSeconds(this.TimeLineSlider.Value);
            }

            if (this.timerWasEnabled)
            {
                this.sliderUpdateTimer.Start();
                if (this.IsPlaying)
                {
                    AudioElement.Play();
                }
            }
        }

        private void TimeLineSliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.sliderUpdateTimer.IsEnabled)
            {
                this.sliderUpdateTimer.Stop();
                this.timerWasEnabled = true;
                this.AudioElement.Pause();
            }
        }

        private void SliderUpdateTick(object sender, EventArgs e)
        {
            if (this.totalTime.TotalSeconds > 0)
            {
                this.TimeLineSlider.Value = this.AudioElement.Position.TotalSeconds;
            }
        }

        private void AudioElementOnMediaEnded(object sender, RoutedEventArgs e)
        {
            AudioElement.Stop();
            this.IsPlaying = false;
        }

        private void AudioElementOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            this.Duration.Text = string.Format(
                "{0}:{1:00}",
                (int)this.AudioElement.NaturalDuration.TimeSpan.TotalMinutes,
                this.AudioElement.NaturalDuration.TimeSpan.Seconds);

            this.CurrentTime.Text = string.Format(
                "{0}:{1:00}",
                (int)this.AudioElement.Position.TotalMinutes,
                this.AudioElement.Position.TotalSeconds);

            this.totalTime = AudioElement.NaturalDuration.TimeSpan;
            TimeLineSlider.Maximum = this.totalTime.TotalSeconds;
        }

        private void OnPlayPressed(object sender, RoutedEventArgs routedEventArgs)
        {
            AudioElement.Play();
            this.IsPaused = false;
            this.IsPlaying = true;
            this.sliderUpdateTimer.Start();
        }

        private void OnPausePressed(object sender, RoutedEventArgs routedEventArgs)
        {
            if (this.IsPaused)
            {
                this.sliderUpdateTimer.Start();
                AudioElement.Play();
            }
            else
            {
                this.sliderUpdateTimer.Stop();
                AudioElement.Pause();
            }

            this.IsPlaying = !this.IsPlaying;
            this.IsPaused = !this.IsPaused;
        }
    }
}
