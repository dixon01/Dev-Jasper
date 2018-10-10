using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoSkimmingSpike
{
    /// <summary>
    /// Interaction logic for SkimmingElement.xaml
    /// </summary>
    public partial class SkimmingElement
    {
        private int videoLength;

        private double secondsStep;

        private double renderWidth;

        private TimeSpan videoPosition;

        private string fileName;

        public SkimmingElement()
        {
            InitializeComponent();
            this.Loaded += (sender, args) =>
                {
                    this.VideoElement.MediaOpened += VideoElementOnMediaOpened;
                };
        }

       public void OpenMedia(string fileName)
       {
           this.fileName = fileName;
           this.VideoElement.Source = new Uri(this.fileName);
           this.VideoElement.Play();
           this.VideoElement.Pause();
       }

        private void VideoElementOnMediaOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            this.videoLength = (int)this.VideoElement.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void VideoElementOnMouseEnter(object sender, MouseEventArgs e)
        {
            this.renderWidth = this.VideoElement.RenderSize.Width;
            this.secondsStep = this.renderWidth / this.videoLength;
            var mousePosition = e.GetPosition(this.VideoElement);
            this.SetCurrentVideoPosition(mousePosition);
        }

        private void VideoElementOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this.VideoElement);
            this.SetCurrentVideoPosition(mousePosition);
        }

        private void SetCurrentVideoPosition(Point mousePosition)
        {
            this.MousePositionText.Text = mousePosition.ToString();
            var videoDifference = (int)(mousePosition.X / this.secondsStep);
            this.videoPosition = TimeSpan.FromSeconds(videoDifference);
            this.VideoElement.Position = this.videoPosition;
            this.VideoPositionText.Text = this.videoPosition.ToString();
        }
    }
}
