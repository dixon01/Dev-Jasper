namespace Library.ViewModel
{
    using System.ComponentModel;

    using Library.ServiceModel;

    public abstract class TrackingDataViewModelBase<T> : ViewModelBase
    {
        public T Model { get; private set; }

        public TrackingDataViewModelBase(T model)
        {
            this.Model = model;
        }
    }
}