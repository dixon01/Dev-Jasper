// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SkimmingElement.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for SkimmingElement.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for SkimmingElement.xaml
    /// </summary>
    public partial class SkimmingElement
    {
        private int videoLength;

        private double secondsStep;

        private double renderWidth;

        private TimeSpan videoPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkimmingElement"/> class.
        /// </summary>
        public SkimmingElement()
        {
            InitializeComponent();
            LoadPlaceholder.Visibility = Visibility.Visible;
            this.Loaded += (sender, args) =>
            {
                this.VideoElement.MediaOpened += VideoElementOnMediaOpened;
            };
        }

        private void VideoElementOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            LoadPlaceholder.Visibility = Visibility.Collapsed;
            this.videoLength = (int)this.VideoElement.NaturalDuration.TimeSpan.TotalSeconds;
            this.DimensionTextBlock.Text = string.Format(
                "{0}x{1}", this.VideoElement.NaturalVideoWidth, this.VideoElement.NaturalVideoHeight);
            this.DurationTextBlock.Text = string.Format(
                "{0}:{1:00}",
                (int)this.VideoElement.NaturalDuration.TimeSpan.TotalMinutes,
                this.VideoElement.NaturalDuration.TimeSpan.Seconds);
        }

        private void VideoElementOnMouseEnter(object sender, MouseEventArgs e)
        {
            this.renderWidth = this.VideoElement.RenderSize.Width;
            this.secondsStep = this.renderWidth / this.videoLength;
            var mousePosition = e.GetPosition(this.VideoElement);
            this.SetCurrentVideoPosition(mousePosition);
            this.TimeTextBlock.Text = string.Format(
                "{0}:{1:00}", (int)this.videoPosition.TotalMinutes, this.videoPosition.Seconds);
        }

        private void VideoElementOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this.VideoElement);
            this.SetCurrentVideoPosition(mousePosition);
        }

        private void SetCurrentVideoPosition(Point mousePosition)
        {
            var videoDifference = (int)(mousePosition.X / this.secondsStep);
            this.videoPosition = TimeSpan.FromSeconds(videoDifference);
            this.VideoElement.Position = this.videoPosition;
            this.TimeTextBlock.Text = string.Format(
                "{0}:{1:00}", (int)this.videoPosition.TotalMinutes, this.videoPosition.Seconds);
        }
    }
}
