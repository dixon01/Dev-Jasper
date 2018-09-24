namespace TextToSpeechTest
{
    using System;

    public abstract class AudioPart : IDisposable
    {
        public event EventHandler Completed;

        public abstract void Start();

        public abstract void Stop();

        protected virtual void RaiseCompleted(EventArgs e)
        {
            var handler = this.Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public abstract void Dispose();
    }
}