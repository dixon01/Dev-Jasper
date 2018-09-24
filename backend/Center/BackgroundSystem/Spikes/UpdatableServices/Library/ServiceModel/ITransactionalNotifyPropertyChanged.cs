namespace Library.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ITransactionalNotifyPropertyChanged : INotifyPropertyChanged
    {
        event EventHandler<TransactionCompletedEventArgs> TransactionCompleted;

        bool IsInTransaction { get; }

        // Can only be called once
        ITransaction BeginTransaction();
    }

    public interface ITransaction : IDisposable
    {
        Task Complete();
    }

    public class TransactionCompletedEventArgs : EventArgs
    {
        public TransactionCompletedEventArgs(Delta delta)
        {
            this.Delta = delta;
        }

        public Delta Delta { get; private set; }
    }
}