namespace TextToSpeechTest
{
    using System;
    using System.Collections.Generic;

    public class AudioList : IDisposable
    {
        private readonly List<AudioPart> parts = new List<AudioPart>();

        private int playIndex = -1;

        public event EventHandler Completed;

        public void AddFile(string fileName)
        {
            if (this.playIndex >= 0)
            {
                throw new NotSupportedException("Can't add items while playing");
            }

            this.parts.Add(new FilePart(fileName));
        }

        public void AddSpeech(SpeechPartBase part)
        {
            if (this.playIndex >= 0)
            {
                throw new NotSupportedException("Can't add items while playing");
            }

            this.parts.Add(part);
        }

        public void Start()
        {
            if (this.playIndex >= 0)
            {
                throw new NotSupportedException("Can't start playing twice");
            }

            this.PlayNext();
        }

        private void PlayNext()
        {
            if (this.playIndex >= 0)
            {
                this.parts[this.playIndex].Completed -= this.PartOnCompleted;
            }

            if (++this.playIndex >= this.parts.Count)
            {
                this.RaiseCompleted(EventArgs.Empty);
                return;
            }

            var part = this.parts[this.playIndex];
            part.Completed += this.PartOnCompleted;
            part.Start();
        }

        public void Stop()
        {
            var index = this.playIndex;
            if (index >= this.parts.Count)
            {
                return;
            }

            var part = this.parts[index];
            part.Stop();
            part.Completed -= this.PartOnCompleted;
            this.RaiseCompleted(EventArgs.Empty);
        }

        protected virtual void RaiseCompleted(EventArgs e)
        {
            var handler = this.Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PartOnCompleted(object sender, EventArgs eventArgs)
        {
            this.PlayNext();
        }

        public void Dispose()
        {
            foreach (var part in this.parts)
            {
                part.Dispose();
            }
        }
    }
}
