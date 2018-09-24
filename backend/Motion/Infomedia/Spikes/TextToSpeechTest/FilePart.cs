namespace TextToSpeechTest
{
    using NAudio.Wave;

    public class FilePart : AudioPart
    {
        private readonly WaveOut waveOut;

        private readonly AudioFileReader reader;

        public FilePart(string fileName)
        {
            this.waveOut = new WaveOut();
            this.reader = new AudioFileReader(fileName);
            this.waveOut.Init(this.reader);
            this.waveOut.PlaybackStopped += this.WaveOutOnPlaybackStopped;
        }

        public override void Start()
        {
            this.waveOut.Play();
        }

        public override void Stop()
        {
            this.waveOut.Stop();
        }

        public override void Dispose()
        {
            this.waveOut.Stop();
            this.reader.Dispose();
            this.waveOut.Dispose();
        }

        private void WaveOutOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            this.RaiseCompleted(stoppedEventArgs);
        }
    }
}