namespace Library.Tracking
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Library.Annotations;
    using Library.ServiceModel;

    public class TrackingModelBase : IDisposable, ITrackingModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<WritableModelUpdatedEventArgs> Updated;

        private Delta innerDelta;

        protected virtual void SetDelta(Delta delta)
        {
            this.innerDelta = delta;
        }

        protected virtual void OnUpdated()
        {
            this.OnUpdated(new WritableModelUpdatedEventArgs(this.innerDelta));
        }

        protected virtual void OnUpdated(WritableModelUpdatedEventArgs e)
        {
            var handler = this.Updated;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            this.OnUpdated();
            this.innerDelta.Dispose();
        }

        public void Dispose()
        {
            this.innerDelta.Dispose();
        }
    }

    public interface ITrackingModel
    {
        event EventHandler<WritableModelUpdatedEventArgs> Updated;
    }
}